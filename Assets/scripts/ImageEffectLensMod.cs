using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class ImageEffectLensMod : MonoBehaviour
{
    [SerializeField] Material lensMat;
    [SerializeField] Material vMat;
    [SerializeField] PlayerController playerController;

    //lens shader variable 
    string d = "_distortion";
    string r = "_vr";
    string ca = "_intensity";

    float maxDistort = -0.37f;
    float minDistort = -0.25f;
    float currentDistort;

    //vignette shader variable
    float maxRadius = 0.855f;
    float minRadius = 0.949f;
    float currentRadius;

    //chromatic aberration variables
    float maxIntensity = 0.5f;
    float minIntensity = 0.01f;
    float currentIntensity;
    void Start()
    {
        //get material componenet
        lensMat = GetComponent<ImageEffectLensMod>().lensMat;
        //return error if no property
        if (!lensMat.HasProperty(d))
        {
            Debug.LogError("the shader associated with the material on this game object is missing a necessary property. _distortion is required");
        }

        if (!vMat.HasProperty(r))
        {
            Debug.LogError("the shader associated with the material on this game object is missing a necessary property. _distortion is required");
        }

        currentDistort = -0.25f;
        currentRadius = 0.809f;
    }
    void Update()
    {
        //if speeding up
        if (playerController.speedUp == true)
        {

            //lens
            if (currentDistort <= maxDistort)
            {
                currentDistort = maxDistort;
            }
            else
            {
                currentDistort -= 0.005f;
            }

            //vignette
            if (currentRadius <= maxRadius)
            {
                currentRadius = maxRadius;
            }
            else
            {
                currentRadius -= 0.005f;
            }

            //chromatic ab.
            if (currentIntensity <= maxIntensity)
            {
                currentIntensity = maxIntensity;
            }
            else
            {
                currentIntensity += 0.005f;
            }

        } 
        else
        {
            //reset to default values if !speedUp
  
            //lens
            if (currentDistort < minDistort)
            {
                currentDistort += 0.005f;
            }
            else if (currentDistort >= minDistort)
            {
                currentDistort = minDistort;
            }

            //vignette
            if (currentRadius < minRadius)
            {
                currentRadius += 0.005f;
            }
            else if (currentRadius >= minRadius)
            {
                currentRadius = minRadius;
            }

            //Chromatic ab
            if (currentIntensity < minIntensity)
            {
                currentIntensity -= 0.005f;
            }
            else if (currentIntensity >= minIntensity)
            {
                currentIntensity = minIntensity;
            }
        }

        //set distortion based on speed
        lensMat.SetFloat(d, currentDistort);
        vMat.SetFloat(r, currentRadius);

    }
}
