using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    public Animator screenFadeAnimator;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        screenFadeAnimator.SetBool("StartFading", false);


    }
    // Start is called before the first frame update
    void Start()
    {
        screenFadeAnimator.SetBool("StartFading", false);
    }

    // Update is called once per frame
    void Update()
    {
        
        /*
        if (Input.GetKey(KeyCode.Space))
        {
            if (SceneManager.GetActiveScene().name == "MENU")
            {
                SceneManager.LoadScene("GAMEPLAYSCENE");
            }
        }
        */

        if (SceneManager.GetActiveScene().name == "MENU")
        {
            if (Input.GetKey(KeyCode.Space))
            {
                screenFadeAnimator.SetBool("StartFading", true);
            }

            if (screenFadeAnimator.GetCurrentAnimatorStateInfo(0).IsName("ReadyForMenu") && screenFadeAnimator.GetBool("StartFading") == true)
            {
                SceneManager.LoadScene("GAMEPLAYSCENE");
                screenFadeAnimator.SetBool("StartFading", false);

            }

        }

        if (SceneManager.GetActiveScene().name == "GAMEPLAYSCENE") 
        {
             GameObject player = GameObject.Find("player");
             PlayerController playerController = player.GetComponent<PlayerController>();

            if (playerController.health <= 0) {

                if (screenFadeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Waiting")) {
                    screenFadeAnimator.SetBool("StartFading", true);

                }


                //screenFadeAnimator.SetBool("StartFading", true);

                if (screenFadeAnimator.GetCurrentAnimatorStateInfo(0).IsName("ReadyForMenu") && screenFadeAnimator.GetBool("StartFading") == true)
                {
                    SceneManager.LoadScene("MENU");
                    screenFadeAnimator.SetBool("StartFading", false);

                }

            }
                
                
                
                
                
        }
    }
}
