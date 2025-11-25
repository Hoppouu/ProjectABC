using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager inst;
    public InputHandler inputHandler;

    private void Awake()
    {
        if (inst != null && inst != this)
        {
            Destroy(gameObject);
            return;
        }
        inst = this;

    }

    void Update()
    {
        
    }
}
