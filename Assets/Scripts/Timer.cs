using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private const string DefaultText = "Time: ";
    //[SerializeField] private UIManager Manager;
    [SerializeField] private Text timeText;

    public static int CurrentTime => (int)(AccumulatedTime * 60);
    public static float AccumulatedTime { get; private set; }
    public static int RawSeconds => CurrentTime / 60;
    public int Minutes(int time)
    {
        int min = time / 3600;
        if (min > 99)
            min = 99;
        return min;
    }
    public int Seconds(int time)
    {
        return (time / 60) % 60;
    }
    private string AssembleTimeString(string concat, int time)
    {
        if(time >= int.MaxValue)
        {
            return concat + "N/A";
        }
        int min = Minutes(time);
        int sec = Seconds(time);
        string concat2 = ":";
        if (min < 10)
            concat += "0";
        if (sec < 10)
            concat2 += "0";
        return concat + min + concat2 + sec;
    }
    private void Start()
    {
        AccumulatedTime = 0;
        timeText.text = AssembleTimeString(DefaultText, CurrentTime);
    }
    private void Update()
    {
        AccumulatedTime += Time.deltaTime;
        timeText.text = AssembleTimeString(DefaultText, CurrentTime);
    }
}
