using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;

public class PacketHandler
{
    public delegate void  PacketProcessor(NetworkPacket packet);

    private Dictionary<PacketType, Action<NetworkPacket>> _handlerMap;
    public PacketHandler()
    {
        _handlerMap = new Dictionary<PacketType, Action<NetworkPacket>>();
        RegisterHandler(PacketType.H2C_MOVE_SYNC, HandleMoveSync);
        RegisterHandler(PacketType.H2C_INTERACT_RES, HandleInteractionResult);
        RegisterHandler(PacketType.H2C_GAME_STATE, HandleGameStateChange);
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

    private void HandleMoveSync(NetworkPacket packet)
    {
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
    }
}

/// <summary>
/// 네트워크 패킷 타입
/// </summary>
public enum PacketType : byte
{
    /// <summary>
    /// Transform 정보 전달
    /// </summary>
    C2H_MOVE = 1,

    /// <summary>
    /// 모든 플레이어 Transform 정보 동기화
    /// </summary>
    H2C_MOVE_SYNC = 2,

    /// <summary>
    /// 상호작용 시도 요청
    /// </summary>
    C2H_INTERACT_REQ = 16,

    /// <summary>
    /// 상호작용 처리 결과
    /// </summary>
    H2C_INTERACT_RES = 17,

    /// <summary>
    /// 게임 상태 변경
    /// </summary>
    H2C_GAME_STATE = 18
}

public class NetworkPacket
{
    public PacketType Type { get; set; }
    public byte[] Data { get; set; }
    public ushort Sequence { get; set; }
}

