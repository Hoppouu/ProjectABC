using UnityEngine;
using Player.Model;
using System;


public class PlayerRoot : MonoBehaviour
{
    public Transform playerTransform;
    public Transform headTransform;

    public event Action OnPlayerInfoUpdate;
    public PlayerModel Model { get; private set; }

    private float _curTime = 0f;
    private const float _INTERVR_TIME = 1/60f;

    private void LateUpdate()
    {
        SendPlayerInfoPacket();
    }

    private void SendPlayerInfoPacket()
    {
        _curTime += Time.deltaTime;
        if (_curTime >= _INTERVR_TIME)
        {
            if (Model.isMine)
            {
                NetworkManager.Instance.ClientSender.SendPlayerInfo(Model);
            }
            else
            {
                OnPlayerInfoUpdate?.Invoke();
            }
            _curTime = 0f;
        }
    }

    public void Initiate(PlayerModel playerModel)
    {
        Model = playerModel;
        transform.position = playerModel.Data.playerPosition;
        transform.rotation = Quaternion.Euler(playerModel.Data.playerRotation);
    }
}
