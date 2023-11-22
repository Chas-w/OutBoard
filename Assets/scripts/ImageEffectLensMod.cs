using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode, ImageEffectAllowedInSceneView]

public class ImageEffectLensMod : MonoBehaviour
{
    public Material effectMaterial;
    public Timer timer;


    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        //take tex and copy from frame buffer to destination (frame buffer with effectMaterial applied)
        Graphics.Blit(source, destination, effectMaterial);
    }

}
