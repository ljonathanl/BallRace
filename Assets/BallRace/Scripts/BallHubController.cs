using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHubController : MonoBehaviour
{
    public BallController ballController; 

    // Update is called once per frame
    void Update()
    {
        transform.position = ballController.transform.position + Vector3.up * 10;
        transform.rotation = Quaternion.Euler(0, ballController.rotation, 0);
    }
}
