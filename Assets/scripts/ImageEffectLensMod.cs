using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class ImageEffectLensMod : MonoBehaviour
{
    [SerializeField] Material mat;
    [SerializeField] PlayerController playerController;

    //shader variable 
    string d = "_distortion";

    float maxDistort = -0.37f;
    float minDistort = -0.25f;
    float currentDistort;

    void Start()
    {
        //get material componenet
        mat = GetComponent<ImageEffectLensMod>().mat;
        //return error if no property
        if (!mat.HasProperty(d))
        {
            Debug.LogError("the shader associated with the material on this game object is missing a necessary property. _distortion is required");
        }

        currentDistort = -0.25f;
    }
    void Update()
    {
        //if speeding up
        if (playerController.speedUp == true)
        {

            if (currentDistort <= maxDistort)
            {
                currentDistort = maxDistort;
            }
            else
            {
                currentDistort -= 0.005f;
            }

        } 
        else
        {
            //reset to default values if !speedUp
            //speed 
            if (currentDistort < minDistort)
            {
                currentDistort += 0.005f;
            }
            else if (currentDistort <= minDistort)
            {
                currentDistort = minDistort;
            }
        }

        //set distortion based on speed
        mat.SetFloat(d, currentDistort);
    }
}
