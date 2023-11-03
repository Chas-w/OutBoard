using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public Camera cam;


    public class Segment
    {
        float p1;
        float p2;

        Color segmentColor;


        public Segment(int point1, int point2, Color thisColor) {
            p1 = point1;
            p2 = point2;
            segmentColor = thisColor;
        
        }
    }

    float roadWidth = 10;




    public Segment[] segments;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void ResetRoad() 
    {
        //for (int i = 0; i < 500; i++)
        //{
            //segments.Add(new Segment());
        //}
        
    }

    void RenderRoad() { 
        

    }

}
