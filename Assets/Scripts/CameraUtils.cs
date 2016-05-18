using UnityEngine;
using System.Collections;
using OpenCVForUnity;
using System.Collections.Generic;
using System.IO;

public class CameraUtils
{
    public static Mat ShiftPerspective(Mat inputMat, Vector2 topLeft, Vector2 topRight, Vector2 bottomLeft, Vector2 bottomRight)
    {
        Mat src_mat = new Mat(4, 1, CvType.CV_32FC2);
        Mat dst_mat = new Mat(4, 1, CvType.CV_32FC2);

        // create the transform
        //src_mat.put(0, 0, topLeft.y, topLeft.x, topRight.y, topRight.x, bottomLeft.y, bottomLeft.x, bottomRight.y, bottomRight.x);
        src_mat.put(0, 0, topLeft.x, topLeft.y, topRight.x, topRight.y, bottomLeft.x, bottomLeft.y, bottomRight.x, bottomRight.y);
        dst_mat.put(0, 0, 0.0, 0.0, inputMat.rows(), 0.0, 0.0, inputMat.cols(), inputMat.rows(), inputMat.cols());
        Mat perspectiveTransform = Imgproc.getPerspectiveTransform(src_mat, dst_mat);

        // instantiate somewhere for the output image to go
        Mat outputMat = inputMat.clone();

        // warp the image using the transform
        Imgproc.warpPerspective(inputMat, outputMat, perspectiveTransform, new Size(inputMat.rows(), inputMat.cols()));

        return outputMat;
    }


    public static Texture2D ConvertMatToTexture2D(Mat inputMat)
    {
        // create an empty texture for the output to use
        Texture2D outputTexture = new Texture2D(inputMat.cols(), inputMat.rows(), TextureFormat.RGBA32, false);

        // convert to a texture
        Utils.matToTexture2D(inputMat, outputTexture);

        return outputTexture;
    }


    /* WORKS BUT UNUSED
    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }
    */


    /* GET PIXEL INFO

    double[] pixelInfo = webcamMat.get(yPos, xPos);

    */




    public static float MapScreenToCamera(float screenCoordinate, float screenSize, float cameraSize)
    {
        return (screenCoordinate * cameraSize) / screenSize;
    }




    public static float MapCameraToScreen(float cameraCoordinate, float cameraSize, float screenSize)
    {
        return (cameraCoordinate * screenSize) / cameraSize;
    }





}
