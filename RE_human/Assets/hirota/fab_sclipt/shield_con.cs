using UnityEngine;

public class shield_con : MonoBehaviour
{
    //
    float span = 0.01f;

    float delta = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        this.delta += Time.deltaTime;
        if (this.delta > this.span)
        {
            Destroy(gameObject);
        }
    }
}
