using UnityEngine;
using Network;

public class PlayerAction : MonoBehaviour
{
    public int playerID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        GameManager.Instance.packetDispatcher.PacketSender.PlayerMove(0, this.transform.position, this.transform.rotation.eulerAngles);
    }

    public void Move(float x, float y)
    {
        this.transform.Translate(new Vector3(x, 0, y) * Time.deltaTime * 5.0f);
    }
}
