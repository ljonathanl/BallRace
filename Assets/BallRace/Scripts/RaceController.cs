using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class RaceController : MonoBehaviour
{

    public GameObject ball;

    public LevelController currentLevel;

    public LevelController[] levels;

    public BallController ballController;

    public TerrainController terrainController;

    public OnlineController onlineController;


    public CheckPointController nextCheckPoint;

    public int nextCheckPointIndex = 0;

    public Color color;

    public Hub hub;


    public int maxLaps = 5;

    public int currentLap = 0;


    public float timeElapsed = 0;

    public float lapTime = 0;

    public List<float> lapTimes;
    public AudioClip checkPointSound;



    public AudioClip welcomeAudio;
    public AudioClip startAudio;
    public AudioClip finishAudio;
    public AudioClip newRecordAudio;
    public AudioClip finalLapAudio;

    public bool isRacing = false;
    public bool isNewRecord = false;

    // public UnityEvent OnLeaderBoardChange;
    public LeaderBoard leaderBoard;

    public string playerName = "UKN";

    public string version = "0.0.1";


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
        playerName = PlayerPrefs.GetString("Name", "UKN");
        Log("Welcome to BallRace (v" + version + ") !!!");
        hub.nameText.text = playerName;
        StartCoroutine(PlayWelcome());
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
        if (currentLap > 0) {
            timeElapsed += Time.deltaTime;
            lapTime += Time.deltaTime;
        }

        string lastLaps = "";

        for (var i = 0; i < lapTimes.Count; i++) {
            lastLaps += convert(lapTimes[i]) + "\n";
        }

        hub.raceText.text = 
            currentLap + "/" + maxLaps + "\n" +
            convert(timeElapsed) + "\n" +
            lastLaps +
            (isRacing && currentLap > 1 ? convert(lapTime) + "\n" : "");
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
                currentLap++;
                if (currentLap == 1) { // Start
                    PlaySound(startAudio);
                    isRacing = true;
                    timeElapsed = 0;
                } else {
                    lapTimes.Add(lapTime);
                }
                if (currentLap > maxLaps) { // End
                    isRacing = false;
                    currentLap = 0;
                    CheckLeaderBoard();
                    playRandomColorCoroutine = StartCoroutine(PlayRandomColor());
                    return;
                }
                if (currentLap == maxLaps) {
                    Log(currentLap == maxLaps ? "FINAL LAP !!!" : "LAP #" + currentLap);
                    PlaySound(finalLapAudio);
                } else {
                    Log(currentLap == maxLaps ? "FINAL LAP !!!" : "LAP #" + currentLap);
                }
                lapTime = 0;
            }
            nextCheckPointIndex = (nextCheckPointIndex + 1) % currentLevel.checkPoints.Length;
            nextCheckPoint = currentLevel.checkPoints[nextCheckPointIndex];
            nextCheckPoint.gameObject.SetActive(true);
            if (nextCheckPointIndex == 0) {
                if (currentLap == maxLaps) {
                    nextCheckPoint.ChangeText("FINISH");    
                } else {
                    nextCheckPoint.ChangeText("LAP " + (currentLap + 1));    
                }
            } else {
                nextCheckPoint.ChangeText("LAP " + currentLap + "." + nextCheckPointIndex);
            }

            hub.color = color;
            color = GetRandomColor();
            nextCheckPoint.ChangeColor(color);
        }
    
    }

    void CheckLeaderBoard() {
        var i = 0;
        for (; i < leaderBoard.raceTimes.Count; i++) {
            var raceTime = leaderBoard.raceTimes[i];
            if (timeElapsed < raceTime.time) {
                break;       
            }
        }

        if (i < leaderBoard.raceTimes.Count) {
            leaderBoard.raceTimes.Insert(i, new RaceTime(playerName, timeElapsed));
            leaderBoard.raceTimes.RemoveAt(leaderBoard.raceTimes.Count - 1);
            SetLeaderBoard(leaderBoard);
            onlineController.SaveLeaderBoard();
            isNewRecord = true;
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
            checkPoint.ballController = ballController;
            checkPoint.raceController = this;
            checkPoint.gameObject.SetActive(false);
        }

        nextCheckPointIndex = 0;
        nextCheckPoint = currentLevel.checkPoints[0];
        nextCheckPoint.gameObject.SetActive(true);
        nextCheckPoint.ChangeText("START");


        color = GetRandomColor();
        nextCheckPoint.ChangeColor(color);

        ball.transform.position = currentLevel.startPosition.position;
        ballController.rotation = currentLevel.startPosition.rotation.eulerAngles.y;

        terrainController.CleanTerrain();

        timeElapsed = 0;
        lapTime = 0;

        lapTimes = new List<float>();

        currentLap = 0;
        isRacing = false;
        isNewRecord = false;
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
        this.leaderBoard = leaderBoard; 
    }

    public void ChangePlayerName(string value) {
        playerName = value;
        PlayerPrefs.SetString("Name", playerName);
        hub.nameText.text = playerName;
    }
}
