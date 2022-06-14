using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewBallManager : MonoBehaviour
{
    public GameObject[] previewBalls;
    private int counter;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public GameObject GetBall()
    {
        GameObject nextBall = previewBalls[counter];
        // Clean the ball
        nextBall.GetComponent<Rigidbody>().velocity = Vector3.zero;
        counter += 1;
        counter = counter % previewBalls.Length;
        return nextBall;
    }

    public void RemovePreviews()
    {
        for (int i = 0; i < previewBalls.Length; i++)
        {
            previewBalls[i].SetActive(false);
        }
    }

    public void RestorePreviews()
    {
        for (int i = 0; i < previewBalls.Length; i++)
        {
            previewBalls[i].SetActive(true);
        }
    }
}
