using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerControllerScript : MonoBehaviour
{
    private Player primaryPlayer;
    public GameObject theBall;
    private bool ballInFlight = false;

    // Reference to managers
    public GameObject cameraManager;
    public GameObject previewBallManager;

    public Transform aimTransform;
    public enum GamePhase { throwing, ballInFlight }
    public GamePhase currentPhase;

    public float currentPowerInput;

    public int frameCounter = 0;
    private bool firePreviewBall = false;
    private float maxFirePower = 500f;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0.33f;
        primaryPlayer = ReInput.players.GetPlayer(0);
        //  aimDirection = new Vector3(0f, 1f, 1f);

        Reset();
    }

    public void Reset()
    {
        cameraManager = GameObject.FindGameObjectWithTag("CameraManager");
        previewBallManager = GameObject.FindGameObjectWithTag("PreviewBallManager");
        currentPhase = GamePhase.throwing;
    }

    private void FixedUpdate()
    {
        frameCounter += 1;
        frameCounter = frameCounter % 5;
        if (frameCounter == 0)
        {
            firePreviewBall = true;
        }
    }
    // Update is called once per frame
    void Update() {

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
            currentPhase = GamePhase.throwing;

        }


        // Is the player throwing??
        if (currentPhase == GamePhase.throwing)
        {
            // Start calling preview balls and fire them at the current level!
           
            if (firePreviewBall && currentPowerInput > .15f)
            {
                GameObject previewBall = previewBallManager.GetComponent<PreviewBallManager>().GetBall();
                previewBall.transform.position = new Vector3(0f, 0f, -6.8f);
                previewBall.GetComponent<Rigidbody>().AddForce(aimTransform.forward * (maxFirePower * currentPowerInput));
                firePreviewBall = false;
            }
            //Vector3 forward = transform.TransformDirection(aimDirection.normalized) * 100;
            Debug.DrawRay(new Vector3(0f, 0f, -6.8f), aimTransform.transform.forward, Color.green);

            if (primaryPlayer.GetButtonDown("Fire") && !ballInFlight)
            {
                Debug.Log("Firing");
                ballInFlight = true;
                // Gather all factors and apply the force to the ball
                theBall.GetComponent<Rigidbody>().AddForce(aimTransform.forward * (maxFirePower * currentPowerInput));
                // Activate gravity on the ball
                theBall.GetComponent<Rigidbody>().useGravity = true;

                cameraManager.GetComponent<CameraManagerScript>().SwitchToBallChaseCamera();
                currentPhase = GamePhase.ballInFlight;
                previewBallManager.GetComponent<PreviewBallManager>().RemovePreviews();
                Time.timeScale = .7f;

            }

          
            GetMoveValue();
            GetPowerValue();
            Debug.Log(currentPowerInput);
        }

        if (currentPhase == GamePhase.ballInFlight)
        {

        }
    }

    void GetPowerValue()
    {
        currentPowerInput = primaryPlayer.GetAxis("Power");
    }

    void GetMoveValue()
    {
        float x = primaryPlayer.GetAxis("AimX");
        float y = primaryPlayer.GetAxis("AimY");

        aimTransform.Rotate(y, x, 0f);
    }
}
