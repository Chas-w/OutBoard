using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class RoadManager : MonoBehaviour
{
    [Header("External Variables")]
    public Sprite defaultSegmentSprite;

    public Camera cam;


    public float trackLength;

    [Header("Speed Variables")]
    public float normSpeed;
    public float maxSpeed;
    public float speed;
    [SerializeField] float maxSpeedMultiplier = 5;


    [Header ("Visual Modifiers")]

    public float ZPos = 0;

    public int drawDistance = 12; //This is distance in segments. If DrawDistance is 12, it will render 12 road segments ahead of the player. 

    public int roadAddonsToRender = 50; //This is how many "Renderer" objects we're going to create to render the differet road addons, like trees, ramps, etc.

    public float segmentLength = 2;

    public float roadWidth = 10;

    public float cameraElevation = 5;

    public float cameraHorizontalOffset = 0;

    public float FOV = 1;

    public float slightUpDownRotation;

    public GameObject renderedSegmentHolder;

    public GameObject renderedSegmentPrefab;

    public List<GameObject> renderedSegmentsList = new List<GameObject>();

    public Segment[] segments;

    public Segment[] endSegments;

    public Vector2 segmentMeshDimensions = new Vector2(2, 2);

    public GameObject renderedRoadAddonHolder;

    public float roadAddonSpriteScale = 3;

    public float roadAddonSpriteVerticalDepthOffset = .25f; //This is necessary for the depth effect to work with Mia's cool shader. The shader has the bonus effect of adding something comparable to a meniscus at the horizon line, but that also means that road addons need to be rendered slightly higher up than they actually are, as they get closer to the horizon. This determines just HOW HIGH we're going. 

    public float treeFrequencyPercentage = 0.9f;

    public float maxTreesPerSegment = 4;

    public float rockFrequencyPercentage = 0.1f;

   //public float minSpaceBetweenRocks = 3;

    public float rampFrequencyPercentage = 0.02f;

    public int minSegmentsBetweenRamps = 10;



    [Header("Course Modifiers")]

    public int maxCurveDistance = 100;

    public int minCurveDistance = 10;

    public int maxCurveLengthInSegments = 30;

    public int maxCurveCurviness = 5;

    private int segmentToCalculateLoopAt;

    private bool calculatedLoop = false;

    public int segmentMaximumDecorations = 20;

    public Sprite[] roadObjectSprites;

    public GameObject[] roadObjectPrefabs;

    private List<GameObject> roadAddonRenderers = new List<GameObject>();





    #region private vars
    float xAspect = 16;
    float yAspect = 9;
    float camHeight = 2f * 5;
    float camWidth;
    float roadEnd = 1000;
    float roadStart = 1;
    #endregion

    public class Segment
    {
        public Vector3 p1;
        public Vector3 p2;

        public Color segmentColor;

        public Sprite segmentSprite;

        public int index;

        public float curviness;

        public List<RoadAddon> roadAddons = new List<RoadAddon>();


        public Segment(Vector3 point1, Vector3 point2, Color thisColor, Sprite thisSegmentSprite, float thisSegmentCurviness) {
            p1 = point1;
            p2 = point2;
            segmentColor = thisColor;
            segmentSprite = thisSegmentSprite;
            curviness = thisSegmentCurviness;

        }
    }

    public class RoadAddon {
        public float horizontalPositionOnSegment;

        public float zPos;

        public Sprite spriteToRender;

        public float horizontalCollisionFraction;


        public RoadAddon(float horizontalPos, float forwardBackwardPos, Sprite sprite, Color color, float horizontalCollidableAmount) {
            horizontalPositionOnSegment = horizontalPos;

            zPos = forwardBackwardPos;

            spriteToRender = sprite;

            horizontalCollisionFraction = horizontalCollidableAmount;
            
        
        }

    
    }

    public void AddRoadObjectAt(int segmentToAddTo, float horizontalPos, float forwardBackwardPos, Sprite sprite, float horizontalCollidableFraction) {

        Debug.Log("So this is where we should be adding an addon?");

        RoadAddon thisAddon = new RoadAddon(horizontalPos, forwardBackwardPos, sprite, Color.white, horizontalCollidableFraction);

        segments[segmentToAddTo].roadAddons.Add(thisAddon);

        

    }
    

    private void Awake()
    {
        maxSpeed = normSpeed * maxSpeedMultiplier;
        speed = normSpeed;

        segments = new Segment[(int)(trackLength/segmentLength)];

        //Adding empty segments to the segments list!
        int loopLength = (int)((trackLength / segmentLength) + 1);
        for (int i = 1; i < loopLength; i++)
        {
            //Debug.Log(new Segment(new Vector3(0, 0, i * segmentLength), new Vector3(0, 0, (i + 1) * segmentLength), Color.white, defaultSegmentSprite));
            segments[i - 1] = new Segment(new Vector3(0, 0, i * segmentLength), new Vector3(0, 0, (i + 1) * segmentLength), Color.white, defaultSegmentSprite, 0);
            segments[i - 1].index = i - 1;

        }


        //Adding empty segments to the endSegments list!

        segmentToCalculateLoopAt = segments.Length - 50;

        endSegments = new Segment[50];




        //make a good number of objects that will act as "Renderers" for our road addons

        for (int i = 0; i < roadAddonsToRender; i++)
        {
            GameObject newAddon = Instantiate(roadObjectPrefabs[0], renderedRoadAddonHolder.transform);

            roadAddonRenderers.Add(newAddon);
            
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        segmentMeshDimensions = new Vector2( Mathf.Ceil(segmentMeshDimensions.x), Mathf.Ceil(segmentMeshDimensions.y));

        camWidth = xAspect / yAspect * camHeight;

        Debug.Log("Are we at least reseting the road?");
        ResetRoad();

        //AddCurveAt(5, 12, 4, 6, 4);

        Debug.Log("So this is where there should be a call for an addon?");
        AddRoadObjectAt(11, 1, 1, roadObjectSprites[0],1f/10f);
        AddRoadObjectAt(12, 1, -1, roadObjectSprites[0],1f/10f);


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

        trackLength = segments.Length * segmentLength;

        //Debug.Log(trackLength);

        //Now we're going to make [DrawDistance] amount of Mesh Renderers
        for (int i = 0; i < drawDistance-1; i++)
        {
            GameObject newRenderedSegment = Instantiate(renderedSegmentPrefab, renderedSegmentHolder.transform);

            renderedSegmentsList.Add(newRenderedSegment);

        }



        //We're adding some random curves here!
        float amountToWait = (int)Random.Range(drawDistance, drawDistance+10);
        for (int i = 9; i < segmentToCalculateLoopAt; i++)
        {
            if (amountToWait > 0)
            {
                amountToWait--;
            }
            else {
                int curveLength = Random.Range(10, maxCurveLengthInSegments + 1);

                int curveEntrySegmentsNumber = 4;//Random.Range(1, 3);

                int curveExitSegmentsNumber = 4;//Random.Range(1, 3);




                float thisCurveCurviness = Random.Range(0, maxCurveCurviness+1);

                if (Random.Range(0, 2) == 1)
                {
                    thisCurveCurviness *= -1;
                }

                Debug.Log(i);
                AddCurveAt(i, curveLength, curveEntrySegmentsNumber, curveExitSegmentsNumber, thisCurveCurviness);

                //amountToWait = (int)(Mathf.Ceil(curveLength));

                if (Random.Range(0, 2) == 1)
                {
                    int curveLength2 = Random.Range(10, maxCurveLengthInSegments + 1);

                    curveEntrySegmentsNumber = 4;//Random.Range(1, 3);

                    curveExitSegmentsNumber = 4;//Random.Range(1, 3);




                    float thisOtherCurveCurviness = Random.Range(0, maxCurveCurviness+1) * (int)-Mathf.Sign(thisCurveCurviness);

                    


                    AddCurveAt(i+curveLength, curveLength2, curveEntrySegmentsNumber, curveExitSegmentsNumber, thisOtherCurveCurviness);



                    amountToWait = (int)(Mathf.Ceil(curveLength + curveLength2)) + Random.Range(minCurveDistance, maxCurveDistance);
                }
                else {
                    amountToWait = (int)(Mathf.Ceil(curveLength)) + Random.Range(minCurveDistance, maxCurveDistance);


                }

            }
        }


        //Now for a buttload of random objects on the track. 
        int segmentsToWaitBeforeAddingRamp = 10;
        for (int i = 5; i < segments.Length; i++)
        {
            Segment segmentWeAddTo = segments[i];

            //Add trees!
            for (int v = 0; v < maxTreesPerSegment; v++)
            {
                float thisTreePercentage = Random.Range(0.0f, 100.0f) / 100;

                Debug.Log("This tree percentage:" + thisTreePercentage + ", base frequency:" + treeFrequencyPercentage);
                

                if (thisTreePercentage <= treeFrequencyPercentage)
                {
                    //Debug.Log("Adding addon to segment number:" + i.ToString());

                    float horizontalRandomOffset = Random.Range(1f, 2); 


                    if (Random.Range(0,2) == 1)
                    {
                        horizontalRandomOffset *= -1;
                    }


                    AddRoadObjectAt(i, 0 + horizontalRandomOffset, Random.Range(-1, 1), roadObjectSprites[0], 1f / 10f);


                }

            }

            //Now we'll add some rocks!

            float thisRockPercentage = Random.Range(0.0f, 100.0f) / 100;

            if (thisRockPercentage <= rockFrequencyPercentage)
            {
                float horizontalRandomOffset = Random.Range(0f, 1f);


                if (Random.Range(0, 2) == 1)
                {
                    horizontalRandomOffset *= -1;
                }

                AddRoadObjectAt(i, 0 + horizontalRandomOffset, Random.Range(-1, 1), roadObjectSprites[2], 9f / 10f);
            }


            //Now we'll add some ramps!
            if (segmentsToWaitBeforeAddingRamp > 0)
            {
                segmentsToWaitBeforeAddingRamp--;
            }
            else
            {
                float thisRampPercentage = Random.Range(0.0f, 100.0f) / 100;
                if (thisRampPercentage <= rampFrequencyPercentage)
                {
                    float horizontalRandomOffset = Random.Range(0f, .8f);


                    if (Random.Range(0, 2) == 1)
                    {
                        horizontalRandomOffset *= -1;
                    }

                    AddRoadObjectAt(i, 0 + horizontalRandomOffset, Random.Range(-1, 1), roadObjectSprites[4], 1f);

                    segmentsToWaitBeforeAddingRamp = minSegmentsBetweenRamps;
                }
            }


        }


    }

    void RenderRoad() {

        Segment baseSegment = FindSegment(ZPos);

        if (baseSegment.index > segmentToCalculateLoopAt && !calculatedLoop)
        {
            calculatedLoop = true;

            for (int i = 0; i < 50; i++)
            {
                endSegments[i] = segments[baseSegment.index + i];
            }

            ResetRoad();

        }

        float basePercent = 1- ((ZPos % segmentLength) / segmentLength);

        float dx = +(baseSegment.curviness * basePercent);
        float x = 0;


        //Debug.DrawLine( WorldToScreen(new Vector3(roadWidth/2,0,baseSegment.p1.z)), WorldToScreen(new Vector3(roadWidth/2, 0, roadEnd)));
        //Debug.Log(WorldToScreen(new Vector3(roadWidth/2, 0, roadStart)).x);

        //Debug.DrawLine(WorldToScreen(new Vector3(-roadWidth / 2, 0, baseSegment.p1.z)), WorldToScreen(new Vector3(-roadWidth / 2, 0, roadEnd)));
        //Debug.Log(WorldToScreen(new Vector3(roadWidth / 2, 0, roadStart)).x);



        //Take a moment to reset the renderers for the road addons
        for (int i = 0; i < roadAddonRenderers.Count; i++)
        {
            GameObject rendererToReset = roadAddonRenderers[i];

            rendererToReset.GetComponent<SpriteRenderer>().sprite = null;

            rendererToReset.transform.position = new Vector3(0, 1000, 0);
        }
        int addonRenderersAvailable = roadAddonsToRender;


        //This is where we draw each road segment
        int meshCounter = 0;
        float dy = 0;
        for (int i = baseSegment.index+1; i < baseSegment.index + drawDistance; i++)
        {
            Segment currentSegment = segments[i % segments.Length];


            //Drawing little dots for debug purposes
            //Debug.DrawLine(WorldToScreen(currentSegment.p1,0-x), WorldToScreen(currentSegment.p1 + new Vector3(0, 0,.1f),0-x));
            //Debug.DrawLine(WorldToScreen(currentSegment.p2, 0-x-dx), WorldToScreen(currentSegment.p2 + new Vector3(0, 0,.1f),0-x-dx), Color.red);

            //Draw left and right segment bounds

            //Debug.DrawLine(WorldToScreen(currentSegment.p1 + new Vector3(roadWidth / 2, 0, 0), -x), WorldToScreen(currentSegment.p2 + new Vector3(roadWidth / 2, 0, 0), -x-dx));



            //Now for the tricky part--making the road segment into a mesh!

            Vector2 meshDimensions = new Vector2(segmentMeshDimensions.x,segmentMeshDimensions.y);


            List<Vector3> vertices = new List<Vector3>();

            //vertices.Add(leftLowerCornerScreenSpace);
            //vertices.Add(rightLowerCornerScreenSpace);
            //vertices.Add(leftUpperCornerScreenSpace);
            //vertices.Add(rightUpperCornerScreenSpace);

            Vector3 rightLowerCornerWorldSpace = currentSegment.p1 + new Vector3(roadWidth / 2, 0, 0);
            Vector3 rightUpperCornerWorldSpace = currentSegment.p2 + new Vector3(roadWidth / 2, 0, 0);


            Vector3 leftLowerCornerWorldSpace = currentSegment.p1 + new Vector3(-roadWidth / 2, 0, 0);
            Vector3 leftUpperCornerWorldSpace = currentSegment.p2 + new Vector3(-roadWidth / 2, 0, 0);


            float xPerStep = roadWidth /meshDimensions.x;
            float yPerStep = segmentLength/meshDimensions.y;
            for (int k = 0; k < meshDimensions.y+1; k++)
            {

                for (int g = 0; g < meshDimensions.x+1; g++)
                {
                    Vector3 currentSegmentVertex = new Vector3(
                        
                        leftLowerCornerWorldSpace.x + g * xPerStep
                        , currentSegment.p1.y
                        , leftLowerCornerWorldSpace.z + k * yPerStep

                        );

                    vertices.Add( WorldToScreen(currentSegmentVertex,-x -(dx * (k/meshDimensions.y) )) );

                    //if (vertices.Count-1 > 0)
                    //{
                        //Debug.DrawLine(vertices[vertices.Count-1], vertices[ vertices.Count - 2]);
                    //}

                }
            }

            List<int> trianglesList = new List<int>();

            for (int k = 0; k < meshDimensions.y; k++)
            {

                for (int g = 0; g < meshDimensions.x; g++)
                {
                    int triangleStartIndex = (int)(g + (k*(meshDimensions.x+1))  );

                    trianglesList.Add(triangleStartIndex);
                    trianglesList.Add((int)(triangleStartIndex + meshDimensions.x + 1));
                    trianglesList.Add((int)(triangleStartIndex + meshDimensions.x + 2));

                    trianglesList.Add(triangleStartIndex);
                    trianglesList.Add((int)(triangleStartIndex + meshDimensions.x + 2));
                    trianglesList.Add(triangleStartIndex+1);
                }
            }



            Vector3 rightUpperCornerScreenSpace = WorldToScreen(currentSegment.p1 + new Vector3(roadWidth / 2, 0, 0), -x);
            Vector3 rightLowerCornerScreenSpace = WorldToScreen(currentSegment.p2 + new Vector3(roadWidth / 2, 0, 0), -x - dx);

            //Debug.DrawLine( rightUpperCornerScreenSpace, rightLowerCornerScreenSpace);


            //Debug.DrawLine(WorldToScreen(currentSegment.p1 + new Vector3(-roadWidth / 2, 0, 0), -x), WorldToScreen(currentSegment.p2 + new Vector3(-roadWidth / 2, 0, 0), -x-dx));

            Vector3 leftUpperCornerScreenSpace = WorldToScreen(currentSegment.p1 + new Vector3(-roadWidth / 2, 0, 0), -x);
            Vector3 leftLowerCornerScreenSpace = WorldToScreen(currentSegment.p2 + new Vector3(-roadWidth / 2, 0, 0), -x - dx);

            Debug.DrawLine(leftUpperCornerScreenSpace, leftLowerCornerScreenSpace);


            

            int segmentMeshIndex = meshCounter;
            //Debug.Log(segmentMeshIndex.ToString());
            GameObject segmentMeshObject = renderedSegmentsList[segmentMeshIndex];
            MeshFilter segmentFilter = segmentMeshObject.GetComponent<MeshFilter>();
            Mesh segmentMesh = segmentFilter.mesh;

            //segmentMesh.Clear();

            //segmentMeshObject.transform.position = WorldToScreen(currentSegment.p1 + currentSegment.p2/2, -x +dx/2);


            /*
            Vector2[] uv = new Vector2[4]
            {
                    new Vector2(0, 0),
                    new Vector2(1, 0),
                    new Vector2(0, 1),
                    new Vector2(1, 1)
            };
            */

            Vector2[] uv = new Vector2[vertices.Count];

            for (int g = 0; g < vertices.Count; g++)
            {
                //Debug.DrawLine(vertices[g], vertices[g] + new Vector3(0, .1f, 0), Color.red);

                //uv[g] = vertices[g];

                uv[g] = new Vector2(  Mathf.Floor( g/(meshDimensions.x+1) )/meshDimensions.y , (g % (meshDimensions.x+1) )/meshDimensions.x );

            }

            //Debug.Log(uv.Length);
            

            

            segmentMesh.vertices = vertices.ToArray();
            segmentMesh.uv = uv;
            segmentMesh.triangles = trianglesList.ToArray();

           // Debug.Log(vertices.ToArray().Length);
           // Debug.Log(trianglesList.ToArray().Length);







            //Little criss-cross pattern for decoration and more visibility
            //Debug.DrawLine(WorldToScreen(currentSegment.p1 + new Vector3(-roadWidth / 2, 0, 0), -x), WorldToScreen(currentSegment.p2 + new Vector3(roadWidth / 2, 0, 0), -x-dx));
            //Debug.DrawLine(WorldToScreen(currentSegment.p1 + new Vector3(roadWidth / 2, 0, 0), -x), WorldToScreen(currentSegment.p2 + new Vector3(-roadWidth / 2, 0, 0), -x-dx));


            //Lines where segments meet
            Debug.DrawLine(leftLowerCornerScreenSpace, rightLowerCornerScreenSpace);
            Debug.DrawLine(leftUpperCornerScreenSpace, rightUpperCornerScreenSpace);


            //here we render the different road addons.
            for (int v = 0; v < currentSegment.roadAddons.Count; v++)
            {
                if (addonRenderersAvailable > 0)
                {



                    Debug.Log("one log, coming up!");
                    RoadAddon addonToRender = currentSegment.roadAddons[v];

                    float addonHorizontalWorldPos = addonToRender.horizontalPositionOnSegment * roadWidth / 2;

                    float addonLateralPos = (currentSegment.p1.z + currentSegment.p2.z) / 2 - segmentLength / 2 * addonToRender.zPos;

                    //Debug.Log(roadAddonsToRender - addonRenderersAvailable);

                    GameObject addonRenderer = roadAddonRenderers[roadAddonsToRender - addonRenderersAvailable];

                    SpriteRenderer addonRendererRenderer = addonRenderer.GetComponent<SpriteRenderer>();

                    addonRendererRenderer.sprite = addonToRender.spriteToRender;

                    float percentAlongSegment = Mathf.InverseLerp(1, -1, addonToRender.zPos);

                    float zCam = addonLateralPos - ZPos;

                    Vector3 addonWorldPos = new Vector3(addonHorizontalWorldPos, currentSegment.p1.y, addonLateralPos);

                    addonRenderer.transform.position = WorldToScreen(addonWorldPos, -x - dx * percentAlongSegment);

                    

                    addonRenderer.transform.localScale = Vector3.one * roadAddonSpriteScale * (1 / Mathf.Tan(FOV / 2)) / zCam;

                    float halfHeight = addonRendererRenderer.bounds.size.y / 2;

                    float camDistFromAddon = Vector3.Distance( new Vector3(cameraHorizontalOffset,currentSegment.p1.y ,ZPos), addonWorldPos) ;

                    addonRenderer.transform.position += new Vector3(0, halfHeight + roadAddonSpriteVerticalDepthOffset * camDistFromAddon, 0);

                    addonRendererRenderer.sortingOrder = (int)-Mathf.Round(addonLateralPos);

                    addonRenderer.GetComponent<BoxCollider2D>().size = new Vector2(addonRendererRenderer.bounds.size.x / addonRenderer.transform.lossyScale.x * addonToRender.horizontalCollisionFraction, addonRendererRenderer.bounds.size.y/addonRenderer.transform.lossyScale.y);

                    addonRenderersAvailable--;

                }
            }




            x = x + dx;
            dx = dx + currentSegment.curviness;

            meshCounter++;
            //dy += roadAddonSpriteVerticalDepthOffset;

        }
    }

    public Segment FindSegment(float currentZPosition) {

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

            segmentToCurve.curviness = EaseIn(0, overallCurviness, (float)i / (float)curveEntrySegments );
        }

        for (int i = 0; i < segmentsInCurve-curveExitSegments; i++)
        {
            Segment segmentToCurve = segments[segmentCurveStartsAt + curveEntrySegments + i];

            segmentToCurve.curviness = overallCurviness;
        }

        for (int i = 0; i < curveExitSegments; i++)
        {
            Segment segmentToCurve = segments[segmentCurveStartsAt + segmentsInCurve - curveExitSegments + i];

            segmentToCurve.curviness = EaseInOut(overallCurviness, 0, (float)i / (float)curveExitSegments);
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
