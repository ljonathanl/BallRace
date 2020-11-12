﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainController : MonoBehaviour
{

    public Model.Terrain terrain = Model.Game.instance.terrain;

    public Model.Ball ball = Model.Game.instance.ball;


    private Terrain terrainGameObject;

    private float[,,] element;

    // public float isOnCircuit;
    // public bool isOnColor;
    private int mapX;
    private int mapY;
    private TerrainData terrainData;
    private Vector3 terrainPosition;
    // private BallController ballController;
    private float[,,] alphaMaps;
    private float[,] heightMap;
    private Vector3 lastPos;
    // private bool isOnZone;
    // public float MinDistance = 0.1f;

    // public float floorFriction = 0.8f;
    // public float zoneFriction = 0.5f;

    // public float colorBonus = 0.2f;

    // public Color currentColor;

    // public int TraceSize = 3;
    private float lastBonusSpeed;

    private bool isReliefEnabled;


    public void SetTerrain(Terrain terrain) {
        this.terrainGameObject = terrain;
        terrainData = terrain.terrainData;
        lastPos = transform.position;
        terrainPosition = terrain.transform.position;
        alphaMaps = terrainData.GetAlphamaps(0, 0, terrainData.alphamapWidth, terrainData.alphamapHeight);
        heightMap = terrainData.GetHeights(0, 0, terrainData.heightmapResolution, terrainData.heightmapResolution);
    }

    public void CleanTerrain() 
    {
        if (terrainData) {
            terrainData.SetAlphamaps(0, 0, alphaMaps);
            if (isReliefEnabled)
                terrainData.SetHeights(0, 0, heightMap);
        }
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
        CleanTerrain();
    }

    void LateUpdate()
    {
        var bonusSpeed = 0f;
        // isOnCircuit = 0;
        // isOnZone = false;
        // only update if we move
            
        if (Vector3.Distance(transform.position, lastPos) > terrain.minDistance) {
            // convert world coords to terrain coords
            mapX = (int)(((transform.position.x - terrainPosition.x) / terrainData.size.x) * terrainData.alphamapWidth);
            mapY = (int)(((transform.position.z - terrainPosition.z) / terrainData.size.y) * terrainData.alphamapHeight);
            // Debug.Log(Time.timeSinceLevelLoad + " X: " + mapX  + " Y:" + mapY);

            var TraceSize = terrain.traceSize;
            if (mapX > TraceSize && mapX < (terrainData.alphamapWidth - TraceSize) && mapY > TraceSize && mapY < (terrainData.alphamapHeight - TraceSize)) {
                element = terrainData.GetAlphamaps(mapX - TraceSize, mapY - TraceSize, TraceSize * 2, TraceSize * 2);
                terrain.isOnCircuit = element[TraceSize, TraceSize, 1];
                terrain.isOnZone = true;
                
                if (ball.distanceToGround < 1) 
                {
                    terrain.isOnColor = (element[TraceSize, TraceSize, 2] + element[TraceSize, TraceSize, 3] + element[TraceSize, TraceSize, 4]) > 0;
                    bonusSpeed = (terrain.isOnCircuit - 1) * terrain.zoneFriction + (terrain.isOnColor ? terrain.colorBonus : 0);
                    var isOnCircuit = terrain.isOnCircuit;
                    for (int i = 0; i < TraceSize * 2; i++)
                    {
                        for (int j = 0; j < TraceSize * 2; j++)
                        {
                            var alpha = 1 - (float) (Mathf.Abs(TraceSize - i) + Mathf.Abs(TraceSize - j)) / (2 * TraceSize); 
                            element[i, j, 2] = Mathf.Min(element[i, j, 2] + ball.color.r * isOnCircuit * alpha, ball.color.r);
                            element[i, j, 3] = Mathf.Min(element[i, j, 3] + ball.color.g * isOnCircuit * alpha, ball.color.g);
                            element[i, j, 4] = Mathf.Min(element[i, j, 4] + ball.color.b * isOnCircuit * alpha, ball.color.b);
                        }
                    }
                    terrainData.SetAlphamaps(mapX - TraceSize, mapY - TraceSize, element);

                    lastPos = transform.position;
                    
                } else {
                    bonusSpeed = 0;
                    terrain.isOnColor = false;
                }

                terrain.currentColor = new Color(element[TraceSize, TraceSize, 2], element[TraceSize, TraceSize, 3], element[TraceSize, TraceSize, 4]);
            } else {
                bonusSpeed = -terrain.floorFriction;
                terrain.isOnZone = false;
                terrain.isOnColor = false;
                terrain.isOnCircuit = 0;
                terrain.currentColor = Color.black;
            }
        } else {
            bonusSpeed = lastBonusSpeed;
        }
        ball.bonusSpeed = ball.speed * bonusSpeed;
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
