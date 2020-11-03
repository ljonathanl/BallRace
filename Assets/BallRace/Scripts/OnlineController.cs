using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;



public class OnlineController : MonoBehaviour
{

    private string apiUrl = "https://jsonblob.com/api/jsonBlob/";

    private string raceId = "cf2b796b-1c56-11eb-92fc-e914927e7460";

    public bool isOnline = false;

    public RaceController raceController;


    string defaultLeaderBoard = @"
        {
            ""raceTimes"": [
                {
                ""name"": ""UKN"",
                ""time"": 500
                },
                {
                ""name"": ""UKN"",
                ""time"": 501
                },
                {
                ""name"": ""UKN"",
                ""time"": 502
                }
            ],
            ""version"": 1
        }   
    ";


    void Start() {
        if (!isOnline) {
            var leaderBoard = JsonUtility.FromJson<LeaderBoard>(defaultLeaderBoard);
            raceController.SetLeaderBoard(leaderBoard);
        }
    }



    public void SaveLeaderBoard() {
        if (isOnline)
            StartCoroutine(SaveRaceLeaderBoard());
    }

    public void LoadLeaderBoard() {
        if (isOnline)
            StartCoroutine(LoadRaceLeaderBoard());
    }
 
    IEnumerator LoadRaceLeaderBoard() {
        UnityWebRequest www = UnityWebRequest.Get(apiUrl + raceId);
        www.SetRequestHeader("Accept", "application/json");
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            var leaderBoard = JsonUtility.FromJson<LeaderBoard>(www.downloadHandler.text);
            Debug.Log("LeaderBoard loaded v" + leaderBoard.version);
            raceController.SetLeaderBoard(leaderBoard);
        }
    }

    IEnumerator SaveRaceLeaderBoard() {
        raceController.leaderBoard.version += 1; 
        string json = JsonUtility.ToJson(raceController.leaderBoard);
        UnityWebRequest www = UnityWebRequest.Put(apiUrl + raceId, json);
        www.SetRequestHeader("Accept", "application/json");
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();
 
        if(www.isNetworkError || www.isHttpError) {
            Debug.Log(www.error);
        }
        else {
            Debug.Log("LeaderBoard saved v" + raceController.leaderBoard.version);
        }
    }
}
