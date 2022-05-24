using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerControllerScript : MonoBehaviour
{
    private Player primaryPlayer;
    public GameObject theBall;
    private bool ballInFlight = false;

  

    public Transform aimTransform;

  
    // Start is called before the first frame update
    void Start()
    {
        primaryPlayer = ReInput.players.GetPlayer(0);
      //  aimDirection = new Vector3(0f, 1f, 1f);
    }

    // Update is called once per frame
    void Update() { 

       //Vector3 forward = transform.TransformDirection(aimDirection.normalized) * 100;
        Debug.DrawRay(new Vector3(0f, 0f, -6.8f), aimTransform.transform.forward, Color.green);
    
        if (primaryPlayer.GetButtonDown("Fire") && !ballInFlight)
        {
            Debug.Log("Firing");
            ballInFlight = true;
            // Gather all factors and apply the force to the ball
            theBall.GetComponent<Rigidbody>().AddForce(aimTransform.forward*800f);
            // Activate gravity on the ball
            theBall.GetComponent<Rigidbody>().useGravity = true;

        }

        if (primaryPlayer.GetButtonDown("Reset"))
        {
            Debug.Log("Resetting the ball");
            ballInFlight = false;
            // MOve it back to position
            theBall.transform.position = new Vector3(0f, 0f, -6.8f);
            //Turn off gravity and zero out velocity
            theBall.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            // Activate gravity on the ball
            theBall.GetComponent<Rigidbody>().useGravity = false;

        }

        GetMoveValue();
    }

    void GetMoveValue()
    {
        float x = primaryPlayer.GetAxis("AimX");
        float y = primaryPlayer.GetAxis("AimY");

        aimTransform.Rotate(y, x, 0f);
    }
}
