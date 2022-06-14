using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerControllerScript : MonoBehaviour
{
    private Player primaryPlayer;
    public GameObject currentBall;
    public GameObject currentGoal;
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

    public GameObject[] team1Balls;
    public GameObject[] team2Balls;

    public GameObject[] allBalls;

    public Dictionary<GameObject, bool> team1BallRegistry;
    public Dictionary<GameObject, bool> team2BallRegistry;


    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0.33f;
        primaryPlayer = ReInput.players.GetPlayer(0);
        //  aimDirection = new Vector3(0f, 1f, 1f);
        team1BallRegistry = new Dictionary<GameObject, bool>();
        team2BallRegistry = new Dictionary<GameObject, bool>();

        SetUpNewRound();
    }

    public void Reset()
    {
        cameraManager = GameObject.FindGameObjectWithTag("CameraManager");
        previewBallManager = GameObject.FindGameObjectWithTag("PreviewBallManager");
        currentPhase = GamePhase.throwing;

    }
    public void SetUpNewRound()
    {

        // Reset ball registries
        team1BallRegistry.Add(team1Balls[0], false);
        team1BallRegistry.Add(team1Balls[1], false);
        team1BallRegistry.Add(team1Balls[2], false);
        team1BallRegistry.Add(team1Balls[3], false);

        team2BallRegistry.Add(team2Balls[0], false);
        team2BallRegistry.Add(team2Balls[1], false);
        team2BallRegistry.Add(team2Balls[2], false);
        team2BallRegistry.Add(team2Balls[3], false);

        // Display UI, do a little camera dance here, whatever...
        // Then, pick the first ball to shoot

        Reset();
        // For now, first ball is team 1, ball 1
        currentBall = team1Balls[0];
        PrepareNextBall();
    }

    public void PrepareNextBall()
    {
        Debug.Log("preparing the ball");
        currentBall.SetActive(true);
        ballInFlight = false;
        // MOve it back to position
        currentBall.transform.position = new Vector3(0f, 0f, -6.8f);
        //Turn off gravity and zero out velocity
        currentBall.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        // Activate gravity on the ball
        currentBall.GetComponent<Rigidbody>().useGravity = false;
        currentPhase = GamePhase.throwing;

        // Tell the camera to follow this ball now
        cameraManager.GetComponent<CameraManagerScript>().ReturnToServingCamera(currentBall);
    }
    bool GetBallStatus(GameObject whichBall, int whichTeam)
    {
        switch (whichTeam)
        {
            case 1:
                return team1BallRegistry[whichBall];
                break;
            case 2:
                return team2BallRegistry[whichBall];
                break;
            default:
                return false;
        }

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
    void Update()
    {

        if (primaryPlayer.GetButtonDown("Reset"))
        {
            PrepareNextBall();
            previewBallManager.GetComponent<PreviewBallManager>().RestorePreviews();

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
                currentBall.GetComponent<Rigidbody>().AddForce(aimTransform.forward * (maxFirePower * currentPowerInput));
                // Activate gravity on the ball
                currentBall.GetComponent<Rigidbody>().useGravity = true;

                //Update which ball the ball chase cam should be watching
                cameraManager.GetComponent<CameraManagerScript>().SwitchToBallChaseCamera(currentBall);
                currentPhase = GamePhase.ballInFlight;
                Invoke("FindNextBall", 5f);
                previewBallManager.GetComponent<PreviewBallManager>().RemovePreviews();
                Time.timeScale = .7f;
            }


            GetMoveValue();
            GetPowerValue();
            // Debug.Log(currentPowerInput);
        }

        if (currentPhase == GamePhase.ballInFlight)
        {
            // TODO: Tell if the ball has stopped.... or time out to make it stop, or both?
            // For now, just wait a sec and then assign a new ball

        }
    }

    void FindNextBall()
    {


        // TODO: Need to calculate distance to the goal here... 


        // For the first ball, just take the other team's first ball
        if (currentBall == team1Balls[0])
        {
            Debug.Log("the current ball is the first ball, so just set current ball to the first ball of the other team");
            currentBall = team2Balls[0];
        }
        else
        {
            Debug.Log("Finding closest ball...");
            int nextTeam = 0;
            GameObject closestBall = null;
            float closestDistanceSqr = Mathf.Infinity;
            Vector3 goalPosition = currentGoal.transform.position;
            foreach (GameObject ball in allBalls)
            {
                if (ball.active)
                {
                    Vector3 directionToTarget = goalPosition - ball.transform.position;
                    float dSqrToTarget = directionToTarget.sqrMagnitude;
                    if (dSqrToTarget < closestDistanceSqr)
                    {
                        Debug.Log("Distance is...");
                        Debug.Log(dSqrToTarget);
                        closestDistanceSqr = dSqrToTarget;
                        closestBall = ball;
                    }
                }
            }
            if (closestBall.tag == "TeamOne")
            {
                Debug.Log("TEAM ONE IS CLOSER!");
                nextTeam = 2;
            }
            if (closestBall.tag == "TeamTwo")
            {
                Debug.Log("TEAM TWO IS CLOSER");
                nextTeam = 1;
            }

            // First make sure there are balls left
            if (AreAnyBallsLeft())
            {
                Debug.Log("Some balls are left so we will continue!");
                // Continue game
                switch (nextTeam)
                {
                    case 1:
                        if (AreAnyTeamBallsLeft(1))
                        {
                            // get a new team 1 ball
                           currentBall = GetNextTeamBall(1);
                        }
                        else
                        {
                            // get a new team 2 ball
                           currentBall = GetNextTeamBall(2);
                        }
                        break;

                    case 2:
                        if (AreAnyTeamBallsLeft(2))
                        {
                            // get a new team 2 ball
                            currentBall = GetNextTeamBall(2);
                        }
                        else
                        {
                            // get a new team 1 ball
                           currentBall =  GetNextTeamBall(1);
                        }
                        break;

                    default:

                        break;
                }
            }
            else
            {
                Debug.Log("GAME OVER");
            }
        }




        PrepareNextBall();
    }

    bool AreAnyTeamBallsLeft(int whichTeam)
    {
        switch (whichTeam)
        {
            case 1:
                foreach (GameObject ball in team1Balls)
                {
                    if (!ball.activeSelf)
                    {
                        return true;
                    }
                }
                return false;
            case 2:
                foreach (GameObject ball in team2Balls)
                {
                    if (!ball.activeSelf)
                    {
                        return true;
                    }
                }
                return false;
            default:
                return false;
        }
    }
    bool AreAnyBallsLeft()
    {
        foreach (GameObject ball in team1Balls)
        {
            if (!ball.activeSelf)
            {
                return true;
            }
        }
        foreach (GameObject ball in team2Balls)
        {
            if (!ball.activeSelf)
            {
                return true;
            }
        }
        return false;
    }

    GameObject GetNextTeamBall(int whichTeam)
    {
        switch (whichTeam)
        {
            case 1:
                foreach (GameObject ball in team1Balls)
                {
                    if (!ball.activeSelf)
                    {
                        return ball;
                    }
                }

                break;
            case 2:
                foreach (GameObject ball in team2Balls)
                {
                    if (!ball.activeSelf)
                    {
                        return ball;
                    }
                }
                break;
            default:
                // TODO how can I change this?
                return team1Balls[0];
                break;
        }
        // TODO I don't like this!
         return team1Balls[0];
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
