using UnityEngine;



public class PlayerRoot : MonoBehaviour
{
    public Transform playerTransform;
    public Transform headTransform;

    public PlayerModel Model { get; private set; }

    private float _curTime = 0f;
    private const float _INTERVR_TIME = 0.01f;

    private void LateUpdate()
    {
        _curTime += Time.deltaTime;
        if (_curTime >= _INTERVR_TIME)
        {
            if (Model.IsMine)
            {
                Model.SetPlayerTransform(transform.position, transform.rotation.eulerAngles);
                NetworkManager.Instance.ClientSender.SendPlayerInfo(Model);
            }

            SetModel();
            _curTime = 0f;
        }
    }

    public void Initiate(PlayerModel playerModel)
    {
        Model = playerModel;
        transform.position = playerModel.PlayerPosition;
        transform.rotation = Quaternion.Euler(playerModel.PlayerRotation);
    }

    private void SetModel()
    {
        transform.position = Model.PlayerPosition;
        transform.rotation = Quaternion.Euler(Model.PlayerRotation);
    }
}
