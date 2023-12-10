using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShroudTriggerer : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController;

    Animator myAnim;


    // Start is called before the first frame update
    void Start()
    {
        myAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerController.health <= 0)
        {
            myAnim.SetBool("StartFading", true);
        } 
    }
}
