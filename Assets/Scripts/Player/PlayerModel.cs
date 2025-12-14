using UnityEngine;
using Network;
public class PlayerModel
{
    public Vector3 PlayerPosition { get; private set; }
    public Vector3 PlayerRotation { get; private set; }
    public Vector3 HeadPosition { get; private set; }
    public Vector3 HeadRotation { get; private set; }

    public int PlayerID { get; private set; }
    public bool IsMine { get; private set; }

    public float moveSpeed { get; private set; }
    public bool isCrawl { get; private set; }

    public PlayerPostureState PlayerPostureState { get; private set; }
    public PlayerMovementType PlayerMovementType { get; private set; }

    public PlayerModel(int playerID)
    {
        PlayerID = playerID;
    }

    public PlayerModel(bool isMine)
    {
        IsMine = isMine;
    }

    public PlayerModel(int playerID, bool isMine)
    {
        PlayerID = playerID;
        IsMine = isMine;
    }

    public void SetPlayerTransform(Vector3 playerPosition, Vector3 playerRotation)
    {
        PlayerPosition = playerPosition;
        PlayerRotation = playerRotation;
    }

    public void SetPlayerState(PlayerPostureState playerPostureState)
    {
        PlayerPostureState = playerPostureState;
    }

    public void SetPlayerState(PlayerMovementType playerMovementType)
    {
        PlayerMovementType = playerMovementType;
    }
}