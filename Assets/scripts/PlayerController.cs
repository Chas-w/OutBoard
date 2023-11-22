using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("External Variables")]
    public RoadManager roadManager;
    public float speedUpMultiplier;

    [Header("Internal Move Variables")]
    [SerializeField] float moveSpeed;
    

    [Header("Player Input")]
    // cleanup player INPUT later

    private Rigidbody2D myBody;
    
    float moveHorizontal;
    
    public bool speedUp;


    // Start is called before the first frame update
    void Start()
    {
        myBody = gameObject.GetComponent<Rigidbody2D>();

        moveSpeed = 5f;
    }

    // Update is called once per frame
    void Update()
    {
        moveHorizontal = Input.GetAxisRaw("Horizontal"); //we can change this in the Unity settings to controller in the future. 
       
        if (Input.GetKeyDown(KeyCode.W))
        {
            speedUp = true; 
        } else if (Input.GetKeyUp(KeyCode.W))
        {
            speedUp = false;
        }
    }

    //---movement 
    void FixedUpdate()
    {
        if (moveHorizontal > 0f || moveHorizontal < -0f)
        {
            myBody.AddForce(new Vector2(moveHorizontal * moveSpeed, 0f), ForceMode2D.Impulse); // not using Time.Delta time beacause AddForce has it applied by default. 
        }

        if (speedUp && roadManager.speed <= roadManager.maxSpeed)
        {
            Debug.Log(speedUp);
            roadManager.speed += speedUpMultiplier;
        }  
        if (!speedUp)
        {
            if (roadManager.speed >= roadManager.normSpeed)
            {
                roadManager.speed-= speedUpMultiplier;
            }
        }
    }
}
