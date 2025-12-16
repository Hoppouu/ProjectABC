using System;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using Player.Model;

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

        protected Vector3 ToVector3(Vec3 vec3)
        {
            return new Vector3(vec3.X, vec3.Y, vec3.Z);
        }

        //TODO: GC최적화 해야함 -> class 동적 할당 부분
        protected PlayerModel ToPlayerModel(PlayerInfo playerInfo)
        {
            PlayerModel playerModel = new PlayerModel(playerInfo.PlayerID);
            playerModel.SetPlayerTransform(ToVector3(playerInfo.Position), ToVector3(playerInfo.Rotation));
            playerModel.SetPostureState(playerInfo.PlayerPostureState);
            playerModel.SetMovementType(playerInfo.PlayerMovementType);
            return playerModel;
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
            GameManager.Instance.SetPlayers(ToPlayerModel(playerInfo));
        }

        private void HandleJoinRequest(NetworkPacket packet, IPEndPoint sender)
        {
            PlayerInfo playerInfo = PlayerInfo.Parser.ParseFrom(packet.Data);
            int nextID = GameManager.Instance.GetNextPlayerID();
            GameManager.Instance.AddPlayer(new PlayerModel(nextID, false), sender);
            _packetSender.Host.SendJoinResponse(nextID, GameManager.Instance.GetPlayers(), sender);
            _packetSender.Host.SendJoinResponseByBroadcast(GameManager.Instance.GetPlayers());
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

            foreach (PlayerInfo playerInfo in playerInfoList.List)
            {
                GameManager.Instance.SetPlayers(ToPlayerModel(playerInfo));
            }
        }

        private void HandleJoinResponse(NetworkPacket packet, IPEndPoint sender)
        {
            PlayerInfoList playerInfoList = PlayerInfoList.Parser.ParseFrom(packet.Data);
            int yourID = playerInfoList.YourID;
            foreach (PlayerInfo playerInfo in playerInfoList.List)
            {
                if (GameManager.Instance.IsExistPlayer(playerInfo.PlayerID)) continue;
                
                //브로드캐스트일 땐 yourID = 0 이다.
                if (yourID != playerInfo.PlayerID)  GameManager.Instance.AddPlayer(new PlayerModel(playerInfo.PlayerID, false));
                else                                GameManager.Instance.AddPlayer(new PlayerModel(yourID, true));
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