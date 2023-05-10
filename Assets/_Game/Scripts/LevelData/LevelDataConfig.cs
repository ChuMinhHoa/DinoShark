using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum LevelType { 
    Oldtown,
    City
}
[CreateAssetMenu(fileName = "LevelDataConfig", menuName = "ScriptAbleObjects/NewLevelDataConfig")]
public class LevelDataConfig : ScriptableObject
{
    public Sprite sprLevelComingSoon;
    public List<LevelData> levelDatas;
    public LevelData GetLevelData(int level) { return levelDatas[level]; }
}
[System.Serializable]
public class LevelData {
    public string levelName;
    public Sprite levelSpriteOn;
    public Sprite levelSpriteOff;
    public LevelType levelType;
    public Vector3 vectorCameraDefault;
    public float cameraLimitMin;
    public float cameraLimitMax;
    public float cameraZoom;
    public float cameraRotate;
    public float cameraZoomMax;
    public float sizeMax;
    public string description;
    //Reward
}
