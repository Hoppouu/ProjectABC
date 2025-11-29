using UnityEngine;
using Network;

public class PlayerAction : MonoBehaviour
{
    public int playerID;
    PlayerModel _playerModel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
    public void Move(float x, float y)
    {
        this.transform.Translate(new Vector3(x, 0, y) * Time.deltaTime * 5.0f);
        NetworkManager.Instance.packetDispatcher.PacketSender.PlayerMove(playerID, this.transform.position, this.transform.rotation.eulerAngles);
    }

    public void Attack()
    {

    }

    public void Use()
    {

    }
}
