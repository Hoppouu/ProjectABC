using Google.Protobuf;
using Network;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Network
{
    public enum NetworkRole
    {
        HOST,
        CLIENT
    }

    public class PacketTransmitter : IDisposable
    {
        public bool IsHost { get; private set; }

        private const int _PORT = 19826;
        private const int _BUFFER_SIZE = 1 << 16;
        private const int _PACKET_SIZE = 1 << 10;

        private UdpClient _udpClient;
        private CancellationTokenSource _cts;
        private ConcurrentQueue<ReceivedPacket> _receivedPackets;

        private IPEndPoint _hostEndPoint;
        private int _clientSendHostLastSeq, _clientReceivedHostLastSeq;

        //Host전용
        private ConcurrentDictionary<IPEndPoint, int> _hostSendClientLastSeq, _hostReceivedClientLastSeq;
        private ConcurrentDictionary<IPEndPoint, byte> _clientEndPoints;

        /// <param name="role">NetworkRole 열거형 사용.</param>
        /// <param name="hostIP">클라이언트 일 경우 넣어야함.</param>
        public PacketTransmitter(NetworkRole role, string hostIP = "")
        {
            _cts = new CancellationTokenSource();
            _receivedPackets = new ConcurrentQueue<ReceivedPacket>();

            switch (role)
            {
                case NetworkRole.HOST:
                    StartAsHost();
                    break;

                case NetworkRole.CLIENT:
                    if (String.IsNullOrEmpty(hostIP)) throw new ArgumentException("Client role requires a valid host IP");
                    StartAsClient(hostIP);
                    break;
            }
        }
        ~PacketTransmitter()
        {
            this.Dispose();
        }

        public bool TryDequeuePacket(out ReceivedPacket packet)
        {
            return _receivedPackets.TryDequeue(out packet);
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

        private void StartAsClient(string hostIP)
        {
            IsHost = false;
            try
            {
                _hostEndPoint = new IPEndPoint(IPAddress.Parse(hostIP), _PORT);
                _clientSendHostLastSeq = -1;
                _clientReceivedHostLastSeq = -1;

                _udpClient = new UdpClient(0);
                _udpClient.Client.ReceiveBufferSize = _BUFFER_SIZE;

                Log.Info($"Client started on port {_udpClient.Client.LocalEndPoint}");
                Task.Run(() => ReceiveLoop(_cts.Token));
            }
            catch (Exception ex)
            {
                Log.Error($"Client failed to start: {ex.Message}");
            }
        }

        private void StartAsHost()
        {
            IsHost = true;
            try
            {
                _hostEndPoint = new IPEndPoint(IPAddress.Loopback, _PORT);
                _clientEndPoints = new ConcurrentDictionary<IPEndPoint, byte>();

                _clientSendHostLastSeq = -1;
                _clientReceivedHostLastSeq = -1;
                _hostSendClientLastSeq = new ConcurrentDictionary<IPEndPoint, int>();
                _hostReceivedClientLastSeq = new ConcurrentDictionary<IPEndPoint, int>();

                _udpClient = new UdpClient(_PORT);
                _udpClient.Client.ReceiveBufferSize = _BUFFER_SIZE;

                Log.Info($"Host started and listening on port {_PORT}");
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
                byte[] serializedData = Serialize(type, message, GetNextSendSequence(NetworkRole.CLIENT));
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
                byte[] serializedData = Serialize(type, message, GetNextSendSequence(NetworkRole.HOST, target));
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


            foreach (IPEndPoint target in _clientEndPoints.Keys)
            {
                try
                {
                    byte[] serializedData = Serialize(type, message, GetNextSendSequence(NetworkRole.HOST, target));
                    if (serializedData == null) continue;
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
                        if (_clientEndPoints.TryAdd(sender, 0))
                        {
                            Log.Info($"New client connected: {sender}");
                        }
                        _hostSendClientLastSeq.TryAdd(sender, -1);
                        _hostReceivedClientLastSeq.TryAdd(sender, -1);
                    }

                    if (data.Length > 0)
                    {
                        NetworkPacket packet = Deserialize(data);
                        if (packet == null) continue;

                        if (IsHost)
                        {
                            _hostReceivedClientLastSeq.AddOrUpdate(
                                sender,
                                packet.Sequence,
                                (_, seq) =>
                                {
                                    if (seq < packet.Sequence)
                                    {
                                        _receivedPackets.Enqueue(new ReceivedPacket(packet, sender));
                                        return packet.Sequence;
                                    }
                                    return seq;
                                }
                            );
                        }
                        else
                        {
                            if (_clientReceivedHostLastSeq < packet.Sequence)
                            {
                                _receivedPackets.Enqueue(new ReceivedPacket(packet, sender));
                                _clientReceivedHostLastSeq = packet.Sequence;
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

        private byte[] Serialize<T>(PacketType type, T message, int seq) where T : IMessage<T>
        {
            byte[] data = message.ToByteArray();

            NetworkPacket packet = new NetworkPacket()
            {
                Type = type,
                Sequence = seq,
                Data = ByteString.CopyFrom(data)
            };
            byte[] sendBytes = packet.ToByteArray();
            if (sendBytes.Length > _PACKET_SIZE)
            {
                Log.Error($"Packet size ({sendBytes.Length} bytes) exceeds the safety limit ({_PACKET_SIZE} bytes).");
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
        private int GetNextSendSequence(NetworkRole role)
        {
            return role switch
            {
                NetworkRole.CLIENT => System.Threading.Interlocked.Increment(ref _clientSendHostLastSeq),
                _ => throw new ArgumentOutOfRangeException(nameof(role), "Invalid NetworkRole in GetNextSendSequence")
            };
        }
        private int GetNextSendSequence(NetworkRole role, IPEndPoint target)
        {
            return role switch
            {
                NetworkRole.HOST => _hostSendClientLastSeq.AddOrUpdate(target, 0, (_, seq) => seq + 1),
                _ => throw new ArgumentOutOfRangeException(nameof(role), "Invalid NetworkRole in GetNextSendSequence")
            };
        }
    }

    public class ReceivedPacket
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