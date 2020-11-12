using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXController : MonoBehaviour
{
    // public Model.Ball ball;

    public Model.Ball ball = Model.Game.instance.ball;
    public Model.Terrain terrain = Model.Game.instance.terrain;
    public ParticleSystem ballParticles;
    public ParticleSystem terrainParticles;

    public Light terrainLight;

    public Renderer blurRenderer;

    public Renderer haloRenderer;

    public float maxTerrainIntensity = 10;

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(ball.position);
        transform.position = ball.position;

        var emission = ballParticles.emission;
        emission.rateOverTime = (ball.velocity) * 10 * Remap(Mathf.Clamp(ball.distanceToGround, 1f, 2), 1f, 2, 1, 0);

        var shape = ballParticles.shape;
        shape.angle = Mathf.Max(5, 20 - ball.velocity / 3);

        var trails = ballParticles.trails;
        trails.lifetime = ball.velocity / 80;

        var main = ballParticles.main;
        main.startSpeedMultiplier = ball.velocity * 2;
        var startColor = main.startColor;
        startColor.color = ball.color;
        main.startColor = startColor;


        main = terrainParticles.main;
        startColor = main.startColor;
        startColor.color = ball.color;
        main.startColor = startColor;

        // blurRenderer.material.SetFloat("_offset", Remap(Mathf.Clamp(ballController.velocity, 10, 30), 10, 30, 0, 0.01f));

        haloRenderer.material.SetColor("_HaloColor", ball.color);
        haloRenderer.material.SetFloat("_HaloAlpha", Mathf.Lerp(haloRenderer.material.GetFloat("_HaloAlpha"), terrain.isOnColor ? 0 : 1f, Time.deltaTime * 3));

        terrainLight.color = terrain.currentColor;
        terrainLight.intensity = Remap(Mathf.Clamp(ball.distanceToGround, 1f, 3), 1f, 3, maxTerrainIntensity, 0);
    }
}
