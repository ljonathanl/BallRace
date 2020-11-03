﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainController : MonoBehaviour
{

    public Terrain terrain;

    private float[,,] element;

    public float isOnCircuit;
    public bool isOnColor;
    private int mapX;
    private int mapY;
    private TerrainData terrainData;
    private Vector3 terrainPosition;
    private BallController ballController;
    private float[,,] alphaMaps;
    private float[,] heightMap;
    private Rigidbody ballRigidBody;
    private Vector3 lastPos;
    private bool isOnZone;
    public float MinDistance = 0.1f;

    public float floorFriction = 0.8f;
    public float zoneFriction = 0.5f;

    public float colorBonus = 0.2f;

    public int TraceSize = 3;
    private float lastBonusSpeed;

    private bool isReliefEnabled;

    void Awake()
    {
        terrainData = terrain.terrainData;
        lastPos = transform.position;
    }

    void Start()
    {
        terrainPosition = terrain.transform.position;
        ballController = GetComponent<BallController>();
        alphaMaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);

        ballRigidBody = ballController.GetComponent<Rigidbody>();

        Debug.Log("Size alphas: " + (terrainData.alphamapWidth * terrainData.alphamapHeight * terrainData.alphamapLayers) 
            + " = " + terrainData.alphamapWidth + " x " + terrainData.alphamapHeight + " x " + terrainData.alphamapLayers
            + " | heights " + (terrainData.heightmapResolution * terrainData.heightmapResolution) 
            + " = " + terrainData.heightmapResolution + "²");

        // for (int i = 0; i < TraceSize * 2; i++)
        // {
        //     for (int j = 0; j < TraceSize * 2; j++)
        //     {
        //         var alpha = 1 - (float) Mathf.Abs((TraceSize - i) + (TraceSize - j)) / (2 * TraceSize); 
        //         Debug.Log(" " + i + " " + j + " " + alpha);
        //     }
        // }
    }

    public void Reset() {
        terrainData.SetAlphamaps(0, 0, alphaMaps);
        if (isReliefEnabled)
            terrainData.SetHeights(0, 0, heightMap);
    }

    public void EnableRelief(bool value) 
    {
        if (value) {
            terrainData.SetHeights(0, 0, heightMap);
        } else {
            var map = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];
            for (int i = 0; i < terrainData.heightmapResolution; i++)
            {
                for (int j = 0; j < terrainData.heightmapResolution; j++)
                {
                    map[i, j] = 0; 
                }
            }
            terrainData.SetHeights(0, 0, map);
        }
        isReliefEnabled = value;
    }

    void OnDestroy()
    {
        isReliefEnabled = true;
        Reset();
    }

    void LateUpdate()
    {
        var bonusSpeed = 0f;
        // isOnCircuit = 0;
        // isOnZone = false;
        // only update if we move
        if (ballController.distanceToGround < 1)
        {
            
            if (Vector3.Distance(transform.position, lastPos) > MinDistance) {
                // convert world coords to terrain coords
                mapX = (int)(((transform.position.x - terrainPosition.x) / terrainData.size.x) * terrainData.alphamapWidth);
                mapY = (int)(((transform.position.z - terrainPosition.z) / terrainData.size.y) * terrainData.alphamapHeight);

                if (mapX > TraceSize && mapX < (terrainData.alphamapWidth - TraceSize) && mapY > TraceSize && mapY < (terrainData.alphamapHeight - TraceSize)) {
                    element = terrainData.GetAlphamaps(mapX - TraceSize, mapY - TraceSize, TraceSize * 2, TraceSize * 2);
                    isOnCircuit = element[TraceSize, TraceSize, 1];
                    isOnColor = (element[TraceSize, TraceSize, 2] + element[TraceSize, TraceSize, 3] + element[TraceSize, TraceSize, 4]) > 0;
                    bonusSpeed = (isOnCircuit - 1) * zoneFriction + (isOnColor ? colorBonus : 0);
                    
                    for (int i = 0; i < TraceSize * 2; i++)
                    {
                        for (int j = 0; j < TraceSize * 2; j++)
                        {
                            var alpha = 1 - (float) (Mathf.Abs(TraceSize - i) + Mathf.Abs(TraceSize - j)) / (2 * TraceSize); 
                            element[i, j, 2] = Mathf.Min(element[i, j, 2] + ballController.color.r * isOnCircuit * alpha, ballController.color.r);
                            element[i, j, 3] = Mathf.Min(element[i, j, 3] + ballController.color.g * isOnCircuit * alpha, ballController.color.g);
                            element[i, j, 4] = Mathf.Min(element[i, j, 4] + ballController.color.b * isOnCircuit * alpha, ballController.color.b);
                        }
                    }
                    terrainData.SetAlphamaps(mapX - TraceSize, mapY - TraceSize, element);

                    lastPos = transform.position;
                    isOnZone = true;
                } else {
                    bonusSpeed = -floorFriction;
                    isOnZone = false;
                    isOnColor = false;
                    isOnCircuit = 0;
                }
            } else {
                bonusSpeed = lastBonusSpeed;
            }
        } else {
            isOnColor = false;
            isOnCircuit = 0;
        } 
        ballController.bonusSpeed = ballController.speed * bonusSpeed;
        lastBonusSpeed = bonusSpeed;
    }



    // void OnCollisionEnter(Collision collision) {

    // }

    // private bool collide = false;

    //  void OnCollisionStay(Collision collision) {
    //     Debug.Log("Collision with " + collision.gameObject.name);
    //     if (collision.gameObject == terrain.gameObject) {
    //         collide = true;
    //     }
    //  }

}
