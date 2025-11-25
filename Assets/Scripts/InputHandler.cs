using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public event Action<Vector3> OnRunInput;
    public event Action<Vector3> OnWalkInput;
    public event Action<Vector2> OnMouseInput;
    public event Action OnJumpInput;

    [Header("Key Settings")]
    public KeyCode walkKey;

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = new Vector3 (x, 0, y);
        if (Input.GetKey(walkKey))
        {
            OnWalkInput?.Invoke(inputDir);
        }
        else
        {
            OnRunInput?.Invoke(inputDir);
        }
        
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        if (mouseX != 0 || mouseY != 0)
        {
            OnMouseInput?.Invoke(new Vector2(mouseX, mouseY));
        }

        if (Input.GetButtonDown("Jump"))
        {
            OnJumpInput?.Invoke();
        }
    }
}
