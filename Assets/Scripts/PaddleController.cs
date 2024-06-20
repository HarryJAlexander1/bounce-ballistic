using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaddleController : MonoBehaviour
{
    [SerializeField] private float MouseSensitivity;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked; // make cursor invisible and locked on screen
    }

    void Update()
    {
        HandlePaddleMovement();
    }

    private void HandlePaddleMovement() 
    {
        // get mouse input
        var mouseX = Input.GetAxis("Mouse X") * MouseSensitivity * Time.deltaTime;

        if (transform.position.x >= 10.0f)
        {
            transform.Translate(Vector2.left * 0.1f);
            return;
        }
        else if (transform.position.x <= -10.0f) 
        {
            transform.Translate(Vector2.right * 0.1f);
            return;
        }
        transform.Translate(Vector2.right * mouseX);
    }
}
