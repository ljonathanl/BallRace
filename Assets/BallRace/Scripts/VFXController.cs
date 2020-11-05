using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    public GameObject ball;

    private BallController ballController;
    private TerrainController terrainController;
    public ParticleSystem ballParticles;
    public ParticleSystem terrainParticles;

    public Light terrainLight;

    public Renderer blurRenderer;

    public Renderer haloRenderer;

    public float maxTerrainIntensity = 10;

    void Start()
    {
        ballController = ball.GetComponent<BallController>();
        terrainController = ball.GetComponent<TerrainController>();
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

        var emission = ballParticles.emission;
        emission.rateOverTime = (ballController.velocity) * 10 * Remap(Mathf.Clamp(ballController.distanceToGround, 1f, 2), 1f, 2, 1, 0);

        var shape = ballParticles.shape;
        shape.angle = Mathf.Max(5, 20 - ballController.velocity / 3);

        var trails = ballParticles.trails;
        trails.lifetime = ballController.velocity / 80;

        var main = ballParticles.main;
        main.startSpeedMultiplier = ballController.velocity * 2;
        var startColor = main.startColor;
        startColor.color = ballController.color;
        main.startColor = startColor;


        main = terrainParticles.main;
        startColor = main.startColor;
        startColor.color = ballController.color;
        main.startColor = startColor;

        // blurRenderer.material.SetFloat("_offset", Remap(Mathf.Clamp(ballController.velocity, 10, 30), 10, 30, 0, 0.01f));

        haloRenderer.material.SetColor("_HaloColor", ballController.color);
        haloRenderer.material.SetFloat("_HaloAlpha", Mathf.Lerp(haloRenderer.material.GetFloat("_HaloAlpha"), terrainController.isOnColor ? 0 : 1f, Time.deltaTime * 3));

        terrainLight.color = terrainController.currentColor;
        terrainLight.intensity = Remap(Mathf.Clamp(ballController.distanceToGround, 1f, 3), 1f, 3, maxTerrainIntensity, 0);
    }
}
