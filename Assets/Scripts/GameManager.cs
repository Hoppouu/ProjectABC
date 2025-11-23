using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PacketTransmitter packetTransmitter;
    private PacketSender packetSender;
    void Start()
    {
        packetTransmitter = new PacketTransmitter(NetworkRole.HOST);
        packetSender = new PacketSender(packetTransmitter);
    }

    void Update()
    {
        packetTransmitter.TickProcessPacketQueue();
    }

    void OnDestroy()
    {
        packetTransmitter.Dispose();
    }
}
