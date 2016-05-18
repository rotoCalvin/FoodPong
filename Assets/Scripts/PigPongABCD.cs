using UnityEngine;
using System.Collections;

using OpenCVForUnity;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class PigPongABCD : MonoBehaviour {

    private const int CAMERA_WIDTH = 640;
    private const int CAMERA_HEIGHT = 360;

    


    public bool showWebcamImage = false;
    private GameObject webcamDisplayCanvas;



    public enum MODE {
        None,
        Calibrate,
        Screen,
        Ball,
        Test
    };
    public MODE mode = MODE.None;

    public enum HIT
    {
        None,
        Left,
        Right
    };


    private bool doPerspectiveShift = false;
    private int perspectiveIndex = 0;
    private Vector2[] perspectiveCoordinates = new Vector2[4];


    public float greyscaleThreshold = 0.6f;

    private float personPercentage = 30f;
    public int personHeightPixels = 150;
    public float greyscalePixelCount = 5f;


    public Sprite calibrationSprite;
    private bool calibrated = true;

    private float a, b, c, d, e, f, g, h;




    



    private WebCamTexture webcamTexture;

    public float updateSeconds = 0.3f;
    private float updateTimer = 0.0f;


    private PropertiesFile pf;

    private FoodManager foodManager;




    // Use this for initialization
    void Start ()
    {
        string configPath = Application.dataPath + "/Scripts/config/camera.ini";
        ReadInConfig(configPath);
        Debug.Log("configPath: " + configPath);

        webcamTexture = new WebCamTexture(CAMERA_WIDTH, CAMERA_HEIGHT);
        webcamTexture.Play();

        webcamDisplayCanvas = GameObject.Find("Webcam Display Canvas");

        if (showWebcamImage)
        {
            GetComponentInChildren<RawImage>().texture = webcamTexture;
        }
        else
        {
            webcamDisplayCanvas.SetActive(false);
        }
    }


    private void ReadInConfig(string configFilename)
    {
        pf = new PropertiesFile(configFilename);

        updateSeconds = float.Parse(pf.get("update_seconds"));

        foodManager = GetComponent<FoodManager>();
        foodManager.Setup(Int32.Parse(pf.get("max_balls")), float.Parse(pf.get("spawn_time")));

        float personHeightPercentage = float.Parse(pf.get("personHeightPercentage"));
        
        a = float.Parse(pf.get("a"));
        b = float.Parse(pf.get("b"));
        c = float.Parse(pf.get("c"));
        d = float.Parse(pf.get("d"));
        e = float.Parse(pf.get("e"));
        f = float.Parse(pf.get("f"));
        g = float.Parse(pf.get("g"));
        h = float.Parse(pf.get("h"));

        Debug.Log("A is " + a);

        personPercentage = Int32.Parse(pf.get("personPercentage"));
        greyscalePixelCount = Int32.Parse(pf.get("greyscalePixelCount"));
    }


	
	// Update is called once per frame
	void Update ()
    {
        updateTimer += Time.deltaTime;

        if (!calibrated)
        {
            calibrated = true;
            mode = MODE.Calibrate;
            Debug.Log("setting mode to CALIBRATE");
        }

        if (updateTimer >= updateSeconds)
        {
            DoShadowCalculation();

            updateTimer = 0;
        }

        //RunTests();
    }



    




    private void DoShadowCalculation()
    {
        if (doPerspectiveShift)
        {
            ShiftPerspective();
        }

        switch (mode)
        {
            case MODE.Calibrate:
                //Debug.Log("Calibrating");
                Calibrate();
                break;
            case MODE.Ball:
                //Debug.Log("Looking for Ball");
                //CalculateBallHits();
                MakeRigidBodyLines();
                break;
            case MODE.Screen:
                //Debug.Log("Looking for Player");
                CalculateScreenMovement();
                break;
            case MODE.Test:
                ShowBallHit();
                break;
            
        }
    }



    // TODO: ADD TO CONFING
    public int minimumShadowCount = 500;
    public int minimumShadowDiff = 6000;
    private void Calibrate()
    {
        float newThreshold = 0f;
        int newShadowCount = 0, lastShadowCount = 0;

        //while (newShadowCount < minimumShadowCount || newShadowCount - lastShadowCount > minimumShadowDiff)
        while (lastShadowCount == 0 || newShadowCount - lastShadowCount < minimumShadowDiff)
        {
            lastShadowCount = newShadowCount;
            newShadowCount = 0;
            newThreshold += 0.005f;

            int x = 0, y = 0;
            for (y = 0; y < perspectiveTexture.height; y++)
            {
                for (x = 0; x < perspectiveTexture.width; x++)
                {
                    Color rgb = perspectiveTexture.GetPixel(x, y); // perspectiveTexture
                    if (rgb.grayscale <= newThreshold)
                    {
                        newShadowCount++;
                    }
                }
            }

            Debug.Log("Calibration at " + newThreshold  + " - " + lastShadowCount + "/" + newShadowCount);
        }

        greyscaleThreshold = newThreshold;
        //mode = MODE.Screen;
        mode = MODE.None;

        GetComponentInChildren<RawImage>().texture = null;
        webcamDisplayCanvas.SetActive(false);
    }


    public void ShowCalibratedImage()
    {
        float newThreshold = 0f;

        Texture2D calibratedTexture = new Texture2D((int)perspectiveTexture.width, (int)perspectiveTexture.height);

        for (newThreshold = 0f; newThreshold <= 1f; newThreshold += 0.005f)
        {
            int x = 0, y = 0;
            for (y = 0; y < perspectiveTexture.height; y++)
            {
                for (x = 0; x < perspectiveTexture.width; x++)
                {
                    Color rgb = perspectiveTexture.GetPixel(x, y);
                    //float greyscaleValue = (rgb.r + rgb.g + rgb.b) / 3f;
                    if (rgb.grayscale <= greyscaleThreshold)
                    {
                        calibratedTexture.SetPixel(x, y, Color.white);
                    }
                    else
                    {
                        calibratedTexture.SetPixel(x, y, Color.blue);
                    }
                }
            }
        }

        calibratedTexture.Apply();
        webcamDisplayCanvas.SetActive(true);
        GetComponentInChildren<RawImage>().texture = calibratedTexture;
        mode = MODE.None;
        Debug.Log("Made Calibration Image");
    }





    private void CalculateScreenMovement()
    {
        float greyscaleCount = 0;

        float greyscaleTotal = 0;

        for (int x = 0; x < perspectiveTexture.width; x++)
        {
            Color rgb = perspectiveTexture.GetPixel(x, personHeightPixels);
            //float greyscaleValue = (rgb.r + rgb.g + rgb.b) / 3f;
            if (rgb.grayscale <= greyscaleThreshold)//greyscaleValue
            {
                greyscaleCount++;
            }

            perspectiveTexture.SetPixel(x, personHeightPixels, Color.red);
            greyscaleTotal += rgb.grayscale;//greyscaleValue
        }

        if (showWebcamImage)
        {
            perspectiveTexture.Apply();
            GetComponentInChildren<RawImage>().texture = perspectiveTexture;
        }

        float greyscalePercentage = (greyscaleCount / perspectiveTexture.width) * 100f;

        // test
        float averageGreyscale = greyscaleTotal / perspectiveTexture.width;

        Debug.Log("Screen Movement: " + greyscalePercentage + "%, average greyscale: " + averageGreyscale);
        if (greyscalePercentage > personPercentage)
        {
            // shadow detected
            Debug.Log("Shadow Detected among " + greyscaleCount + " pixels");


            mode = MODE.Ball;
            GetComponent<GameManager>().StartDroppingFood();
            GetComponent<GameManager>().MoveTitleSignUp();
        }
    }


    private void CalculateBallHits()
    {
        Camera cam = GetComponentInParent<Camera>();

        int[] counts = new int[2] { 0, 0 };
        float x, y, leftX, middleX, rightX;
        Vector2 bottomLeft, bottomRight, middle;
        List<GameObject> balls = GetComponent<FoodManager>().GetCurrentFoods();

        foreach (GameObject ball in balls)
        {
            counts[0] = 0;
            counts[1] = 0;
            BoundingBox2D boundingBox = new BoundingBox2D(ball);

            if (boundingBox.topLeft.y < 3.5f)
            {
                bottomLeft = cam.WorldToScreenPoint(boundingBox.topLeft);
                middle = cam.WorldToScreenPoint(boundingBox.middle);
                bottomRight = cam.WorldToScreenPoint(boundingBox.topRight);

                y = CameraUtils.MapScreenToCamera(bottomLeft.y, cam.pixelHeight, perspectiveTexture.height);
                leftX = CameraUtils.MapScreenToCamera(bottomLeft.x, cam.pixelWidth, perspectiveTexture.width);
                middleX = CameraUtils.MapScreenToCamera(middle.x, cam.pixelWidth, perspectiveTexture.width);
                rightX = CameraUtils.MapScreenToCamera(bottomRight.x, cam.pixelWidth, perspectiveTexture.width);

                y = Math.Max(y, 0f);
                y = Math.Min(y, perspectiveTexture.height);
                leftX = Math.Max(leftX, 0f);
                leftX = Math.Min(leftX, perspectiveTexture.width);
                middleX = Math.Max(middleX, 0f);
                middleX = Math.Min(middleX, perspectiveTexture.width);
                rightX = Math.Max(rightX, 0f);
                rightX = Math.Min(rightX, perspectiveTexture.width);

                for (x = leftX; x < middleX; x++)
                {
                    for (int i = -2; i < 3; i+=2)
                    {
                        Color rgb = perspectiveTexture.GetPixel((int)x, (int)y + i);
                        if (rgb.grayscale < greyscaleThreshold)
                        {
                            counts[0]++;
                        }
                    }
                }

                for (x = middleX; x < rightX; x++)
                {
                    for (int i = -2; i < 3; i += 2)
                    {
                        Color rgb = perspectiveTexture.GetPixel((int)x, (int)y + i);
                        if (rgb.grayscale < greyscaleThreshold)
                        {
                            counts[1]++;
                        }
                    }
                }

                if (Math.Max(counts[0], counts[1]) > greyscalePixelCount)
                {
                    if (counts[0] >= counts[1])
                    {
                        PerformHit(ball, HIT.Left);
                        Debug.Log("HIT - " + counts[0]);
                    }
                    else
                    {
                        PerformHit(ball, HIT.Right);
                        Debug.Log("HIT - " + counts[1]);
                    }
                }
            }
        }
    }


    public void ShowBallHit()
    {
        Camera cam = GetComponentInParent<Camera>();

        Texture2D hitTexture = new Texture2D(perspectiveTexture.width, perspectiveTexture.height);
        hitTexture.SetPixels(perspectiveTexture.GetPixels());

        int[] counts = new int[2] { 0, 0 };
        float x, y, leftX, middleX, rightX;
        Vector2 bottomLeft, bottomRight, middle;
        List<GameObject> balls = GetComponent<FoodManager>().GetCurrentFoods();

        foreach (GameObject ball in balls)
        {

            BoundingBox2D boundingBox = new BoundingBox2D(ball);

            bottomLeft = cam.WorldToScreenPoint(boundingBox.topLeft);
            middle = cam.WorldToScreenPoint(boundingBox.middle);
            bottomRight = cam.WorldToScreenPoint(boundingBox.topRight);

            y = CameraUtils.MapScreenToCamera(bottomLeft.y, cam.pixelHeight, perspectiveTexture.height);
            leftX = CameraUtils.MapScreenToCamera(bottomLeft.x, cam.pixelWidth, perspectiveTexture.width);
            middleX = CameraUtils.MapScreenToCamera(middle.x, cam.pixelWidth, perspectiveTexture.width);
            rightX = CameraUtils.MapScreenToCamera(bottomRight.x, cam.pixelWidth, perspectiveTexture.width);

            y = Math.Max(y, 0f);
            y = Math.Min(y, perspectiveTexture.height);
            leftX = Math.Max(leftX, 0f);
            leftX = Math.Min(leftX, perspectiveTexture.width);
            middleX = Math.Max(middleX, 0f);
            middleX = Math.Min(middleX, perspectiveTexture.width);
            rightX = Math.Max(rightX, 0f);
            rightX = Math.Min(rightX, perspectiveTexture.width);

            for (x = leftX; x < middleX; x++)
            {
                //hitTexture.SetPixel((int)x, (int)y, Color.red);
                for (int i = -2; i < 3; i += 2)
                {
                    hitTexture.SetPixel((int)x, (int)y + i, Color.red);
                }
            }

            for (x = middleX; x < rightX; x++)
            {
                //hitTexture.SetPixel((int)x, (int)y, Color.green);
                for (int i = -2; i < 3; i += 2)
                {
                    hitTexture.SetPixel((int)x, (int)y + i, Color.green);
                }
            }

            Debug.DrawLine(boundingBox.topLeft, boundingBox.bottomLeft, Color.red);
            Debug.DrawLine(boundingBox.topLeft, boundingBox.topRight, Color.red);
            Debug.DrawLine(boundingBox.topRight, boundingBox.bottomRight, Color.red);
            Debug.DrawLine(boundingBox.bottomLeft, boundingBox.bottomRight, Color.red);
        }



        hitTexture.Apply();
        webcamDisplayCanvas.SetActive(true);
        GetComponentInChildren<RawImage>().texture = hitTexture;
    }








    public void MakeRigidBodyLines()
    {
        GetComponent<ShadowCollider>().SetPointsFromTexture(perspectiveTexture, greyscaleThreshold);

        /*
        Texture2D hitTexture = new Texture2D(perspectiveTexture.width, perspectiveTexture.height);
        hitTexture.SetPixels(perspectiveTexture.GetPixels());

        GetComponent<ShadowCollider>().SetPointsFromTexture(hitTexture, greyscaleThreshold);
        hitTexture.Apply();

        webcamDisplayCanvas.SetActive(true);
        GetComponentInChildren<RawImage>().texture = hitTexture;

        mode = MODE.None;
        */
    }

    public void ShowRigidBodyLines()
    {
        Texture2D hitTexture = new Texture2D(perspectiveTexture.width, perspectiveTexture.height);
        hitTexture.SetPixels(perspectiveTexture.GetPixels());

        GetComponent<ShadowCollider>().ShowPointsFromTexture(hitTexture, greyscaleThreshold);
        hitTexture.Apply();

        webcamDisplayCanvas.SetActive(true);
        GetComponentInChildren<RawImage>().texture = hitTexture;

        mode = MODE.None;
    }









    private Mat firstMat;
    public void RangeFind()
    {
        // bgr
        Scalar lowerBound = new Scalar(0, 0, 200);
        Scalar upperBound = new Scalar(50, 50, 255);

        Mat mask = new Mat();
        Core.inRange(firstMat, lowerBound, upperBound, mask);

        Texture2D testTexture = new Texture2D(mask.cols(), mask.rows(), TextureFormat.RGBA32, false);
        Utils.matToTexture2D(mask, testTexture);

        webcamDisplayCanvas.SetActive(true);
        GetComponentInChildren<RawImage>().texture = testTexture;



        /* CRASHES EVERYTHING!!!
        OpenCVForUnity.BackgroundSubtractorMOG bs = new BackgroundSubtractorMOG(new IntPtr(3));
        Mat fgMask = new Mat();
        bs.apply(firstMat, fgMask, 0.1);

        Texture2D testTexture = new Texture2D(fgMask.cols(), fgMask.rows(), TextureFormat.RGBA32, false);
        Utils.matToTexture2D(fgMask, testTexture);
        GetComponentInChildren<RawImage>().texture = testTexture;
        */
    }



















    private Texture2D perspectiveTexture;
    private Mat perspectiveMat, perspectiveTransform, webcamMat;
    private void ShiftPerspective()
    {
        Utils.webCamTextureToMat(this.webcamTexture, webcamMat);

        // instantiate somewhere for the output image to go
        perspectiveMat = webcamMat.clone();

        // warp the image using the transform
        Imgproc.warpPerspective(webcamMat, perspectiveMat, perspectiveTransform, new Size(webcamMat.rows(), webcamMat.cols()));

        /*
        // TEST draw blue line accross width of image 10 pixels down
        double[] blue = new double[4] { 0, 0, 255, 255 };
        for (int x = 0; x < perspectiveMat.cols(); x++)
        {
            perspectiveMat.put(10, x, blue);
        }
        */
        if (firstMat == null)
        {
            firstMat = perspectiveMat.clone();
        }


        //perspectiveTexture = CameraUtils.ConvertMatToTexture2D(perspectiveMat);
        if (perspectiveTexture == null)
        {
            perspectiveTexture = new Texture2D(perspectiveMat.cols(), perspectiveMat.rows(), TextureFormat.RGBA32, false);
        }
        Utils.matToTexture2D(perspectiveMat, perspectiveTexture);


        /*
        // TEST
        for (int x = 0; x < perspectiveTexture.width; x++)
        {
            perspectiveTexture.SetPixel(x, perspectiveTexture.height - 10, Color.blue);
        }
        perspectiveTexture.Apply();
        Debug.Log(CAMERA_HEIGHT + " OR " + perspectiveTexture.height + ", " + CAMERA_WIDTH + " OR " + perspectiveTexture.width);
        */

        if (showWebcamImage)
        {
            GetComponentInChildren<RawImage>().texture = perspectiveTexture;
        }


        


        // dispose of mats
        perspectiveMat.Dispose();
    }















    public float hitAmount = 8f;
    private void PerformHit(GameObject food, HIT hitDirection)
    {
        Rigidbody2D rb = food.GetComponent<Rigidbody2D>();
        rb.velocity = Vector3.zero;
        rb.transform.Rotate(FoodManager.RandomRotation());
        rb.AddForce(Vector2.up * hitAmount, ForceMode2D.Impulse);

        food.GetComponent<Food>().Hit();
    }






    public void SetupPerspectiveShift()
    {
        // Debug.Log("adding point " + perspectiveIndex);
        if (GetComponentInChildren<RawImage>() == null)
        {
            return;
        }

        Vector3 mousePosition = Input.mousePosition;
        UnityEngine.Rect camPosition = GetComponentInParent<Camera>().pixelRect;
        UnityEngine.Rect rawPosition = GetComponentInChildren<RawImage>().rectTransform.rect;

        float xPos = GetRawImageMouseX(mousePosition.x, rawPosition, camPosition);
        float yPos = GetRawImageMouseY(mousePosition.y, rawPosition, camPosition);
        if (xPos != -1 && yPos != -1)
        {
            Vector2 cameraPosition = new Vector2(xPos, yPos);

            //Debug.Log("mouse position: " + mousePosition.x + ", " + mousePosition.y);
            //Debug.Log("screen Position: " + screenPosition.x + ", " + screenPosition.y);
            Debug.Log("camera position: " + cameraPosition.x + ", " + cameraPosition.y);

            if (perspectiveIndex <= 3)
            {
                perspectiveCoordinates[perspectiveIndex] = cameraPosition;
            }
            

            if (perspectiveIndex == 3)
            {
                Debug.Log("doing perspective shift!");
                webcamMat = new Mat(this.webcamTexture.height, this.webcamTexture.width, CvType.CV_8UC4);
                Utils.webCamTextureToMat(this.webcamTexture, webcamMat);

                Mat src_mat = new Mat(4, 1, CvType.CV_32FC2);
                Mat dst_mat = new Mat(4, 1, CvType.CV_32FC2);

                // create the transform
                src_mat.put(0, 0, perspectiveCoordinates[0].x, perspectiveCoordinates[0].y, perspectiveCoordinates[1].x, perspectiveCoordinates[1].y,
                    perspectiveCoordinates[2].x, perspectiveCoordinates[2].y, perspectiveCoordinates[3].x, perspectiveCoordinates[3].y);
                dst_mat.put(0, 0, 0.0, 0.0, webcamMat.rows(), 0.0, 0.0, webcamMat.cols(), webcamMat.rows(), webcamMat.cols());
                //dst_mat.put(0, 0, 0.0, 0.0, 180, 0.0, 0.0, 320, 180, 320);

                Debug.Log(webcamMat.rows() + "," + webcamMat.cols());

                perspectiveTransform = Imgproc.getPerspectiveTransform(src_mat, dst_mat);

                // dispose
                src_mat.Dispose();
                dst_mat.Dispose();

                doPerspectiveShift = true;
            }

            perspectiveIndex++;
        }
    }



    private float GetRawImageMouseX(float _x, UnityEngine.Rect inner, UnityEngine.Rect outer)
    {
        //if point is inside inner rectangle
        if (_x > ((outer.width - inner.width) / 2) && _x < (((outer.width - inner.width) / 2) + inner.width))
        {
            //calculate x relative to inner rectangle which is RawImage in our case
            _x = _x - ((outer.width - inner.width) / 2);
        }
        else //if point is outside the inner rectangle
        {
            return -1;
        }
        return _x;
    }

    private float GetRawImageMouseY(float _y, UnityEngine.Rect inner, UnityEngine.Rect outer)
    {
        //if point is inside inner rectangle
        if (_y > ((outer.height - inner.height) / 2) && _y < (((outer.height - inner.height) / 2) + inner.height))
        {
            //calculate y relative to inner rectangle which is RawImage in our case
            _y = _y - ((outer.height - inner.height) / 2);

            //unity does the y coordinate as 0 at bottom, this converts the y so 0 is at the top
            _y = Math.Abs(inner.height - _y);
        }
        else //if point is outside the inner rectangle
        {
            return -1;
        }
        return _y;
    }




    private void RunTests()
    {
        List<GameObject> foodsOnScreen = foodManager.GetCurrentFoods();

        for (int i = 0; i < foodsOnScreen.Count; i++)
        {
            BoundingBox2D boundingBox = new BoundingBox2D(foodsOnScreen[i]);

            Debug.DrawLine(boundingBox.topLeft, boundingBox.bottomLeft, Color.red);
            Debug.DrawLine(boundingBox.topLeft, boundingBox.topRight, Color.red);
            Debug.DrawLine(boundingBox.topRight, boundingBox.bottomRight, Color.red);
            Debug.DrawLine(boundingBox.bottomLeft, boundingBox.bottomRight, Color.red);
        }
    }




    public void ToggleVideoFeed()
    {
        showWebcamImage = !showWebcamImage;
        Debug.Log("showing webcam image: " + showWebcamImage);

        if (showWebcamImage)
        {
            webcamDisplayCanvas.SetActive(true);
            GetComponentInChildren<RawImage>().texture = webcamTexture;

        }
        else
        {
            GetComponentInChildren<RawImage>().texture = null;
            webcamDisplayCanvas.SetActive(false);
        }
    }


    public void StartCalibration()
    {
        calibrated = false;
        webcamDisplayCanvas.SetActive(true);
        GetComponentInChildren<RawImage>().texture = calibrationSprite.texture;
    }




}


