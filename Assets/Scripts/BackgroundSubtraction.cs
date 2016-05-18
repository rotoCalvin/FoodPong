using UnityEngine;
using System.Collections;
using OpenCVForUnity;
using System.Collections.Generic;

public class BackgroundSubtraction : MonoBehaviour {

    private Mat imag = null;
    private Mat origin = null;
    private Mat kalman = null;



    public double _learningRate = 0.0;

    public double _minBlobArea = 0.0;
    public double _maxBlobArea = 0.0;







    // Use this for initialization
    void Start () {

        PlayVideo();


    }
	
	// Update is called once per frame
	void Update () {

        
	
	}


    void PlayVideo()
    {
        Mat inFrame = new Mat();
        Mat outbox = new Mat();
        Mat diffFrame = null;
        List<OpenCVForUnity.Rect> array = new List<OpenCVForUnity.Rect>();

        BackgroundSubtractorMOG2 mBGSub = Video.createBackgroundSubtractorMOG2();
        // dt, accel noise mag, dist thres, maximum allowed skipped frames, max trace length

        VideoCapture camera = new VideoCapture(0);
        //camera.open(camera-name);

        int i = 0;

        if (!camera.isOpened())
        {
            Debug.Log("Can not open Camera, try it later.");
            return;
        }

        while (true)
        {
            if (!camera.read(inFrame))
            {
                break;
            }
            //Imgproc.resize(inFrame, inFrame, new Size(640, 360), 0.0, 0.0, Imgproc.INTER_LINEAR);

            imag = inFrame.clone();
            origin = inFrame.clone();

            if (i == 0)
            {
                diffFrame = new Mat(outbox.size(), CvType.CV_8UC1);
                diffFrame = outbox.clone();
            }
            else if (i == 1)
            {
                diffFrame = new Mat(inFrame.size(), CvType.CV_8UC1);
                ProcessFrame(camera, inFrame, diffFrame, mBGSub);
                inFrame = diffFrame.clone();

                array = DetectionContours(diffFrame);

                List<Point> detections = new List<Point>();

                foreach (OpenCVForUnity.Rect contour in array)
                {
                    int objectCenterX = (int)((contour.tl().x + contour.br().x) / 2);
                    int objectCenterY = (int)((contour.tl().y + contour.br().y) / 2);

                    Point p = new Point(objectCenterX, objectCenterY);
                    detections.Add(p);
                }
            }


            if (array.Count > 0)
            {
                foreach (OpenCVForUnity.Rect contour in array)
                {
                    int objectCenterX = (int)((contour.tl().x + contour.br().x) / 2);
                    int objectCenterY = (int)((contour.tl().y + contour.br().y) / 2);
                    Point p = new Point(objectCenterX, objectCenterY);

                    Imgproc.rectangle(imag, contour.br(), contour.tl(), new Scalar(0, 255, 0), 2);
                    Imgproc.circle(imag, p, 1, new Scalar(0, 0, 255), 2);

                }
            }


            Texture2D outputTexture = ConvertMatToTexture2D(imag);

            // apply the texture to the on-screen component
            gameObject.GetComponent<GUITexture>().texture = outputTexture;


        }
    }




    // FUNCTIONS


    protected void ProcessFrame(VideoCapture capture, Mat mRgba, Mat mFGMask, BackgroundSubtractorMOG2 mBGSub)
    {
        mBGSub.apply(mRgba, mFGMask, _learningRate);
        Imgproc.cvtColor(mFGMask, mRgba, Imgproc.COLOR_GRAY2BGRA, 0);
        Mat erode = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(8, 8));
        Mat dilate = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(8, 8));

        Mat openElem = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(3, 3), new Point(1, 1));
        Mat closeElem = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(7, 7), new Point(3, 3));

        Imgproc.threshold(mFGMask, mFGMask, 127, 255, Imgproc.THRESH_BINARY);
        Imgproc.morphologyEx(mFGMask, mFGMask, Imgproc.MORPH_OPEN, erode);
        Imgproc.morphologyEx(mFGMask, mFGMask, Imgproc.MORPH_OPEN, dilate);
        Imgproc.morphologyEx(mFGMask, mFGMask, Imgproc.MORPH_OPEN, openElem);
        Imgproc.morphologyEx(mFGMask, mFGMask, Imgproc.MORPH_CLOSE, closeElem);
    }


    public List<OpenCVForUnity.Rect> DetectionContours(Mat outmat)
    {
        Mat v = new Mat();
        Mat vv = outmat.clone();
        List<MatOfPoint> contours = new List<MatOfPoint>();
        Imgproc.findContours(vv, contours, v, Imgproc.RETR_LIST, Imgproc.CHAIN_APPROX_SIMPLE);

        int maxAreaIdx = -1;
        OpenCVForUnity.Rect r = null;
        List<OpenCVForUnity.Rect> rect_array = new List<OpenCVForUnity.Rect>();

        for (int idx = 0; idx < contours.Count; idx++)
        {
            Mat contour = contours[idx];
            double contourArea = Imgproc.contourArea(contour);
            if (contourArea > _minBlobArea && contourArea < _maxBlobArea)
            {
                maxAreaIdx = idx;
                r = Imgproc.boundingRect(contours[maxAreaIdx]);
                rect_array.Add(r);
            }
        }

        v.release();
        return rect_array;
    }


    private Texture2D ConvertMatToTexture2D(Mat inputMat)
    {
        // create an empty texture for the output to use
        Texture2D outputTexture = new Texture2D(inputMat.cols(), inputMat.rows(), TextureFormat.RGBA32, false);

        // convert to a texture
        Utils.matToTexture2D(inputMat, outputTexture);

        return outputTexture;
    }


}
