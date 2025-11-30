using UnityEngine;

public class PlayerRoot : MonoBehaviour
{
    public PlayerModel Model { get; private set; }
    public Material[] materials;
    private MeshRenderer _renderer;
    void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    private void LateUpdate()
    {

        if (Model.IsMine)
        {
            Model.playerPosition = transform.position;
            Model.playerRotation = transform.rotation.eulerAngles;
            NetworkManager.Instance.ClientSender.SendPlayerInfo(Model);
        }

        SetModel();
    }

    public void Initiate(PlayerModel playerModel)
    {
        Model = playerModel;
        transform.position = playerModel.playerPosition;
        transform.rotation = Quaternion.Euler(playerModel.playerRotation);
        if(Model.IsMine)    _renderer.material = materials[0];
        else _renderer.material = materials[1];
    }

    private void SetModel()
    {
        transform.position = Model.playerPosition;
        transform.rotation = Quaternion.Euler(Model.playerRotation);
    }
}
