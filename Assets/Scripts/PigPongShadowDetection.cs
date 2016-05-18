using UnityEngine;
using System.Collections;

using OpenCVForUnity;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class PigPongShadowDetection : MonoBehaviour {

    private const int CAMERA_WIDTH = 640;
    private const int CAMERA_HEIGHT = 360;

    


    public bool showWebcamImage = false;
    private GameObject webcamDisplayCanvas;


    public enum MODE {
        None,
        Calibrate,
        Screen,
        Ball
    };

    public enum HIT
    {
        None,
        Left,
        Right
    };


    private bool doPerspectiveShift = false;
    private int perspectiveIndex = 0;
    private Vector2[] perspectiveCoordinates = new Vector2[4];


    public float greyscaleThreshold = 0.35f;

    private float personPercentage = 30f;
    public int personHeightPixels = 150;


    public Sprite calibrationSprite;
    private bool calibrated = true;

    public float a, b, c, d, e, f, g, h;




    private MODE mode = MODE.None;



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

        personPercentage = Int32.Parse(pf.get("personPercentage"));
        //greyscalePixelCount = Int32.Parse(pf.get("greyscalePixelCount"));
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

        GetInput();

        //RunTests();
    }



    private void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // close application
        {
            Application.Quit();
        }
        else if (Input.GetKeyDown(KeyCode.Space)) // show the webcam image when spacebar is pressed
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
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            calibrated = false;
            webcamDisplayCanvas.SetActive(true);
            GetComponentInChildren<RawImage>().texture = calibrationSprite.texture;
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            mode = MODE.Screen;
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            mode = MODE.Ball;
        }


        else if (Input.GetKeyDown(KeyCode.L))
        {
            GetComponentInParent<DebugConsole>().visible = !GetComponentInParent<DebugConsole>().visible;
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            ShowCalibratedImage();
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            ShowBallHit();
        }


        // calibrate camera position
        if (Input.GetMouseButtonDown(0))
        {
            SetupPerspectiveShift();
        }


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
                CalculateBallHits();
                break;
            case MODE.Screen:
                //Debug.Log("Looking for Player");
                CalculateScreenMovement();
                break;
            
        }
    }



    private void Calibrate()
    {
        float calibrationX = calibrationSprite.textureRect.x;
        float calibrationY = calibrationSprite.textureRect.y;
        float calibrationEndX = calibrationSprite.textureRect.x + calibrationSprite.textureRect.width;
        float calibrationEndY = calibrationSprite.textureRect.y + calibrationSprite.textureRect.height;

        float newThreshold = 0f;
        int newShadowCount = 0, lastShadowCount = 0;

        while (newShadowCount < 100f || newShadowCount - lastShadowCount > 10000)
        {
            lastShadowCount = newShadowCount;
            newShadowCount = 0;
            newThreshold += 0.005f;

            int x = 0, y = 0;
            for (y = Convert.ToInt16(calibrationY); y < calibrationEndY; y++)
            {
                for (x = Convert.ToInt16(calibrationX); x < calibrationEndX; x++)
                {
                    Color rgb = perspectiveTexture.GetPixel(x, y);
                    float greyscaleValue = (rgb.r + rgb.g + rgb.b) / 3f;
                    if (greyscaleValue <= newThreshold)
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


    private void ShowCalibratedImage()
    {
        float calibrationX = 0;
        float calibrationY = 0;
        float calibrationEndX = perspectiveTexture.width;
        float calibrationEndY = perspectiveTexture.height;

        float newThreshold = 0f;

        Texture2D calibratedTexture = new Texture2D((int)perspectiveTexture.width, (int)perspectiveTexture.height);

        for (newThreshold = 0f; newThreshold <= 1f; newThreshold += 0.005f)
        {
            int x = 0, y = 0;
            for (y = Convert.ToInt16(calibrationY); y < calibrationEndY; y++)
            {
                for (x = Convert.ToInt16(calibrationX); x < calibrationEndX; x++)
                {
                    Color rgb = perspectiveTexture.GetPixel(x, y);
                    float greyscaleValue = (rgb.r + rgb.g + rgb.b) / 3f;
                    if (greyscaleValue <= greyscaleThreshold)
                    {
                        calibratedTexture.SetPixel(x, y, Color.red);
                    }
                    else
                    {
                        calibratedTexture.SetPixel(x, y, Color.grey);
                    }
                }
            }
        }

        calibratedTexture.Apply();
        webcamDisplayCanvas.SetActive(true);
        GetComponentInChildren<RawImage>().texture = calibratedTexture;
        //mode = MODE.Screen;
        mode = MODE.None;
        Debug.Log("Made Calibration Image");
    }



    private void CalculateScreenMovement()
    {
        float greyscaleCount = 0;

        // test
        float greyscaleTotal = 0;

        for (int x = 0; x < perspectiveTexture.width; x++)
        {
            Color rgb = perspectiveTexture.GetPixel(x, personHeightPixels);
            float greyscaleValue = (rgb.r + rgb.g + rgb.b) / 3f;
            if (greyscaleValue <= greyscaleThreshold)
            {
                greyscaleCount++;
            }

            perspectiveTexture.SetPixel(x, personHeightPixels, Color.red);

            // test
            greyscaleTotal += greyscaleValue;
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
        }
    }


    private void CalculateBallHits()
    {
        /*
        Camera cam = GetComponentInParent<Camera>();
        List<GameObject> balls = GetComponent<FoodManager>().GetCurrentFoods();
        float greyscaleCount = 0f, greyScalePercentageLeft = 0f, greyScalePercentageRight = 0f;
        int x = 0;

        Texture2D hitTexture = new Texture2D((int)perspectiveTexture.width, (int)perspectiveTexture.height);
        hitTexture.SetPixels(perspectiveTexture.GetPixels());

        foreach (GameObject ball in balls)
        {
            BoundingBox2D ballBounds = new BoundingBox2D(ball);

            Vector2 bottomLeft = cam.WorldToScreenPoint(ballBounds.topLeft);
            //bottomLeft.y = cam.pixelHeight - bottomLeft.y;
            Vector2 bottomRight = cam.WorldToScreenPoint(ballBounds.topRight);
            //bottomRight.y = cam.pixelHeight - bottomRight.y;

            int bottomLeftX = (int)CameraUtils.MapScreenToCamera(bottomLeft.x, cam.pixelWidth, CAMERA_WIDTH);
            int bottomRightX = (int)CameraUtils.MapScreenToCamera(bottomRight.x, cam.pixelWidth, CAMERA_WIDTH);
            int bottomMiddleX = (int)CameraUtils.MapScreenToCamera((bottomRight.x - bottomLeft.x) / 2, cam.pixelWidth, CAMERA_WIDTH);
            int bottomY = (int)CameraUtils.MapScreenToCamera(bottomLeft.y, cam.pixelHeight, CAMERA_HEIGHT);

            greyScalePercentageLeft = 0f;
            greyScalePercentageRight = 0f;

            hitTexture.SetPixel(5, 5, Color.blue);

            greyscaleCount = 0f;
            // left side
            for (x = bottomLeftX; x <= bottomMiddleX; x++)
            {
                Color rgb = perspectiveTexture.GetPixel(x, bottomY);
                //float greyscaleValue = (rgb.r + rgb.g + rgb.b) / 3f;
                float greyscaleValue = rgb.grayscale;
                if (greyscaleValue <= greyscaleThreshold)
                {
                    greyscaleCount++;
                }
                hitTexture.SetPixel(x, bottomY, Color.red);
            }
            greyScalePercentageLeft = (greyscaleCount / perspectiveTexture.width) * 100f;

            // right side
            greyscaleCount = 0f;
            for (x = bottomMiddleX; x < bottomRightX; x++)
            {
                Color rgb = perspectiveTexture.GetPixel(x, bottomY);
                float greyscaleValue = (rgb.r + rgb.g + rgb.b) / 3f;
                if (greyscaleValue <= greyscaleThreshold)
                {
                    greyscaleCount++;
                }
                hitTexture.SetPixel(x, bottomY, Color.green);
            }
            greyScalePercentageRight = (greyscaleCount / perspectiveTexture.width) * 100f;

            if (Math.Min(greyScalePercentageLeft, greyScalePercentageRight) >= 40f)
            {
                if (greyScalePercentageLeft > greyScalePercentageRight)
                {
                    PerformHit(ball, HIT.Left);
                }
                else
                {
                    PerformHit(ball, HIT.Right);
                }
            }
        }

        hitTexture.Apply();
        webcamDisplayCanvas.SetActive(true);
        GetComponentInChildren<RawImage>().texture = hitTexture;
        */




        //Texture2D hitTexture = new Texture2D((int)perspectiveTexture.height, (int)perspectiveTexture.width);
        Texture2D hitTexture = new Texture2D((int)webcamTexture.width, (int)webcamTexture.height);
        hitTexture.SetPixels(webcamTexture.GetPixels());
        for (int x = 0; x < testPixel.x; x++)
        {
            for (int y = 0; y < testPixel.y; y++)
            {
                hitTexture.SetPixel(x, y, Color.red);
                //hitTexture.SetPixel(y, x, Color.red);
            }
        }

        hitTexture.Apply();
        testPixel.x+=5; testPixel.y+=5;
        webcamDisplayCanvas.SetActive(true);
        GetComponentInChildren<RawImage>().texture = hitTexture;
    }
    private Vector2 testPixel = new Vector2(0, 0);

    private void ShowBallHit()
    {
        /*
        Texture2D hitTexture = new Texture2D((int)perspectiveTexture.width, (int)perspectiveTexture.height);
        Camera cam = GetComponentInParent<Camera>();
        List<GameObject> balls = GetComponent<FoodManager>().GetCurrentFoods();

        float allPixels = perspectiveTexture.width * perspectiveTexture.height;
        Color[] allGrey = new Color[(int)allPixels];
        for (int i = 0; i < allPixels; i++)
        {
            allGrey[i] = Color.grey;
        }
        hitTexture.SetPixels(0, 0, hitTexture.width, hitTexture.height, allGrey);


        foreach (GameObject ball in balls)
        {
            BoundingBox2D ballBounds = new BoundingBox2D(ball);

            //Vector2 topLeft = cam.WorldToScreenPoint(ballBounds.topLeft);
            Vector2 topLeft = cam.WorldToScreenPoint(ballBounds.bottomLeft);
            topLeft.y = cam.pixelHeight - topLeft.y;
            //Vector2 bottomRight = cam.WorldToScreenPoint(ballBounds.bottomRight);
            Vector2 bottomRight = cam.WorldToScreenPoint(ballBounds.topRight);
            bottomRight.y = cam.pixelHeight - bottomRight.y;

            //int x = (int)topLeft.x;
            int x = (int)CameraUtils.MapScreenToCamera(topLeft.x, cam.pixelWidth, 640f);
            int y = (int)CameraUtils.MapScreenToCamera(topLeft.y, cam.pixelHeight, 360f);
            int width = (int)CameraUtils.MapScreenToCamera(bottomRight.x - topLeft.x, cam.pixelWidth, 640f);
            int height = (int)CameraUtils.MapScreenToCamera(bottomRight.y - topLeft.y, cam.pixelHeight, 360f);

            if (x + width > 640)
            {
                width = 640 - x;
            }
            else if (x < 0)
            {
                x = 0;
            }

            if (y + height > 360)
            {
                height = 360 - y;
            }
            else if (y < 0)
            {
                y = 0;
            }


            Debug.Log("ball at " + x + "," + y + " with size " + width + "," + height);

            // NOT SETTING PIXELS CORRECTLY

            float ballPixels = width * height;
            Color[] ballBlue = new Color[(int)ballPixels];
            
            for (int j = 0; j < ballPixels; j++)
            {
                ballBlue[j] = Color.blue;
            }
            hitTexture.SetPixels(x, y, width, height, ballBlue);
            
        }
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
        perspectiveTexture = CameraUtils.ConvertMatToTexture2D(perspectiveMat);

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
        food.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        food.GetComponent<Rigidbody2D>().AddForce(Vector2.up * hitAmount, ForceMode2D.Impulse);
    }






    private void SetupPerspectiveShift()
    {
        // Debug.Log("adding point " + perspectiveIndex);

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
