﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Model;

public class RaceController : MonoBehaviour
{

    public Model.Race race = Model.Game.instance.race;

    public Model.Ball ball = Model.Game.instance.ball;

    public LevelController currentLevel;

    public LevelController[] levels;

    public TerrainController terrainController;

    public OnlineController onlineController;


    public CheckPointController nextCheckPoint;

    public int nextCheckPointIndex = 0;

    public Color color;

    public Hub hub;


    // public int maxLaps = 5;

    // public int currentLap = 0;


    // public float timeElapsed = 0;

    // public float lapTime = 0;

    // public List<float> lapTimes;
    public AudioClip checkPointSound;



    public AudioClip welcomeAudio;
    public AudioClip startAudio;
    public AudioClip finishAudio;
    public AudioClip newRecordAudio;
    public AudioClip finalLapAudio;

    // public bool isRacing = false;
    // public bool isNewRecord = false;

    // public UnityEvent OnLeaderBoardChange;
    // public LeaderBoard leaderBoard;

    // public string playerName = "UKN";

    // public string version = "0.0.1";


    public Color[] colors;


    private Coroutine playRandomColorCoroutine;

    void Log(string message) {
        hub.infoText.GetComponent<AnimatedText>().ChangeText(message);
    }


    // Start is called before the first frame update
    void Start()
    {
        SetLevel(currentLevel);
        Reset();
        race.playerName = PlayerPrefs.GetString("Name", "UKN");
        Log("Welcome to BallRace (v" + race.version + ") !!!");
        hub.nameText.text = race.playerName;
        StartCoroutine(PlayWelcome());
        playRandomColorCoroutine = StartCoroutine(PlayRandomColor());
    }

    IEnumerator PlayWelcome() {
        yield return new WaitForSeconds(1);
        PlaySound(welcomeAudio);
    }

    IEnumerator PlayRandomColor() {
        while (true) {
            yield return new WaitForSeconds(1);
            color = GetRandomColor();
            hub.color = color;
        }
    }

    void Update() {
        if (race.currentLap > 0) {
            race.timeElapsed += Time.deltaTime;
            race.lapTime += Time.deltaTime;
        }

        string lastLaps = "";

        for (var i = 0; i < race.lapTimes.Count; i++) {
            lastLaps += convert(race.lapTimes[i]) + "\n";
        }

        hub.raceText.text = 
            race.currentLap + "/" + race.maxLaps + "\n" +
            convert(race.timeElapsed) + "\n" +
            lastLaps +
            (race.isRacing && race.currentLap > 1 ? convert(race.lapTime) + "\n" : "");
    }

    private string convert(float toConvert) {
        return string.Format("{0:#0}:{1:00}.{2:000}",
                     Mathf.Floor(toConvert / 60),//minutes
                     Mathf.Floor(toConvert) % 60,//seconds
                     Mathf.Floor((toConvert*100) % 100));
    }

    void PlaySound(AudioClip sound) {
        GetComponent<AudioSource>().PlayOneShot(sound);
    }


    public void OnCheckPoint(CheckPointController checkPoint) {
        if (nextCheckPoint == checkPoint) {
            checkPoint.gameObject.SetActive(false);
            PlaySound(checkPointSound);
            if (nextCheckPointIndex == 0) {
                race.currentLap++;
                if (race.currentLap == 1) { // Start
                    PlaySound(startAudio);
                    race.isRacing = true;
                    race.timeElapsed = 0;
                    if (playRandomColorCoroutine != null) {
                        StopCoroutine(playRandomColorCoroutine);
                    }
                } else {
                    race.lapTimes.Add(race.lapTime);
                }
                if (race.currentLap > race.maxLaps) { // End
                    race.isRacing = false;
                    race.currentLap = 0;
                    CheckLeaderBoard();
                    playRandomColorCoroutine = StartCoroutine(PlayRandomColor());
                    return;
                }
                if (race.currentLap == race.maxLaps) {
                    Log(race.currentLap == race.maxLaps ? "FINAL LAP !!!" : "LAP #" + race.currentLap);
                    PlaySound(finalLapAudio);
                } else {
                    Log(race.currentLap == race.maxLaps ? "FINAL LAP !!!" : "LAP #" + race.currentLap);
                }
                race.lapTime = 0;
            }
            nextCheckPointIndex = (nextCheckPointIndex + 1) % currentLevel.checkPoints.Length;
            nextCheckPoint = currentLevel.checkPoints[nextCheckPointIndex];
            nextCheckPoint.gameObject.SetActive(true);
            if (nextCheckPointIndex == 0) {
                if (race.currentLap == race.maxLaps) {
                    nextCheckPoint.ChangeText("FINISH");    
                } else {
                    nextCheckPoint.ChangeText("LAP " + (race.currentLap + 1));    
                }
            } else {
                nextCheckPoint.ChangeText("LAP " + race.currentLap + "." + nextCheckPointIndex);
            }

            hub.color = color;
            color = GetRandomColor();
            nextCheckPoint.ChangeColor(color);
        }
    
    }

