using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class PlayerAction : MonoBehaviour
{
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
    }
}
