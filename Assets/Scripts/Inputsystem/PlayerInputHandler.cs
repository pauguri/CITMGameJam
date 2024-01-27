using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    private bool moving = false;
    private Vector2 movement = Vector2.zero;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void onMove(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            moving = true;
            movement = context.ReadValue<Vector2>();
        }
        if (context.canceled)
        {
            moving = false;
        }
    }

    void Move()
    {
        transform.Translate(movement);
    }

    // Update is called once per frame
    void Update()
    {
        if(moving)
        {
            Move();
        }
    }
}
