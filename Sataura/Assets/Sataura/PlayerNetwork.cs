using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine;


public class PlayerNetwork : NetworkBehaviour
{
    private Rigidbody2D rb2D;
    [SerializeField] private float jumpForce = 20;



    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

 

    public void Jump(InputAction.CallbackContext context)
    {
        if(context.performed)
        {
            rb2D.velocity = Vector2.up * jumpForce;
        }      
    }   

    public void Movement(InputAction.CallbackContext context)
    {
        Debug.Log(context.ReadValue<Vector2>());
    }

}
