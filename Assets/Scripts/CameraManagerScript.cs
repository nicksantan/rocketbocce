using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManagerScript : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject ballChaseCamera;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReturnToServingCamera(GameObject whichBall){
        ballChaseCamera.GetComponent<Camera>().enabled = false;
        // Is more needed here?
    }
    public void SwitchToBallChaseCamera(GameObject whichBall)
    {
        // TODO: Turn off all cameras?
        ballChaseCamera.GetComponent<BallChaseCameraScript>().targetBall = whichBall;
        ballChaseCamera.GetComponent<Camera>().enabled = true;
    }
}
