using System;
using System.Collections.Generic;
using System.Net;
using Network;
public class PacketHandler
{
    public event Action<PlayerInfo> OnMove;
    public event Action<PlayerInfoList> OnMoveSync;
    public event Action<InteractResult> OnInteractionResult;
    public event Action<GameState> OnGameStateChanged;
    public event Action<int> OnPlayerJoin;

    private Dictionary<PacketType, Action<NetworkPacket>> _handlerMap;
    public PacketHandler()
    {
        _handlerMap = new Dictionary<PacketType, Action<NetworkPacket>>();
        RegisterHandler(PacketType.C2HMove, HandleMove);
        RegisterHandler(PacketType.H2CMoveSync, HandleMoveSync);
        RegisterHandler(PacketType.H2CInteractRes, HandleInteractionResult);
        RegisterHandler(PacketType.H2CGameState, HandleGameStateChange);
        RegisterHandler(PacketType.C2HPlayerJoin, HandlePlayerJoin);
    }

    public void RoutePacket(NetworkPacket packet, IPEndPoint sender)
    {
        //[TODO]
        //sender를 알 수 있도록 수정해야함.
        if(_handlerMap.TryGetValue(packet.Type, out Action<NetworkPacket> handler))
        {
            handler.Invoke(packet);
        }
        else
        {
            Log.Error($"Unknown Packet Type: {packet.Type}");
        }
    }

    private void RegisterHandler(PacketType type, Action<NetworkPacket> handler)
    {
        _handlerMap[type] = handler;
    }

    private void HandleMove(NetworkPacket packet)
    {
        PlayerInfo playerInfo = PlayerInfo.Parser.ParseFrom(packet.Data);
        Log.Info($"{playerInfo.Position} {playerInfo.Rotation}");
    }
    private void HandleMoveSync(NetworkPacket packet)
    {
        PlayerInfoList playerInfoList = PlayerInfoList.Parser.ParseFrom(packet.Data);
        OnMoveSync.Invoke(playerInfoList);
        // TODO
        // 1. 패킷 데이터(packet.Data)를 역직렬화하여 위치 정보(PlayerMoveData)로 변환
        // 2. 이 정보를 사용하여 씬에 있는 모든 플레이어의 위치를 보간(Interpolate)하여 업데이트
        // Console.WriteLine($"[UDP Sync] Player Position Sync received. Sequence: {packet.Sequence}");
    }

    private void HandleInteractionResult(NetworkPacket packet)
    {
        //TODO
        // 1. 패킷 데이터를 역직렬화하여 상호작용 결과(InteractionResult)로 변환
        // 2. 등 뒤 공격 성공/실패, 미션 완료 등을 씬에 반영 (이펙트, 상태 변경)
        // Console.WriteLine($"[Reliable] Interaction result processed.");
    }

    private void HandleGameStateChange(NetworkPacket packet)
    {
        //TODO
        // 1. 패킷 데이터를 역직렬화하여 새로운 게임 상태(GameState.Meeting, GameState.Play)로 변환
        // 2. UI 변경 (투표 화면 표시), 플레이어 이동 제한 등 적용
        // Console.WriteLine($"[Reliable] Game State changed.");
        GameState gameStateUpdate = GameState.Parser.ParseFrom(packet.Data);
        OnGameStateChanged.Invoke(gameStateUpdate);
        
    }

    private void HandlePlayerJoin(NetworkPacket packet)
    {
        PlayerInfo playerInfo = PlayerInfo.Parser.ParseFrom(packet.Data);
        OnPlayerJoin.Invoke(playerInfo.PlayerId);
    }
}