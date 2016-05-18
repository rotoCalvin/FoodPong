using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using OpenCVForUnity;


public class PerspectiveCorrection : MonoBehaviour {

    public Vector2 topLeft;
    public Vector2 topRight;
    public Vector2 bottomLeft;
    public Vector2 bottomRight;

    public int threshLow = 0, threshHigh = 255;


    private WebCamTexture webcamTexture;
    private Texture2D outputTexture;



    public float updateSeconds = 0.3f;
    private float timer = 0.0f;





    // Use this for initialization
    void Start()
    {
        webcamTexture = new WebCamTexture();
        webcamTexture.Play();
        //gameObject.GetComponent<GUITexture>().texture = webcamTexture;   
    }



    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= updateSeconds)
        {
            TakeSnapshot();

            timer = 0;
        }
    }






    


    void TakeSnapshot()
    {
        Debug.Log("WebcamTexture", this.webcamTexture);
        Debug.Log("webcamTexture size " + this.webcamTexture.height + "," + this.webcamTexture.width);
        Mat inputMat = new Mat(this.webcamTexture.height, this.webcamTexture.width, CvType.CV_8UC4);

        Utils.webCamTextureToMat(this.webcamTexture, inputMat);
        Debug.Log("inputMat dst ToString " + inputMat.ToString());

        // greyscale image
        ConvertToGreyscale(inputMat);

        // threshhold (make bitmap)
        ConvertToBitmap(inputMat);

        /*
        // shift perspective
        Mat outputMat = ShiftPerspective(inputMat);

        // convert to texture
        Texture2D outputTexture = ConvertMatToTexture2D(outputMat);
        */

        // REMOVE:  USES BITMAP AS OUTPUT INSTEAD OF PERSPECTIVE SHIFT
        outputTexture = ConvertMatToTexture2D(inputMat);

        // apply the texture to the on-screen component
        gameObject.GetComponent<GUITexture>().texture = outputTexture;

        inputMat.release();
        inputMat.Dispose();
        //outputMat.Dispose();
    }



    private void ConvertToGreyscale(Mat inputMat)
    {
        Imgproc.cvtColor(inputMat, inputMat, Imgproc.COLOR_BGR2GRAY);
    }


    private void ConvertToBitmap(Mat inputMat)
    {
        //Imgproc.threshold(inputMat, inputMat, threshLow, threshHigh, Imgproc.THRESH_BINARY | Imgproc.THRESH_OTSU);
        Imgproc.adaptiveThreshold(inputMat, inputMat, 140, Imgproc.ADAPTIVE_THRESH_GAUSSIAN_C, Imgproc.THRESH_BINARY, 11, 4);
    }


    private Mat ShiftPerspective(Mat inputMat)
    {
        Mat src_mat = new Mat(4, 1, CvType.CV_32FC2);
        Mat dst_mat = new Mat(4, 1, CvType.CV_32FC2);


        // jpeg pixels
        // 183,14  433,109
        // 184,282 447,268

        // gameplay pixels
        // 130,10  472,145
        // 130,380 490,360

        // create the transform
        src_mat.put(0, 0, topLeft.x, topLeft.y, topRight.x, topRight.y, bottomLeft.x, bottomLeft.y, bottomRight.x, bottomRight.y);
        dst_mat.put(0, 0, 0.0, 0.0, inputMat.rows(), 0.0, 0.0, inputMat.cols(), inputMat.rows(), inputMat.cols());
        Mat perspectiveTransform = Imgproc.getPerspectiveTransform(src_mat, dst_mat);

        // instantiate somewhere for the output image to go
        Mat outputMat = inputMat.clone();

        // warp the image using the transform
        Imgproc.warpPerspective(inputMat, outputMat, perspectiveTransform, new Size(inputMat.rows(), inputMat.cols()));

        return outputMat;
    }


    private Texture2D ConvertMatToTexture2D(Mat inputMat)
    {
        // create an empty texture for the output to use
        Texture2D outputTexture = new Texture2D(inputMat.cols(), inputMat.rows(), TextureFormat.RGBA32, false);

        // convert to a texture
        Utils.matToTexture2D(inputMat, outputTexture);

        return outputTexture;
    }










    void OnGUI()
    {
        if (GUI.Button(new UnityEngine.Rect(10, 70, 50, 30), "Snap"))
        {
            TakeSnapshot();
        }
    }


}
