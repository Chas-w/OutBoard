using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D myBody;

    [SerializeField] float moveSpeed;
    [SerializeField] float jumpForce;
    private bool isJumping;
    private float moveHorizontal; 
    private float moveVertical;

    // Start is called before the first frame update
    void Start()
    {
        myBody = gameObject.GetComponent<Rigidbody2D>();

        moveSpeed = 5f;
        jumpForce = 40f;
        isJumping = false; 
    }

    // Update is called once per frame
    void Update()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal"); //we can change this in the Unity settings to controller in the future. 
       /* moveVertical = Input.GetAxisRaw("Vertical");*/ 
    }

    //---movement 
    void FixedUpdate()
    {
        if (moveHorizontal > 0f || moveHorizontal < -0f)
        {
            myBody.AddForce(new Vector2(moveHorizontal * moveSpeed, 0f), ForceMode2D.Impulse); // not using Time.Delta time beacause AddForce has it applied by default. 
        }

       /* if (!isJumping && moveVertical > 0f)
        {
            myBody.AddForce(new Vector2(0f, moveVertical * jumpForce), ForceMode2D.Impulse);  
        }*/

    }

    //---Jumping
   /* private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "floor")
        {
            isJumping = false; 
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        isJumping = true; 
    }*/
}
