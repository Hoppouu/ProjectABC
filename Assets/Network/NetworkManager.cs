using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkManager : MonoBehaviour, IDisposable
{
    private const int PORT = 19826;

    //네트워크 단편화 피하기 위해 1400 바이트 이하로 유지.
    private const int BUFFER_SIZE = 1024;

    private UdpClient _udpClient;
    private CancellationTokenSource _cts;

    public bool IsHost { get; private set; }

    private IPEndPoint _hostEndPoint;
    private PacketHandler _packetHandler;

    private void Awake()
    {
        _packetHandler = new PacketHandler();
        _cts = new CancellationTokenSource();
    }

    private void OnDestroy()
    {
        Dispose();
    }

    public void StartAsClient(string hostIP)
    {
        IsHost = false;
        try
        {
            _udpClient = new UdpClient(0);
            _hostEndPoint = new IPEndPoint(IPAddress.Parse(hostIP), PORT);
            Log.Info($"Client started on port {_udpClient.Client.LocalEndPoint}", this);

            Task.Run(() => ReceiveLoop(_cts.Token));
        }
        catch (Exception ex)
        {
            Log.Error($"Client failed to start: {ex.Message}", this);
        }
    }

    public void StartAsHost()
    {
        IsHost = true;
        try
        {
            _udpClient = new UdpClient(PORT);
            Log.Info($"Host started and listening on port {PORT}", this);
            // 호스트는 자기 자신에게도 접속하는 로직을 추가로 실행해야 함 (StartAsClient(localIp)와 유사)
            Task.Run(() => ReceiveLoop(_cts.Token));
        }
        catch (Exception ex)
        {
            Log.Error($"Host failed to start: {ex.Message}", this);
        }
    }

    public void StartAsHostClient()
    {
        IsHost = false;

        string loopbackIP = "127.0.0.1";
        try
        {
            _udpClient = new UdpClient(0);
            _hostEndPoint = new IPEndPoint(IPAddress.Parse(loopbackIP), PORT);
            Log.Info($"HostClient started on port {_udpClient.Client.LocalEndPoint}", this);
            Task.Run(() => ReceiveLoop(_cts.Token));
        }
        catch (Exception ex)
        {
            Log.Error($"HostClient failed to start: {ex.Message}", this);
        }
    }

    private async Task ReceiveLoop(CancellationToken ct)
    {
        while(!ct.IsCancellationRequested)
        {
            try
            {
                UdpReceiveResult result = await _udpClient.ReceiveAsync();
                byte[] data = result.Buffer;
                IPEndPoint sender = result.RemoteEndPoint;

                //[TODO]: data를 NetworkPacket 객체로 변환!! Sequence Number, PacketType 등을 추출하는 로직 필요!
                if (data.Length > 0)
                {
                    NetworkPacket packet = ParseBytesToPacket(data);
                    if (packet != null)
                    {
                        _packetHandler.RoutePacket(packet, sender);
                        // 메인 스레드가 아닌 곳에서 유니티 함수 호출을 위한 처리가 필요.
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                break;
            }
            catch (Exception ex)
            {
                if (!_cts.IsCancellationRequested)
                {
                    Log.Error($"UDP Receive Error: {ex.Message}", this);
                }
            }
        }
    }

    private void SendToHost(byte[] data)
    {
        if (_udpClient == null) return;

        try
        {
            IPEndPoint target = IsHost ? null : _hostEndPoint;

            if(target == null)
            {
                Log.Warning("Send target endpoint is null. Are you trying to broadcast from a client?");
                return;
            }

            _udpClient.Send(data, data.Length, target);
        }
        catch (Exception ex)
        {
            Log.Error($"UDP Send Error: {ex.Message}");
        }
    }

    private void SendByBroadcast(byte[] data)
    {
    }


    public void Dispose()
    {
        if(_cts != null)
        {
            _cts.Cancel();
        }

        if(_udpClient != null )
        {
            _udpClient.Close();
            _udpClient = null;
            Log.Info("disposion complete.", this);
        }
    }

    private NetworkPacket ParseBytesToPacket(byte[] data)
    {
        //[TODO]
        //1. 헤더(PacketType)와 Sequence Number를 byte[]에서 추출.
        //2. 진짜 Data부분 추출
        //3. 역직렬화 로직 구현 (Protobuf/MessagePack 등)
        return new NetworkPacket();
    }
}