    void CheckLeaderBoard() {
        var i = 0;
        for (; i < race.leaderBoard.raceTimes.Count; i++) {
            var raceTime = race.leaderBoard.raceTimes[i];
            if (race.timeElapsed < raceTime.time) {
                break;       
            }
        }

        if (i < race.leaderBoard.raceTimes.Count) {
            race.leaderBoard.raceTimes.Insert(i, new RaceTime(race.playerName, race.timeElapsed));
            race.leaderBoard.raceTimes.RemoveAt(race.leaderBoard.raceTimes.Count - 1);
            SetLeaderBoard(race.leaderBoard);
            onlineController.SaveLeaderBoard();
            race.isNewRecord = true;
            Log("NEW RECORD !!! CONGRATULATION !!!");
            PlaySound(newRecordAudio);
        } else {
            Log("FINISH !!!");
            PlaySound(finishAudio);
        }
        
    }

    public void Reset() {
        if (playRandomColorCoroutine != null) {
            StopCoroutine(playRandomColorCoroutine);
        }

        foreach(var checkPoint in currentLevel.checkPoints) {
            checkPoint.ball = ball;
            checkPoint.raceController = this;
            checkPoint.gameObject.SetActive(false);
        }

        nextCheckPointIndex = 0;
        nextCheckPoint = currentLevel.checkPoints[0];
        nextCheckPoint.gameObject.SetActive(true);
        nextCheckPoint.ChangeText("START");


        color = GetRandomColor();
        nextCheckPoint.ChangeColor(color);

        ball.position = currentLevel.startPosition.position;
        ball.rotation = currentLevel.startPosition.rotation.eulerAngles.y;

        terrainController.CleanTerrain();

        race.timeElapsed = 0;
        race.lapTime = 0;

        race.lapTimes = new List<float>();

        race.currentLap = 0;
        race.isRacing = false;
        race.isNewRecord = false;
        onlineController.LoadLeaderBoard();
    }

    public void SetLevel(int levelNumber) 
    {
        SetLevel(levels[levelNumber]);
    }

    public void SetLevel(LevelController level) 
    {
        foreach(var checkPoint in currentLevel.checkPoints) {
            checkPoint.gameObject.SetActive(false);
        }
        terrainController.CleanTerrain();
        currentLevel = level;
        terrainController.SetTerrain(currentLevel.terrain);
        transform.position = currentLevel.terrain.transform.position;
        Reset();
    }

    Color GetRandomColor() {
        return Random.ColorHSV(0, 1, 0.5f, 1, 0.5f, 0.8f);

        //return colors[Random.Range(0, colors.Length)];
    }

    public void SetLeaderBoard(LeaderBoard leaderBoard) {
        var text = "";
        for (var i = 0; i < leaderBoard.raceTimes.Count; i++) {
            var raceTime = leaderBoard.raceTimes[i];
            text += (i + 1) + "." + raceTime.name.Substring(0, 3) + "*" + convert(raceTime.time) + "\n";
        }

        hub.leaderBoardText.text = text;
        race.leaderBoard = leaderBoard; 
    }

    public void ChangePlayerName(string value) {
        race.playerName = value;
        PlayerPrefs.SetString("Name", race.playerName);
        hub.nameText.text = race.playerName;
    }
}
