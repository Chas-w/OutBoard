using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class hsTable : MonoBehaviour
{
    // references https://www.youtube.com/watch?v=iAbaqGYdnyI&t=49s for code help 
    public TMP_Text hsNumb;
    public TMP_Text csNumb;
    public GameObject timer;

    public static hsTable Instance { get; private set; }

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


    }

    float highestScore = 0;
    float currentScore;
    float finalScore;
    private void Start()
    {
        
    }
    void Update()
    {
        

        if (SceneManager.GetActiveScene().name == "GAMEPLAYSCENE") 
        {
            timer = GameObject.Find("inGameUI");
            Timer score = timer.GetComponent<Timer>();
            currentScore = score.currentPoints;
        }

        if (SceneManager.GetActiveScene().name == "SCOREBOARD")
        {
            finalScore = currentScore;
            if (finalScore > highestScore)
            {
                highestScore = currentScore;
            }

            csNumb.text = currentScore.ToString();
            hsNumb.text = highestScore.ToString();

        }


    }
}
