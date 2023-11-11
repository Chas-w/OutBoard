using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RoadManager : MonoBehaviour
{
    public Sprite defaultSegmentSprite;

    public Camera cam;

    float xAspect = 16;
    float yAspect = 9;

    float camHeight = 2f * 5;

    float camWidth;

    public float ZPos = 0;

    public float speed = .5f;

    public float drawDistance = 12; //This is distance in segments. If DrawDistance is 12, it will render 12 road segments ahead of the player. 

    public float segmentLength = 2;

    [SerializeField]
    float trackLength;




    public class Segment
    {
        public Vector3 p1;
        public Vector3 p2;

        public Color segmentColor;

        public Sprite segmentSprite;

        public int index;

        public float curviness;


        public Segment(Vector3 point1, Vector3 point2, Color thisColor, Sprite thisSegmentSprite, float thisSegmentCurviness) {
            p1 = point1;
            p2 = point2;
            segmentColor = thisColor;
            segmentSprite = thisSegmentSprite;
            curviness = thisSegmentCurviness;

        }
    }

    public float roadWidth = 10;

    float roadEnd = 1000;
    float roadStart = 1;

    public float cameraElevation = 5;

    public float cameraHorizontalOffset = 0;

    public float FOV = 1;

    public float slightUpDownRotation;


    public GameObject renderedSegmentHolder;

    public GameObject renderedSegmentPrefab;

    public List<GameObject> renderedSegmentsList = new List<GameObject>();




    public Segment[] segments = new Segment[500];



    // Start is called before the first frame update
    void Start()
    {
        camWidth = xAspect / yAspect * camHeight;

        ResetRoad();

        AddCurveAt(5, 12, 4, 6, 4);
    }

    // Update is called once per frame
    void Update()
    {
        ZPos = ZPos + speed * Time.deltaTime;

        //Debug.Log(Mathf.Floor(ZPos / segmentLength) % segments.Length);

        RenderRoad();
    }


    void ResetRoad() 
    {
        for (int i = 1; i < 501; i++)
        {
            //Debug.Log(new Segment(new Vector3(0, 0, i * segmentLength), new Vector3(0, 0, (i + 1) * segmentLength), Color.white, defaultSegmentSprite));
            segments[i-1] = new Segment(new Vector3(0,0,i* segmentLength), new Vector3(0,0,(i+1)*segmentLength),Color.white,defaultSegmentSprite, 0);
            segments[i - 1].index = i - 1;
        
        }

        trackLength = segments.Length * segmentLength;

        Debug.Log(trackLength);

        //Now we're going to make [DrawDistance] amount of Mesh Renderers
        for (int i = 0; i < drawDistance-1; i++)
        {
            GameObject newRenderedSegment = Instantiate(renderedSegmentPrefab, renderedSegmentHolder.transform);

            renderedSegmentsList.Add(newRenderedSegment);

        }

        

    }

    void RenderRoad() {

        Segment baseSegment = FindSegment(ZPos);
        float basePercent = 1- ((ZPos % segmentLength) / segmentLength);

        float dx = +(baseSegment.curviness * basePercent);
        float x = 0;


        //Debug.DrawLine( WorldToScreen(new Vector3(roadWidth/2,0,baseSegment.p1.z)), WorldToScreen(new Vector3(roadWidth/2, 0, roadEnd)));
        //Debug.Log(WorldToScreen(new Vector3(roadWidth/2, 0, roadStart)).x);


        //Debug.DrawLine(WorldToScreen(new Vector3(-roadWidth / 2, 0, baseSegment.p1.z)), WorldToScreen(new Vector3(-roadWidth / 2, 0, roadEnd)));
        //Debug.Log(WorldToScreen(new Vector3(roadWidth / 2, 0, roadStart)).x);


        for (int i = baseSegment.index+1; i < baseSegment.index + drawDistance; i++)
        {
            Segment currentSegment = segments[i % segments.Length];


            //Drawing little dots for debug purposes
            //Debug.DrawLine(WorldToScreen(currentSegment.p1,0-x), WorldToScreen(currentSegment.p1 + new Vector3(0, 0,.1f),0-x));
            //Debug.DrawLine(WorldToScreen(currentSegment.p2, 0-x-dx), WorldToScreen(currentSegment.p2 + new Vector3(0, 0,.1f),0-x-dx), Color.red);
            
            //Draw left and right segment bounds

            //Debug.DrawLine(WorldToScreen(currentSegment.p1 + new Vector3(roadWidth / 2, 0, 0), -x), WorldToScreen(currentSegment.p2 + new Vector3(roadWidth / 2, 0, 0), -x-dx));

            Vector3 rightUpperCornerScreenSpace = WorldToScreen(currentSegment.p1 + new Vector3(roadWidth / 2, 0, 0), -x);
            Vector3 rightLowerCornerScreenSpace = WorldToScreen(currentSegment.p2 + new Vector3(roadWidth / 2, 0, 0), -x - dx);

            Debug.DrawLine( rightUpperCornerScreenSpace, rightLowerCornerScreenSpace);


            //Debug.DrawLine(WorldToScreen(currentSegment.p1 + new Vector3(-roadWidth / 2, 0, 0), -x), WorldToScreen(currentSegment.p2 + new Vector3(-roadWidth / 2, 0, 0), -x-dx));

            Vector3 leftUpperCornerScreenSpace = WorldToScreen(currentSegment.p1 + new Vector3(-roadWidth / 2, 0, 0), -x);
            Vector3 leftLowerCornerScreenSpace = WorldToScreen(currentSegment.p2 + new Vector3(-roadWidth / 2, 0, 0), -x - dx);

            Debug.DrawLine(leftUpperCornerScreenSpace, leftLowerCornerScreenSpace);


            //Now for the tricky part--making the road segment into a mesh!

            int segmentMeshIndex = (int)( (currentSegment.index) % (drawDistance-1));
            Debug.Log(segmentMeshIndex.ToString());
            GameObject segmentMeshObject = renderedSegmentsList[segmentMeshIndex];
            MeshFilter segmentFilter = segmentMeshObject.GetComponent<MeshFilter>();
            Mesh segmentMesh = segmentFilter.mesh;

            //segmentMesh.Clear();

            //segmentMeshObject.transform.position = WorldToScreen(currentSegment.p1 + currentSegment.p2/2, -x +dx/2);

            List<Vector3> vertices = new List<Vector3>();

            vertices.Add(leftLowerCornerScreenSpace);
            vertices.Add(rightLowerCornerScreenSpace);
            vertices.Add(leftUpperCornerScreenSpace);
            vertices.Add(rightUpperCornerScreenSpace);

            Vector2[] uv = new Vector2[4]
            {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1)
            };


            segmentMesh.uv = uv;
            segmentMesh.vertices = vertices.ToArray();
            segmentMesh.triangles = new int[] { 0, 2, 1, 2,3,1};








            //Little criss-cross pattern for decoration and more visibility
            //Debug.DrawLine(WorldToScreen(currentSegment.p1 + new Vector3(-roadWidth / 2, 0, 0), -x), WorldToScreen(currentSegment.p2 + new Vector3(roadWidth / 2, 0, 0), -x-dx));
            //Debug.DrawLine(WorldToScreen(currentSegment.p1 + new Vector3(roadWidth / 2, 0, 0), -x), WorldToScreen(currentSegment.p2 + new Vector3(-roadWidth / 2, 0, 0), -x-dx));


            //Lines where segments meet
            Debug.DrawLine(leftLowerCornerScreenSpace, rightLowerCornerScreenSpace);
            Debug.DrawLine(leftUpperCornerScreenSpace, rightUpperCornerScreenSpace);

           
            x = x + dx;
            dx = dx + currentSegment.curviness;



        }

    }

    Segment FindSegment(float currentZPosition) {

        float segmentIndex = (Mathf.Floor(currentZPosition / segmentLength) % segments.Length);

        return segments[(int)segmentIndex];
    }

    Vector3 WorldToScreen(Vector3 worldPos, float cameraXOffset) {
        float xCam = worldPos.x - cameraHorizontalOffset - cameraXOffset;
        float yCam = worldPos.y - cameraElevation;
        float zCam = worldPos.z - ZPos;

        float xProj = xCam * (1/Mathf.Tan(FOV/2) ) / zCam;
        float yProj = yCam * (1/Mathf.Tan(FOV/2) ) / zCam;

        float xScreen = xProj;
        float yScreen = yProj+slightUpDownRotation;

        return new Vector3(xScreen,yScreen,0);

    }

    void AddCurveAt( int segmentCurveStartsAt ,int segmentsInCurve, int curveEntrySegments, int curveExitSegments, float overallCurviness) {

        for (int i = 0; i < curveEntrySegments; i++)
        {
            Segment segmentToCurve = segments[segmentCurveStartsAt + i];

            segmentToCurve.curviness = EaseIn(0, overallCurviness, i / curveEntrySegments);
        }

        for (int i = 0; i < segmentsInCurve-curveExitSegments; i++)
        {
            Segment segmentToCurve = segments[segmentCurveStartsAt + curveEntrySegments + i];

            segmentToCurve.curviness = overallCurviness;
        }

        for (int i = 0; i <curveExitSegments; i++)
        {
            Segment segmentToCurve = segments[segmentCurveStartsAt + segmentsInCurve - curveExitSegments + i];

            segmentToCurve.curviness = EaseInOut(overallCurviness,0, i / curveExitSegments);
        }




        //for (int i = 1; i < segmentsInCurve; i++)
        //{
        //Segment segmentToCurve = segments[segmentCurveStartsAt + curveEntrySegments + i];


        //}
    }


    float EaseIn(float a, float b, float alpha) {
        return a + (b - a) * Mathf.Pow(alpha, 2);
    }
    
    float EaseOut(float a, float b, float alpha) {
        return a + (b - a) * (1 - Mathf.Pow(1 - alpha, 2));
    }

    float EaseInOut(float a, float b, float alpha)
    {
        return a + (b - a) * ((-Mathf.Cos(alpha*Mathf.PI)/2)+0.5f);
    }

}
