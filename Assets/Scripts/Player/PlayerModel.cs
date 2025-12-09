using UnityEngine;

public class PlayerModel
{
    private Vector3 _playerPosition;
    private Vector3 _playerRotation;
    private Vector3 _headPosition;
    private Vector3 _headRotation;
    public bool isMine { get; private set; }
    public PlayerModel(bool isMine)
    {
        this.isMine = isMine;
    }

    public void SetPosition(Transform playerTransform, Transform headTransform)
    {
        _playerPosition = playerTransform.position;
        _headPosition = headTransform.position;
    }

}
