using Network;
using UnityEngine;
using UnityEngine.UIElements;

public class PacketSender
{
    private PacketTransmitter _packetTransmitter;

    public PacketSender(PacketTransmitter packetTransmitter)
    {
        _packetTransmitter = packetTransmitter;
    }

    public void PlayerMove(int playerId, Vector3 position, Vector3 rotation)
    {
        Network.PlayerMove playerMove = new Network.PlayerMove
        {
            PlayerId = playerId,
            Position = Vector3ToVec3(position),
            Rotation = Vector3ToVec3(rotation)
        };
        _packetTransmitter.SendPacket(PacketType.C2HMove, playerMove);
    }

    public void PlayerMoveSync(GameObject[] players)
    {

        if (!_packetTransmitter.IsHost) throw new System.InvalidOperationException("PlayerMoveSync should only be called by the host");

        Network.PlayerMoveSync playerMoveSync = new Network.PlayerMoveSync();
        foreach (GameObject player in players)
        {
            Network.PlayerMove playerMove = new Network.PlayerMove
            {
                PlayerId = 0,
                Position = Vector3ToVec3(player.transform.position),
                Rotation = Vector3ToVec3(player.transform.rotation.eulerAngles)
            };
            playerMoveSync.PlayerMoveArr.Add(playerMove);
        }

        _packetTransmitter.SendPacket(PacketType.H2CMoveSync, playerMoveSync);
    }

    private Vec3 Vector3ToVec3(Vector3 vector)
    {
        return new Vec3 { X = vector.x, Y = vector.y, Z = vector.z };
    }
}