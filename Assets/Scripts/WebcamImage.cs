using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WebcamImage : MonoBehaviour {


	// Use this for initialization
	void Start () {
        WebCamTexture webcamTexture = new WebCamTexture();
        webcamTexture.Play();
        gameObject.GetComponent<GUITexture>().texture = webcamTexture;
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
