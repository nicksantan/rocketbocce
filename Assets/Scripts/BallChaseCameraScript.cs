using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallChaseCameraScript : MonoBehaviour
{

    public GameObject targetBall;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (targetBall)
            Debug.Log("chasing");
        {
            transform.position = new Vector3(targetBall.transform.position.x, targetBall.transform.position.y + 1.47f, targetBall.transform.position.z - 1.44f);
        }
    }
}
