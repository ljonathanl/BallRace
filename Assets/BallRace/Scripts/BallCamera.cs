using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCamera : MonoBehaviour
{

    public Model.Ball ball = Model.Game.instance.ball;

    public Vector3 offset;

    private Camera currentCamera;


    // Start is called before the first frame update
    void Start()
    {
        currentCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = ball.position + Quaternion.Euler(0, ball.rotation, 0) * offset;
        //transform.LookAt(ball.transform);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, ball.rotation, transform.rotation.eulerAngles.z);
        currentCamera.fieldOfView = Mathf.Lerp(currentCamera.fieldOfView, 60 + Mathf.Floor(ball.velocity / 5) * 20, Time.deltaTime);
    }
}
