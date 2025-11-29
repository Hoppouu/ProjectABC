using System.Net;
using UnityEngine;
using Manager;
using System.Collections.Generic;

namespace Network
{
    public class PacketSenderBase
    {
        protected readonly PacketTransmitter _packetTransmitter;

        public PacketSenderBase(PacketTransmitter packetTransmitter)
        {
            _packetTransmitter = packetTransmitter;
        }
        protected Vec3 ToVec3(Vector3 vector)
        {
            return new Vec3 { X = vector.x, Y = vector.y, Z = vector.z };
        }
        protected PlayerInfo ToPlayerInfo(PlayerEntry playerEntry)
        {
            Network.PlayerInfo playerInfo = new Network.PlayerInfo()
            {
                PlayerID = playerEntry.id,
                Position = ToVec3(playerEntry.gameObject.transform.position),
                Rotation = ToVec3(playerEntry.gameObject.transform.rotation.eulerAngles)
            };

            return playerInfo;
        }

        protected PlayerInfoList ToPlayerInfoList(int playerID, List<PlayerEntry> playerEntries)
        {
            Network.PlayerInfoList playerInfoList = new PlayerInfoList();

            playerInfoList.YourID = playerID;
            foreach (PlayerEntry playerEntry in playerEntries)
            {
                playerInfoList.List.Add(ToPlayerInfo(playerEntry));
            }

            return playerInfoList;
        }
    }

    public class HostPacketSender : PacketSenderBase
    {
        public HostPacketSender(PacketTransmitter packetTransmitter) : base(packetTransmitter) { }

        public void BroadcastPlayersInfo(List<PlayerEntry> players)
        {
            if (!_packetTransmitter.IsHost) throw new System.InvalidOperationException("PlayerMoveSync should only be called by the host");

            _packetTransmitter.SendToClientByBroadcast(PacketType.PlayerInfoList, ToPlayerInfoList(0, players));
        }

        public void SendJoinResponse(int playerID, List<PlayerEntry> palyers, IPEndPoint target)
        {
            _packetTransmitter.SendToClientByBroadcast(PacketType.JoinResponse, ToPlayerInfoList(playerID, palyers));
        }
    }

    public class ClientPacketSender : PacketSenderBase
    {
        public ClientPacketSender(PacketTransmitter packetTransmitter) : base(packetTransmitter) { }

        public void SendPlayerInfo(PlayerEntry playerEntry)
        {
            _packetTransmitter.SendToHost(PacketType.PlayerInfo, ToPlayerInfo(playerEntry));
        }
        public void SendJoinRequest()
        {
            _packetTransmitter.SendToHost(PacketType.JoinRequest, new Empty());
        }
    }


    public class PacketSender
    {
        public HostPacketSender Host { get; }
        public ClientPacketSender Client { get; }
        public PacketSender(PacketTransmitter packetTransmitter)
        {
            if (packetTransmitter.IsHost)
            {
                Host = new HostPacketSender(packetTransmitter);
                Client = new ClientPacketSender(packetTransmitter);
            }
            else
            {
                Host = null;
                Client = new ClientPacketSender(packetTransmitter);
            }
        }
    }
}