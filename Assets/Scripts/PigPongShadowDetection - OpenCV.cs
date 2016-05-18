using UnityEngine;
using System.Collections;

using OpenCVForUnity;
using System.Collections.Generic;
using System;

public class PigPongShadowDetectionOpenCV : MonoBehaviour {

    private const int CAMERA_WIDTH = 640;
    private const int CAMERA_HEIGHT = 360;

    private const int MAX_BALLS = 5;


    public int personLineHeight = 252;

    private Texture displayTexture;


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


    public int ballWidth = 250;
    public int ballHeight = 250;

    private int personThreshold = 40;
    private int greyscaleThreshold = 125;
    private int greyscalePixelCount = 400;


    private bool calibrated = true;
    private OpenCVForUnity.Rect calibrationFrame;
    private int calibrationOffset = 40;

    public float a, b, c, d, e, f, g, h;

    public bool running = true;

    private MODE mode = MODE.None;



    private WebCamTexture webcamTexture;

    public float updateSeconds = 0.3f;
    private float updateTimer = 0.0f;

    public GameObject ball;




    // Use this for initialization
    void Start ()
    {
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();

        displayTexture = webcamTexture;
    }
	
	// Update is called once per frame
	void Update ()
    {
        updateTimer += Time.deltaTime;

        if (updateTimer >= updateSeconds)
        {
            DoShadowCalculation();

            updateTimer = 0;
        }

        RunTests();
    }


    

    private void DoShadowCalculation()
    {
        if (running)
        {
            switch (mode)
            {
                case MODE.Ball:
                    CalculateBallHits();
                    break;
                case MODE.Screen:
                    CalculateScreenMovement();
                    break;
            }

        }
    }


    private void CalculateBallHits()
    {
        int[] counts = new int[2] { 0, 0 };

        float x, y;
        BoundingBox2D boundingBox = new BoundingBox2D(ball);
        float newDx, newDy;
        int newX, newY;
        

        for (y = boundingBox.middle.y; y < boundingBox.bottomLeft.y; y++)
        {
            for (x = boundingBox.bottomLeft.x; x < boundingBox.middle.x; x++)
            {
                newDx = ((a * x) + (b * y) + c) / ((g * x) + (h * y) + 1);
                newDy = ((d * x) + (e * y) + f) / ((g * x) + (h * y) + 1);
                newX = Convert.ToInt16(newDx);
                newY = Convert.ToInt16(newDy);

                if (newX >= 0 && newX >= 0 && newY < CAMERA_WIDTH && newY < CAMERA_HEIGHT)
                {
                    Color rgb = webcamTexture.GetPixel(newX, newY);
                    float greyscaleValue = (rgb.r + rgb.g + rgb.b) / 3f;
                    if (greyscaleValue < greyscaleThreshold)
                    {
                        counts[0]++;
                    }
                }
            }
        }


        for (y = boundingBox.middle.y; y < boundingBox.bottomRight.y; y++)
        {
            for (x = boundingBox.middle.x; x < boundingBox.bottomRight.x; x++)
            {
                newDx = ((a * x) + (b * y) + c) / ((g * x) + (h * y) + 1);
                newDy = ((d * x) + (e * y) + f) / ((g * x) + (h * y) + 1);
                newX = Convert.ToInt16(newDx);
                newY = Convert.ToInt16(newDy);

                if (newX >= 0 && newX >= 0 && newY < CAMERA_WIDTH && newY < CAMERA_HEIGHT)
                {
                    Color rgb = webcamTexture.GetPixel(newX, newY);
                    float greyscaleValue = (rgb.r + rgb.g + rgb.b) / 3f;
                    if (greyscaleValue < greyscaleThreshold)
                    {
                        counts[1]++;
                    }
                }
            }
        }


        if (counts[0] > greyscalePixelCount || counts[1] > greyscalePixelCount)
        {
            if (counts[0] >= counts[1])
            {
                PerformHit(HIT.Left);
            }
            else
            {
                PerformHit(HIT.Right);
            }
        }

    }

    private void CalculateScreenMovement()
    {

    }



    private Texture2D perspectiveTexture;
    private void ShiftPerspective()
    {
        // convert to mat
        Mat webcamMat = new Mat(this.webcamTexture.height, this.webcamTexture.width, CvType.CV_8UC4);
        Utils.webCamTextureToMat(this.webcamTexture, webcamMat);

        Mat perspectiveMat = CameraUtils.ShiftPerspective(webcamMat, perspectiveCoordinates[0], perspectiveCoordinates[1], perspectiveCoordinates[2], perspectiveCoordinates[3]);

        perspectiveTexture = CameraUtils.ConvertMatToTexture2D(perspectiveMat);

        displayTexture = perspectiveTexture;

        //byte[] buff = new byte[webcamMat.total() * webcamMat.channels()];
        //webcamMat.get(200, 200, buff);
        //byte twohundred = buff[0];
        //Console.WriteLine(twohundred.ToString());
    }






    private void PerformHit(HIT hitLocation)
    {
        
    }









    

    private void RunTests()
    {
        BoundingBox2D boundingBox = new BoundingBox2D(ball);

        Debug.DrawLine(boundingBox.topLeft, boundingBox.bottomLeft, Color.red);
        Debug.DrawLine(boundingBox.topLeft, boundingBox.topRight, Color.red);
        Debug.DrawLine(boundingBox.topRight, boundingBox.bottomRight, Color.red);
        Debug.DrawLine(boundingBox.bottomLeft, boundingBox.bottomRight, Color.red);

        if (doPerspectiveShift)
        {
            ShiftPerspective();
        }

        if (Input.GetMouseButtonDown(0) && !doPerspectiveShift)
        {
           // Debug.Log("adding point " + perspectiveIndex);

            Vector3 mousePosition = Input.mousePosition;
            Camera cam = GetComponentInParent<Camera>();
            Vector2 screenPosition = cam.WorldToScreenPoint(mousePosition);
            

            Vector2 cameraPosition = new Vector2(CameraUtils.MapScreenToCamera(mousePosition.x, Screen.width, 640f), 360f - CameraUtils.MapScreenToCamera(mousePosition.y, Screen.height, 360f));

            //Debug.Log("mouse position: " + mousePosition.x + ", " + mousePosition.y);
            //Debug.Log("screen Position: " + screenPosition.x + ", " + screenPosition.y);
            Debug.Log("camera position: " + cameraPosition.x + ", " + cameraPosition.y);

            perspectiveCoordinates[perspectiveIndex] = cameraPosition;
            perspectiveIndex++;

            if (perspectiveIndex == 4)
            {
                Debug.Log("doing perspective shift!");
                doPerspectiveShift = true;
            }
        }


        gameObject.GetComponent<GUITexture>().texture = displayTexture;

        //inputMat.release();
        //inputMat.Dispose();
    }



}
