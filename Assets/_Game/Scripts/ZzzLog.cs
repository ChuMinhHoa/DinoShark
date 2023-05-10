using UnityEngine;
using System.Collections;

public class ZzzLog : MonoBehaviour
{
    uint qsize = 15;  // number of messages to keep
    Queue myLogQueue = new Queue();

    void Start()
    {
        Debug.Log("Started up logging.");
    }

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        if(type != LogType.Warning)
        {
            myLogQueue.Enqueue("[" + type + "] : " + logString);
            if (type == LogType.Exception)
                myLogQueue.Enqueue(stackTrace);
            while (myLogQueue.Count > qsize)
                myLogQueue.Dequeue();
        }
    }

    void OnGUI()
    {
        //GUILayout.BeginArea(new Rect(Screen.width - 400, 0, 500, Screen.height));
        //GUILayout.Label("\n" + string.Join("\n", myLogQueue.ToArray()));
        //GUILayout.EndArea();
        GUI.color = Color.black;
        GUI.Label(new Rect(Screen.width - 400, 0, 500, Screen.height),
            "\n" + string.Join("\n", myLogQueue.ToArray())
        );
    }
}