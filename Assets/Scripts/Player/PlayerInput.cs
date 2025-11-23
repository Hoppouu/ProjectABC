using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerAction playerAction;
    void Start()
    {
        playerAction = GetComponent<PlayerAction>();
    }

    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        playerAction.Move(inputX, inputY);
    }
}
