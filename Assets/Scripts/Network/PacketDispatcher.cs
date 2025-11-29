using Network;
using System.Collections;
using UnityEngine;

public class PacketDispatcher : MonoBehaviour
{
    public HostPacketSender HostSender => _packetSender.Host;
    public ClientPacketSender ClientSender => _packetSender.Client;

    public HostPacketHandler HostHandler => _packetHandler.Host;
    public ClientPacketHandler ClientHandler => _packetHandler.Client;

    private PacketTransmitter _packetTransmitter;
    private PacketHandler _packetHandler;
    private PacketSender _packetSender;

    public void Setup(NetworkRole role, string hostIP = "")
    {
        _packetTransmitter = new PacketTransmitter(role, hostIP);
        _packetSender = new PacketSender(_packetTransmitter);
        _packetHandler = new PacketHandler(_packetSender);
    }

    public void TickProcessPacketQueue()
    {
        while (_packetTransmitter.TryDequeuePacket(out ReceivedPacket receivedPacket))
        {
            switch (receivedPacket.SenderType)
            {
                case NetworkRole.HOST:
                    _packetHandler.Client.RoutePacket(receivedPacket.Packet, receivedPacket.Sender);
                    break;
                case NetworkRole.CLIENT:
                    _packetHandler.Host.RoutePacket(receivedPacket.Packet, receivedPacket.Sender);
                    break;
            }
        }
    }
    void Update()
    {
        if (_packetTransmitter == null || _packetHandler == null) return;
        TickProcessPacketQueue();
    }

    private void OnDestroy()
    {
        if(_packetTransmitter != null)
        {
            _packetTransmitter.Dispose();
        }
    }
}
