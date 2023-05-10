using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeUtil : MonoBehaviour {
    public static string RemainTimeToString(float remainTime) {
        TimeSpan timeSpan = TimeSpan.FromSeconds(remainTime);
        return string.Format("{0:D1}h {1:D2}m {2:D2}s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
    public static string RemainTimeToString2(float remainTime) {
        TimeSpan timeSpan = TimeSpan.FromSeconds(remainTime);
        return string.Format("{0:D1}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
    }
    public static string RemainTimeToString3(float remainTime) {
        TimeSpan timeSpan = TimeSpan.FromSeconds(remainTime);
        return string.Format("{0:D1}h {1:D2}m", timeSpan.Hours, timeSpan.Minutes);
    }
    public static string TimeToString(float inputTime)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(inputTime);
        if (timeSpan.TotalDays >= 1)
        {
            return string.Format("{0:D1}d {1:D2}h {2:D2}m", timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
        }
        else if (timeSpan.TotalHours >= 1)
        {
            return string.Format("{0:D1}h {1:D2}m {2:D2}s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }
        else if (timeSpan.TotalMinutes >= 1)
        {
            return string.Format("{0:D1}m {1:D2}s", timeSpan.Minutes, timeSpan.Seconds);
        }
        if (timeSpan.TotalSeconds < 10)
        {
            return string.Format("{0:D1}s", timeSpan.Seconds);
        }
        return string.Format("{0:D2}s", timeSpan.Seconds);
    }
}
