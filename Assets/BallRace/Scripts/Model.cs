using System.Collections.Generic;
using System;

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