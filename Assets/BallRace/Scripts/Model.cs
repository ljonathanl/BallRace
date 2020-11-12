using System.Collections.Generic;
using System;
using UnityEngine;

namespace Model {

    [Serializable]
    public class RaceTime {

        public string name;

        public float time;

        public RaceTime(string name, float time)
        {
            this.name = name;
            this.time = time;
        }
    }

    [Serializable]
    public class LeaderBoard {
        public List<RaceTime> raceTimes;

        public int version;
    }


    [Serializable]
    public class Race {
        public int maxLaps = 5;

        public int currentLap = 0;


        public float timeElapsed = 0;

        public float lapTime = 0;

        public List<float> lapTimes;

        public LeaderBoard leaderBoard;

        public bool isRacing = false;
        public bool isNewRecord = false;

        public string playerName = "UKN";

        public string version = "0.0.1";

    }

    [Serializable]
    public class Ball {
        public Color color;

        public float speed = 1f;

        public float turnSpeed = 1f;

        public float jumpSpeed = 1f;


        public float rotation = 0;

        public float velocity;

        public float distanceToGround;

        public float bonusSpeed = 0;

        public Vector3 position;


    }


    [Serializable]
    public class Terrain {

        public bool isOnZone;
        public float minDistance = 0.1f;

        public float floorFriction = 0.8f;
        public float zoneFriction = 0.5f;

        public float colorBonus = 0.2f;

        public float isOnCircuit;
        public bool isOnColor;


        public int traceSize = 3;

        public Color currentColor;

    }


    [Serializable]
    public class Game {

        public Terrain terrain = new Terrain();

        public Race race = new Race();

        public Ball ball = new Ball();

        public static Game instance = new Game();

    }

}