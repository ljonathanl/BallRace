﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BB8Controller : MonoBehaviour
{
    public Model.Ball ball = Model.Game.instance.ball;

    public Model.Terrain terrain = Model.Game.instance.terrain;


    public RaceController raceController;

    public Transform bb8;


    public Renderer headRenderer;
    public Renderer headHaloRenderer;

    public Renderer lightRenderer;

    public Light spotLight;

    private Material headMaterial;
    private Material headHaloMaterial;

    private Vector3 startPosition;

    void Start() {
        headMaterial = headRenderer.materials[0];
        headHaloMaterial = headHaloRenderer.materials[0];
        startPosition = bb8.localPosition;
    }

    private float angle = 0;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = ball.position;
        headMaterial.color = ball.color;

        if (raceController.race.isRacing) {
            Vector3 relativePos = raceController.nextCheckPoint.transform.position - transform.position;
            Quaternion toRotation = Quaternion.LookRotation(relativePos);
            transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, Time.deltaTime);
            lightRenderer.gameObject.SetActive(false);
        } else {
            angle = (angle + Time.deltaTime * 100) % 360;
            transform.rotation = Quaternion.Euler(0, angle, 0); 
            lightRenderer.material.SetColor("_LightColor", ball.color);
            spotLight.color = ball.color;
            if (raceController.race.isNewRecord) {
                lightRenderer.gameObject.SetActive(true);
            }
        }

        headHaloMaterial.SetColor("_HaloColor", ball.color);
        headHaloMaterial.SetFloat("_HaloAlpha", Mathf.Lerp(headHaloMaterial.GetFloat("_HaloAlpha"), terrain.isOnColor ? 0 : 1f, Time.deltaTime * 3));

        bb8.localPosition = Vector3.Lerp(bb8.localPosition, startPosition + Vector3.up * Remap(Mathf.Clamp(ball.velocity, 0, 20), 0, 20, 0.1f, -0.1f), Time.deltaTime);

        // transform.LookAt(raceController.nextCheckPoint.transform.position); 
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }


}
