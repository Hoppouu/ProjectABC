using System;
using System.Collections.Generic;
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
            if (_handlerMap.TryGetValue(packet.Type, out Action<NetworkPacket, IPEndPoint> handler))
            {
                handler.Invoke(packet, sender);
            }
            else
            {
                Log.Error($"Unknown Packet Type: {packet.Type}");
            }

        }
    }

    public class HostPacketHandler:PacketHandlerBase
    {
        public event Action<IPEndPoint> OnJoinRequest;
        public HostPacketHandler(PacketSender packetSender) : base(packetSender)
        {
            RegisterHandler(PacketType.C2HMove, HandleMove);
            RegisterHandler(PacketType.C2HPlayerJoin, HandlePlayerJoin);
            RegisterHandler(PacketType.C2HJoinRequest, HandleJoinRequest);
        }

        private void HandleMove(NetworkPacket packet, IPEndPoint sender)
        {
            PlayerInfo playerInfo = PlayerInfo.Parser.ParseFrom(packet.Data);
        }

        private void HandlePlayerJoin(NetworkPacket packet, IPEndPoint sender)
        {
            PlayerInfoList playerInfoList = PlayerInfoList.Parser.ParseFrom(packet.Data);

        }

        private void HandleJoinRequest(NetworkPacket packet, IPEndPoint sender)
        {
            PlayerInfo playerInfo = PlayerInfo.Parser.ParseFrom(packet.Data);
            int nextID = GameManager.Instance.GetNextPlayerID();
            GameManager.Instance.AddPlayer(nextID, false, sender);
            _packetSender.Host.SendJoinResponse(nextID, GameManager.Instance.GetPlayers(), sender);
        }

    }

    public class ClientPacketHandler : PacketHandlerBase
    {

        public event Action<PlayerInfo> OnMove;
        public event Action<PlayerInfoList> OnMoveSync;
        public event Action<InteractResult> OnInteractionResult;
        public event Action<GameState> OnGameStateChanged;
        public event Action<int> OnJoinResponse;

        public ClientPacketHandler(PacketSender packetSender) : base(packetSender)
        {
            RegisterHandler(PacketType.H2CMoveSync, HandleMoveSync);
            RegisterHandler(PacketType.H2CPlayerJoinSync, HandlePlayerJoin);
            RegisterHandler(PacketType.H2CJoinResponse, HandleJoinResponse);
        }



        private void HandleMoveSync(NetworkPacket packet, IPEndPoint sender)
        {
            PlayerInfoList playerInfoList = PlayerInfoList.Parser.ParseFrom(packet.Data);
            OnMoveSync.Invoke(playerInfoList);
        }

        private void HandlePlayerJoin(NetworkPacket packet, IPEndPoint sender)
        {
            PlayerInfoList playerInfoList = PlayerInfoList.Parser.ParseFrom(packet.Data);
            //GameManager.Instance.CreatePlayerPrefab(false);
        }
        private void HandleJoinResponse(NetworkPacket packet, IPEndPoint sender)
        {
            PlayerInfo playerInfo = PlayerInfo.Parser.ParseFrom(packet.Data);
            OnJoinResponse.Invoke(playerInfo.PlayerId);
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