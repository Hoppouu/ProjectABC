using UnityEngine;

public class PlayerModel
{
    public Transform headPos;
    public Vector3 playerPosition;
    public Vector3 playerRotation;
    private Vector3 _headPosition;
    private Vector3 _headRotation;

    public int PlayerID { get; private set; }
    public bool IsMine { get; private set; }

    public PlayerModel(int playerID)
    {
        this.PlayerID = playerID;
    }
    public PlayerModel(int playerID, bool isMine)
    {
        PlayerID = playerID;
        IsMine = isMine;
    }
}