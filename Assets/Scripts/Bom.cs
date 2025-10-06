using UnityEngine;

public class Bom : MonoBehaviour
{
    public float deleteTime = 3.0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //
        Destroy(gameObject, deleteTime);
    }

    
}
