using Network;
using System.Net;
using UnityEngine;

public class PacketSender
{
    private PacketTransmitter _packetTransmitter;

    public PacketSender(PacketTransmitter packetTransmitter)
    {
        _packetTransmitter = packetTransmitter;
    }

    public void PlayerMove(int playerId, Vector3 position, Vector3 rotation)
    {
        Network.PlayerInfo playerInfo = new Network.PlayerInfo
        {
            PlayerId = playerId,
            Position = Vector3ToVec3(position),
            Rotation = Vector3ToVec3(rotation)
        };
        _packetTransmitter.SendToHost(PacketType.C2HMove, playerInfo);
    }

    public void PlayerMoveSync(GameObject[] players)
    {
        if (!_packetTransmitter.IsHost) throw new System.InvalidOperationException("PlayerMoveSync should only be called by the host");

        Network.PlayerInfoList playerInfoList = new Network.PlayerInfoList();
        foreach (GameObject player in players)
        {
            Network.PlayerInfo playerList = new Network.PlayerInfo
            {
                PlayerId = 0,
                Position = Vector3ToVec3(player.transform.position),
                Rotation = Vector3ToVec3(player.transform.rotation.eulerAngles)
            };
            playerInfoList.List.Add(playerList);
        }

        _packetTransmitter.SendToHost(PacketType.H2CMoveSync, playerInfoList);
    }
    
    public void PlayerJoin(int playerId)
    {
        Network.PlayerInfo playerInfo = new Network.PlayerInfo
        {
            PlayerId = playerId,
            Position = Vector3ToVec3(Vector3.zero),
            Rotation = Vector3ToVec3(Vector3.zero)
        };
        _packetTransmitter.SendToHost(PacketType.C2HPlayerJoin, playerInfo);
    }

    private Vec3 Vector3ToVec3(Vector3 vector)
    {
        return new Vec3 { X = vector.x, Y = vector.y, Z = vector.z };
    }
}