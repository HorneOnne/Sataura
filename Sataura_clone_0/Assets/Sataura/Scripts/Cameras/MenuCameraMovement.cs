using UnityEngine;

public class MenuCameraMovement : MonoBehaviour
{
    [SerializeField] private float cameraMoveSpeed = 5f;
    private bool moveRight = true;


    // Update is called once per frame
    void Update()
    {
        

        if(transform.position.x <= -10000)
        {
            moveRight = true;
        }
        

        if (transform.position.x >= 10000)
        {
            moveRight = false; 
        }

        if(moveRight)
        {
            transform.position += Vector3.right * cameraMoveSpeed * Time.deltaTime;
        }
        else
        {
            transform.position += Vector3.left * cameraMoveSpeed * Time.deltaTime;
        }
    }
}
