using Google.Protobuf;
using Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class NetworkManager : IDisposable
{
    public bool IsHost { get; private set; }

    private const int PORT = 19826;
    private const int BUFFER_SIZE = 1 << 16;
    private const int PACKET_SIZE = 1 << 10;

    private UdpClient _udpClient;
    private CancellationTokenSource _cts;
    private PacketHandler _packetHandler;
    private ConcurrentQueue<ReceivedPacket> _receivedPackets;

    private IPEndPoint _hostEndPoint;
    private int _hostSequence, _clientSequence;
    private List<IPEndPoint> _clientEndPoints;
    private Dictionary<IPEndPoint, int> _clientLastReceivedSequence;
    private int _hostLastReceivedSequence;

    public NetworkManager()
    {
        _packetHandler = new PacketHandler();
        _cts = new CancellationTokenSource();
        _receivedPackets = new ConcurrentQueue<ReceivedPacket>();
    }
    public void TickProcessPacketQueue()
    {
        while (_receivedPackets.TryDequeue(out ReceivedPacket receivedPacket))
        {
            _packetHandler.RoutePacket(receivedPacket.Packet, receivedPacket.Sender);
        }
    }

    public void Dispose()
    {
        if (_cts != null)
        {
            _cts.Cancel();
        }

        if (_udpClient != null)
        {
            _udpClient.Close();
            _udpClient = null;
            Log.Info("disposion complete.");
        }
    }

    public void StartAsClient(string hostIP)
    {
        IsHost = false;
        try
        {
            _hostEndPoint = new IPEndPoint(IPAddress.Parse(hostIP), PORT);
            _clientSequence = -1;
            _hostLastReceivedSequence = -1;

            _udpClient = new UdpClient(0);
            _udpClient.Client.ReceiveBufferSize = BUFFER_SIZE;

            Log.Info($"Client started on port {_udpClient.Client.LocalEndPoint}");
            Task.Run(() => ReceiveLoop(_cts.Token));
        }
        catch (Exception ex)
        {
            Log.Error($"Client failed to start: {ex.Message}");
        }
    }

    public void StartAsHost()
    {
        IsHost = true;

        string loopbackIP = "127.0.0.1";
        try
        {
            _hostEndPoint = new IPEndPoint(IPAddress.Parse(loopbackIP), PORT);
            _clientEndPoints = new List<IPEndPoint>();
            _clientSequence = -1;
            _clientLastReceivedSequence = new Dictionary<IPEndPoint, int>();
            _hostSequence = -1;
            _hostLastReceivedSequence = -1;

            _udpClient = new UdpClient(PORT);
            _udpClient.Client.ReceiveBufferSize = BUFFER_SIZE;

            Log.Info($"Host started and listening on port {PORT}");
            Task.Run(() => ReceiveLoop(_cts.Token));
        }
        catch (Exception ex)
        {
            Log.Error($"Host failed to start: {ex.Message}");
        }
    }
    public void SendToHost<T>(PacketType type, T message) where T : IMessage<T>
    {
        if (_udpClient == null) return;

        try
        {
            byte[] serializedData = Serialize(type, message);
            if (serializedData == null) return;
            _udpClient.Send(serializedData, serializedData.Length, _hostEndPoint);
        }
        catch (Exception ex)
        {
            Log.Error($"UDP Send Error: {ex.Message}");
        }
    }

    public void SendToClient<T>(PacketType type, T message, IPEndPoint target) where T : IMessage<T>
    {
        if (_udpClient == null) return;

        if (!IsHost)
        {
            Log.Error("A client cannot send directly to another client");
            return;
        }
        try
        {
            byte[] serializedData = Serialize(type, message);
            if (serializedData == null) return;
            _udpClient.Send(serializedData, serializedData.Length, target);
        }
        catch (Exception ex)
        {
            Log.Error($"UDP Send Error: {ex.Message}");
        }
    }

    public void SendByBroadcast<T>(PacketType type, T message) where T : IMessage<T>
    {
        if (_udpClient == null) return;

        if (!IsHost)
        {
            Log.Error("A client cannot broadcast");
            return;
        }

        byte[] serializedData = Serialize(type, message);
        if (serializedData == null) return;

        foreach (IPEndPoint target in _clientEndPoints)
        {
            try
            {
                _udpClient.Send(serializedData, serializedData.Length, target);
            }
            catch (Exception ex)
            {
                Log.Error($"UDP Send Error: {ex.Message}");
            }
        }
    }
    private async Task ReceiveLoop(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            UdpReceiveResult result;
            IPEndPoint sender;
            try
            {
                result = await _udpClient.ReceiveAsync();
                sender = result.RemoteEndPoint;
                byte[] data = result.Buffer;
                if (IsHost)
                {
                    if (!_clientEndPoints.Contains(sender))
                    {
                        _clientEndPoints.Add(sender);
                        Log.Info($"New client connected: {sender}");
                    }

                    if (!_clientLastReceivedSequence.ContainsKey(sender))
                    {
                        _clientLastReceivedSequence.Add(sender, -1);
                    }
                }

                if (data.Length > 0)
                {
                    NetworkPacket packet = Deserialize(data);
                    if (packet == null) continue;

                    if (IsHost)
                    {
                        if (_clientLastReceivedSequence[sender] < packet.Sequence)
                        {
                            _receivedPackets.Enqueue(new ReceivedPacket(packet, sender));
                            _clientLastReceivedSequence[sender] = packet.Sequence;
                        }
                    }
                    else
                    {
                        if (_hostLastReceivedSequence < packet.Sequence)
                        {
                            _receivedPackets.Enqueue(new ReceivedPacket(packet, sender));
                            _hostLastReceivedSequence = packet.Sequence;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (!_cts.IsCancellationRequested)
                {
                    Log.Error($"UDP Receive Error: {ex.Message}");
                }
            }
        }
    }

    private byte[] Serialize<T>(PacketType type, T message) where T : IMessage<T>
    {
        byte[] data = message.ToByteArray();

        NetworkPacket packet = new NetworkPacket()
        {
            Type = type,
            Sequence = IsHost ? GetNextHostSequence() : GetNextClientSequence(),    //<======== 여기 고쳐야함
            Data = ByteString.CopyFrom(data)
        };
        byte[] sendBytes = packet.ToByteArray();
        if (sendBytes.Length > PACKET_SIZE)
        {
            Log.Error($"Packet size ({sendBytes.Length} bytes) exceeds the safety limit ({PACKET_SIZE} bytes).");
            return null;
        }
        return sendBytes;
    }

    private NetworkPacket Deserialize(byte[] data)
    {
        try
        {
            MessageParser<NetworkPacket> parser = NetworkPacket.Parser;
            return parser.ParseFrom(data);
        }
        catch (Exception ex)
        {
            Log.Error($"Protobuf 역직렬화 실패: {ex.Message}");
            return null;
        }
    }

    private int GetNextHostSequence()
    {
        return System.Threading.Interlocked.Increment(ref _hostSequence);
    }

    private int GetNextClientSequence()
    {
        return System.Threading.Interlocked.Increment(ref _clientSequence);
    }

    private class ReceivedPacket
    {
        public NetworkPacket Packet { get; set; }
        public IPEndPoint Sender { get; set; }

        public ReceivedPacket(NetworkPacket packet, IPEndPoint sender)
        {
            Packet = packet;
            Sender = sender;
        }
    }
}

