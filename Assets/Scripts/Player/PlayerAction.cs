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

    void Update()
    {
        
    }
    public void Move(float x, float y)
    {
        this.transform.Translate(new Vector3(x, 0, y) * Time.deltaTime * 5.0f);
    }

    public void Attack()
    {

    }

    public void Use()
    {

    }
}
