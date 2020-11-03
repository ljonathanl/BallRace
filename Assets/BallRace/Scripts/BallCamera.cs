using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCamera : MonoBehaviour
{

    public GameObject ball;

    Vector3 offset;
    private BallController ballController;

    private Camera currentCamera;


    // Start is called before the first frame update
    void Start()
    {
        offset = transform.position - ball.transform.position;
        ballController = ball.GetComponent<BallController>();
        currentCamera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = ball.transform.position + Quaternion.Euler(0, ballController.rotation, 0) * offset;
        //transform.LookAt(ball.transform);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, ballController.rotation, transform.rotation.eulerAngles.z);
        currentCamera.fieldOfView = Mathf.Lerp(currentCamera.fieldOfView, 60 + Mathf.Floor(ballController.velocity / 5) * 20, Time.deltaTime);
    }
}
