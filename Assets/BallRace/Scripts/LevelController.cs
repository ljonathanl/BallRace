using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{

    public CheckPointController[] checkPoints;
    public Transform startPosition;

    public Terrain terrain;


    public string levelId;


    void Start() 
    {
        foreach(var checkPoint in checkPoints) {
            checkPoint.gameObject.SetActive(false);
        }
    }


}
