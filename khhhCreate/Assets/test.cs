using UnityEngine;
using UnityEngine.InputSystem;


public class test : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            transform.Translate(-3, 0, 0);
        }


    }
}
