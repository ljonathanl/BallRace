using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    public GameObject ball;

    private BallController ballController;
    private ParticleSystem particle;

    public Renderer blurRenderer;

    void Start()
    {
        ballController = ball.GetComponent<BallController>();

        particle = GetComponentInChildren<ParticleSystem>();
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(ball.transform.position);
        transform.position = ball.transform.position;

        var emission = particle.emission;
        emission.rateOverTime = (ballController.velocity) * 10 * Remap(Mathf.Clamp(ballController.distanceToGround, 1f, 2), 1f, 2, 1, 0);

        var shape = particle.shape;
        shape.angle = Mathf.Max(5, 20 - ballController.velocity / 3);

        var trails = particle.trails;
        trails.lifetime = ballController.velocity / 80;

        var main = particle.main;
        main.startSpeedMultiplier = ballController.velocity * 2;
        var startColor = main.startColor;
        startColor.colorMax = ballController.color;
        main.startColor = startColor;

        blurRenderer.material.SetFloat("_offset", Remap(Mathf.Clamp(ballController.velocity, 10, 40), 10, 40, 0, 0.02f));
    }
}
