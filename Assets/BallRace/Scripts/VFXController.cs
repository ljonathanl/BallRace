using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    public GameObject ball;

    private BallController ballController;
    private ParticleSystem particle;
    public float yRotationOffset = 0;

    void Start()
    {
        ballController = ball.GetComponent<BallController>();

        particle = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(ball.transform.position);
        transform.position = ball.transform.position;

        var emission = particle.emission;
        emission.rateOverTime = ballController.velocity / 8;

        var main = particle.main;
        main.startSpeedMultiplier = ballController.velocity;
        var startColor = main.startColor;
        startColor.colorMax = ballController.color;
        main.startColor = startColor;
    }
}
