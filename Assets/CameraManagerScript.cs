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

    public void SwitchToBallChaseCamera()
    {
        // Turn off all cameras?
        ballChaseCamera.GetComponent<Camera>().enabled = true;
    }
}
