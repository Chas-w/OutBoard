using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("External Variables")]
    public RoadManager roadManager;
    public float speedUpMultiplier;
    public float centrifugalForceMultiplier = 0.3f;


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
        Vector2 netHorizontalForce;
        netHorizontalForce = Vector2.zero;
        


        if (moveHorizontal > 0f || moveHorizontal < -0f)
        {
            //myBody.AddForce(new Vector2(moveHorizontal * moveSpeed, 0f), ForceMode2D.Impulse); // not using Time.Delta time beacause AddForce has it applied by default. 
            netHorizontalForce += new Vector2(moveHorizontal * moveSpeed, 0f);
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


        //This is where we apply centrifugal force, when the player is going around bends.


        float ZPos = roadManager.ZPos;
        if (roadManager.FindSegment(ZPos).curviness != 0)
        {
            //myBody.AddForce(new Vector2(-roadManager.FindSegment(ZPos).curviness * centrifugalForceMultiplier, 0f), ForceMode2D.Impulse);
            netHorizontalForce += new Vector2(-roadManager.FindSegment(ZPos).curviness * Mathf.Pow(centrifugalForceMultiplier,2) * roadManager.speed, 0f);
            Debug.Log(-roadManager.FindSegment(ZPos).curviness);
        }

        myBody.AddForce(netHorizontalForce, ForceMode2D.Impulse);

    }
}
