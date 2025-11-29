using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace Network
{
    public class PacketHandlerBase
    {
        private Dictionary<PacketType, Action<NetworkPacket, IPEndPoint>> _handlerMap;
        protected PacketSender _packetSender;

        public PacketHandlerBase(PacketSender packetSender)
        {
            _packetSender = packetSender;
            _handlerMap = new Dictionary<PacketType, Action<NetworkPacket, IPEndPoint>>();
        }

        protected void RegisterHandler(PacketType type, Action<NetworkPacket, IPEndPoint> handler)
        {
            _handlerMap[type] = handler;
        }

        public void RoutePacket(NetworkPacket packet, IPEndPoint sender)
        {
            if (_handlerMap.TryGetValue(packet.PacketType, out Action<NetworkPacket, IPEndPoint> handler))
            {
                handler.Invoke(packet, sender);
            }
            else
            {
                Log.Error($"Unknown Packet Type: {packet.PacketType}");
            }

        }
    }

    public class HostPacketHandler:PacketHandlerBase
    {
        //public event Action<IPEndPoint> OnJoinRequest;
        public HostPacketHandler(PacketSender packetSender) : base(packetSender)
        {
            RegisterHandler(PacketType.PlayerInfo, HandlePlayerInfo);
            RegisterHandler(PacketType.JoinRequest, HandleJoinRequest);
        }

        private void HandlePlayerInfo(NetworkPacket packet, IPEndPoint sender)
        {
            PlayerInfo playerInfo = PlayerInfo.Parser.ParseFrom(packet.Data);
        }

        private void HandleJoinRequest(NetworkPacket packet, IPEndPoint sender)
        {
            PlayerInfo playerInfo = PlayerInfo.Parser.ParseFrom(packet.Data);
            int nextID = GameManager.Instance.GetNextPlayerID();
            GameManager.Instance.AddPlayer(nextID, false, sender);
            //여기에 기존 플레이어 동기화 코드 추가해야함.
            _packetSender.Host.SendJoinResponse(nextID, GameManager.Instance.GetPlayers(), sender);
        }

    }

    public class ClientPacketHandler : PacketHandlerBase
    {

        public ClientPacketHandler(PacketSender packetSender) : base(packetSender)
        {
            RegisterHandler(PacketType.PlayerInfoList, HandlePlayerInfoList);
            RegisterHandler(PacketType.JoinResponse, HandleJoinResponse);
        }



        private void HandlePlayerInfoList(NetworkPacket packet, IPEndPoint sender)
        {
            PlayerInfoList playerInfoList = PlayerInfoList.Parser.ParseFrom(packet.Data);
        }

        private void HandleJoinResponse(NetworkPacket packet, IPEndPoint sender)
        {
            PlayerInfoList playerInfoList = PlayerInfoList.Parser.ParseFrom(packet.Data);
            int yourID = playerInfoList.YourID;

            Log.Info(playerInfoList.List.Count.ToString());
            foreach (PlayerInfo playerInfo in playerInfoList.List)
            {
                if (GameManager.Instance.IsExistPlayer(playerInfo.PlayerID)) continue;

                if (yourID != playerInfo.PlayerID)  GameManager.Instance.AddPlayer(playerInfo.PlayerID, false);
                else                                GameManager.Instance.AddPlayer(yourID, true);
            }
        }
    }

    public class PacketHandler
    {
        public HostPacketHandler Host { get; }
        public ClientPacketHandler Client { get; }

        public PacketHandler(PacketSender packetSender)
        {
            if(packetSender.Host != null)
            {
                Host = new HostPacketHandler(packetSender);
            }
            else
            {
                Host = null;
            }

            Client = new ClientPacketHandler(packetSender);
        }
    }
}