using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHubController : MonoBehaviour
{
    public Model.Ball ball = Model.Game.instance.ball; 

    // Update is called once per frame
    void Update()
    {
        transform.position = ball.position + Vector3.up * 10;
        transform.rotation = Quaternion.Euler(0, ball.rotation, 0);
    }
}
