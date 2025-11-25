using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private PlayerAction playerAction;
    [HideInInspector] public bool isMine;
    void Start()
    {
        playerAction = GetComponent<PlayerAction>();
    }

    void Update()
    {
        if (!isMine) return;
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        playerAction.Move(inputX, inputY);
    }
}
