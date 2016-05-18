using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    // scripts
    private PigPongABCD gameInput;

    // graphics
    public GameObject titleSign;
    //private int titleSignTweenId = -1;
    private bool titleSignUp = false;


	// Use this for initialization
	void Start ()
    {
        gameInput = GetComponent<PigPongABCD>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        HandleInput();

        //float framerate = 1.0f / Time.deltaTime;
        //Debug.Log("framerate: " + framerate);
    }

    void Awake()
    {
        
    }


    void OnApplicationQuit()
    {
        StopDroppingFood();
    }



    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // close application
        {
            Application.Quit();
        }

        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            GUI.enabled = !GUI.enabled;
            Debug.Log("enabled?" + GUI.enabled);
        }

        else if (Input.GetKeyDown(KeyCode.Space)) // show the webcam image when spacebar is pressed
        {
            gameInput.ToggleVideoFeed();
        }

        else if (Input.GetKeyDown(KeyCode.L))
        {
            GetComponentInParent<DebugConsole>().visible = !GetComponentInParent<DebugConsole>().visible;
        }





        else if (Input.GetKeyDown(KeyCode.Z))
        {
            gameInput.StartCalibration();
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            StopDroppingFood();
            MoveTitleSignDown();
            gameInput.mode = PigPongABCD.MODE.Screen;
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            gameInput.mode = PigPongABCD.MODE.Ball;
            StartDroppingFood();
        }



        else if (Input.GetKeyDown(KeyCode.Q))
        {
            StartDroppingFood();
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            titleSignUp = !titleSignUp;

            if (titleSignUp)
            {
                MoveTitleSignUp();
            }
            else
            {
                MoveTitleSignDown();
            }
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            StopDroppingFood();
            gameInput.mode = PigPongABCD.MODE.None;
        }





        else if (Input.GetKeyDown(KeyCode.A))
        {
            gameInput.ShowCalibratedImage();
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            //StartDroppingFood();
            gameInput.mode = PigPongABCD.MODE.Test;
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            gameInput.ShowRigidBodyLines();
        }


        else if (Input.GetKeyDown(KeyCode.N))
        {
            gameInput.greyscaleThreshold -= 0.02f;
            Debug.Log("greyscaleThreshold: " + gameInput.greyscaleThreshold);
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            gameInput.greyscaleThreshold += 0.02f;
            Debug.Log("greyscaleThreshold: " + gameInput.greyscaleThreshold);
        }

        else if (Input.GetKeyDown(KeyCode.J))
        {
            gameInput.greyscalePixelCount -= 1f;
            Debug.Log("greyscalePixelCount: " + gameInput.greyscalePixelCount);
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            gameInput.greyscalePixelCount += 1f;
            Debug.Log("greyscalePixelCount: " + gameInput.greyscalePixelCount);
        }




        // calibrate camera position
        if (Input.GetMouseButtonDown(0))
        {
            gameInput.SetupPerspectiveShift();
        }
    }

    





    /*
    void OnGUI()
    {
        GUI.Box(new Rect(10, 10, 200, 90), "Game Settings");

        string inputGrayscale = "0.6";
        inputGrayscale = GUI.TextField(new Rect(20, 40, 180, 20), inputGrayscale);
        if (GUI.changed)
        {
            gameInput.greyscaleThreshold = float.Parse(inputGrayscale);
        }
    }
    */




    

    /// <summary>
    /// When a food hits the ground
    /// </summary>
    void OnCollisionEnter2d()
    {

    }






    public void StartDroppingFood()
    {
        Debug.Log("DROPPING FOOD");
        GetComponent<FoodManager>().dropFood = true;
    }

    public void StopDroppingFood()
    {
        Debug.Log("STOP DROPPING FOOD");
        GetComponent<FoodManager>().dropFood = false;
    }


    public void MoveTitleSignUp()
    {
        //titleSignTweenId = LeanTween.move(titleSign, new Vector2(0.45f, 10.5f), 0.75f).setEase(LeanTweenType.easeInBack).id;
        LeanTween.move(titleSign, new Vector2(0.45f, 10.5f), 0.75f).setEase(LeanTweenType.easeInBack);
    }

    public void MoveTitleSignDown()
    {
        LeanTween.move(titleSign, new Vector2(0.14f, 2.64f), 0.75f).setEase(LeanTweenType.easeOutBack);
    }


}
