using UnityEngine;

public class PlayerRoot : MonoBehaviour
{
    public Transform playerTransform;
    public Transform headTransform;

    public PlayerModel Model { get; private set; }
    public void Initialize(int playerID)
    {
        Model = GameManager.Instance.GetPlayer(playerID).playerModel;
        Set();
    }

    public void Awake()
    {
#if UNITY_EDITOR
        Model = new PlayerModel(true);
        Set();
#endif
    }

    public void LateUpdate()
    {
        Set();
    }

    private void Set()
    {
        Model.SetPosition(playerTransform, headTransform);
//        Model.playerRotation = playerTransform.rotation.eulerAngles;
//        Model.headRotation = headTransform.rotation.eulerAngles;
    }

}