/*
        Mat mat1 = new Mat(); // last frame
        Mat mat2 = new Mat(); // this frame

        MatOfDMatch matches = new MatOfDMatch();
        MatOfDMatch gm = new MatOfDMatch();

        List<DMatch> goodMatches = new List<DMatch>();
        List<Point> objList = new List<Point>();
        List<Point> sceneList = new List<Point>();

        MatOfKeyPoint keypointsObject = new MatOfKeyPoint();
        MatOfKeyPoint keypointsScene = new MatOfKeyPoint();

        Mat descriptorsObject = new Mat();
        Mat descriptorsScene = new Mat();

        MatOfPoint2f obj = new MatOfPoint2f();
        MatOfPoint2f scene = new MatOfPoint2f();

        FeatureDetector fd = FeatureDetector.create(FeatureDetector.ORB);
        fd.detect(mat1, keypointsObject);
        fd.detect(mat2, keypointsScene);

        // Calculate Descriptors (Feature Vectors)
        DescriptorExtractor extractor = DescriptorExtractor.create(3);
        extractor.compute(mat1, keypointsObject, descriptorsObject);
        extractor.compute(mat2, keypointsScene, descriptorsScene);

        DescriptorMatcher matcher = DescriptorMatcher.create(DescriptorMatcher.BRUTEFORCE_HAMMING);
        matcher.match(descriptorsObject, descriptorsScene, matches);

        float maxDist = 0, minDist = 100;
        List<DMatch> matchesList = matches.toList();

        // Quick Calculation of max and min distances between keypoints
        int i = 0;
        for (i = 0; i < descriptorsObject.rows(); i++)
        {
            float dist = matchesList[i].distance;
            if (dist < minDist)
                minDist = dist;
            else if (dist > maxDist)
                maxDist = dist;
        }

        for (i = 0; i < descriptorsObject.rows(); i++)
        {
            if (matchesList[i].distance < (3 * minDist))
            {
                goodMatches.Add(matchesList[i]);
            }
        }

        gm.fromList(goodMatches);

        List<KeyPoint> keypointsObjectList = keypointsObject.toList();
        List<KeyPoint> keypointsSceneList = keypointsScene.toList();

        for (i = 0; i < goodMatches.Count; i++)
        {
            objList.Add(keypointsObjectList[goodMatches[i].queryIdx].pt);
            sceneList.Add(keypointsSceneList[goodMatches[i].trainIdx].pt);
        }
        obj.fromList(objList);
        scene.fromList(sceneList);

        Mat Hh = Calib3d.findHomography(obj, scene);

        Mat warping = mat1.clone();
        Size ims = new Size(mat1.cols(), mat1.rows());
        Imgproc.warpPerspective(mat1, warping, h, ims);
        */
