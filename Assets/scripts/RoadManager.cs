using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public Camera cam;

    float xAspect = 16;
    float yAspect = 9;

    float camHeight = 2f * 5;

    float camWidth;


    public class Segment
    {
        Vector3 p1;
        Vector3 p2;

        Color segmentColor;


        public Segment(Vector3 point1, Vector3 point2, Color thisColor) {
            p1 = point1;
            p2 = point2;
            segmentColor = thisColor;
        
        }
    }

    public float roadWidth = 10;

    public float roadEnd = 8;
    public float roadStart = 1;

    public float cameraElevation = 5;

    public float FOV = 1;

    public float slightUpDownRotation;




    public Segment[] segments;



    // Start is called before the first frame update
    void Start()
    {
        camWidth = xAspect / yAspect * camHeight;
    }

    // Update is called once per frame
    void Update()
    {

        RenderRoad();
    }


    void ResetRoad() 
    {
        //for (int i = 0; i < 500; i++)
        //{
            //segments.Add(new Segment());
        //}
        
    }

    void RenderRoad() {
        Debug.DrawLine( WorldToScreen(new Vector3(roadWidth/2,0,roadStart)), WorldToScreen(new Vector3(roadWidth/2, 0, roadEnd)));
        //Debug.Log(WorldToScreen(new Vector3(roadWidth/2, 0, roadStart)).x);


        Debug.DrawLine(WorldToScreen(new Vector3(-roadWidth / 2, 0, roadStart)), WorldToScreen(new Vector3(-roadWidth / 2, 0, roadEnd)));
        //Debug.Log(WorldToScreen(new Vector3(roadWidth / 2, 0, roadStart)).x);

    }

    Vector3 WorldToScreen(Vector3 worldPos) {
        float xCam = worldPos.x - 0;
        float yCam = worldPos.y - cameraElevation;
        float zCam = worldPos.z - 0;

        float xProj = xCam * (1/Mathf.Tan(FOV/2) ) / zCam;
        float yProj = yCam * (1/Mathf.Tan(FOV/2) ) / zCam;

        float xScreen = xProj;
        float yScreen = yProj+slightUpDownRotation;

        return new Vector3(xScreen,yScreen,0);

    }

}
