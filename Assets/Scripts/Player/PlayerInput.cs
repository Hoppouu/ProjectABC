using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerAction playerAction;
    private PlayerModel playerModel;
    void Start()
    {
        playerAction = GetComponent<PlayerAction>();
        playerModel = GetComponent<PlayerRoot>().Model;
    }

    void Update()
    {
        if (!playerModel.IsMine) return;
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        playerAction.Move(inputX, inputY);
    }
}
