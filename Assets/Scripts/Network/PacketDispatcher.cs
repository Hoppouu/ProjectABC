using Network;
using UnityEngine;

public class PacketDispatcher : MonoBehaviour
{
    private PacketTransmitter _packetTransmitter;
    public PacketHandler PacketHandler { get; private set; }
    public PacketSender PacketSender { get; private set; }

    public void Setup(NetworkRole role, string hostIP = "")
    {
        _packetTransmitter = new PacketTransmitter(role, hostIP);
        PacketSender = new PacketSender(_packetTransmitter);
        PacketHandler = new PacketHandler(PacketSender);
    }

    public void TickProcessPacketQueue()
    {
        while (_packetTransmitter.TryDequeuePacket(out ReceivedPacket receivedPacket))
        {
            PacketHandler.RoutePacket(receivedPacket.Packet, receivedPacket.Sender);
        }
    }
    void Update()
    {
        if (_packetTransmitter == null || PacketHandler == null) return;
        TickProcessPacketQueue();

    }

    private void OnDestroy()
    {
        _packetTransmitter.Dispose();
    }
}
