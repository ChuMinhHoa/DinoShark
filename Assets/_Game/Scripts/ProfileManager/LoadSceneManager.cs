using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SDK;

public class LoadSceneManager : Singleton<LoadSceneManager>
{
    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
    public bool isLoadDataDone;
    LevelData levelNextData;
    LevelData levelLastData;
    private void Start()
    {
        ProfileManager.Instance.playerData.LoadData();
        ProfileManager.Instance.playerData.SetCurrentLevel();
        ChangeOnFirstSceneLevel();
    }

    void ChangeOnFirstSceneLevel() {
        TutorialManager.Instance.canUpdateTutorial = false;
        levelNextData = ProfileManager.Instance.dataConfig.GetLevelData(ProfileManager.Instance.playerData.currentLevel);
        levelLastData = null;
        UIManager.instance.ShowLoadScene(); 
        PanelLoadScene.Instance.FirstLoad();
        LoadGame();
    }

    public void ChangeLevel(int lastLevel = 0, bool cheat = false) {
        isLoadDataDone = false;
        TutorialManager.Instance.canUpdateTutorial = false;
        levelNextData = ProfileManager.Instance.dataConfig.GetLevelData(ProfileManager.Instance.playerData.currentLevel);
        if(cheat)
        {
            levelLastData = ProfileManager.Instance.dataConfig.GetLevelData(lastLevel);
        }
        else
        {
            levelLastData = ProfileManager.Instance.dataConfig.GetLevelData(ProfileManager.Instance.playerData.currentLevel - 1);
        }
        
        UIManager.instance.ShowLoadScene();
        PanelLoadScene.Instance.UpScene(LoadGame);
    }

    void LoadGame() {
        UIManager.instance.ClosePanelTotal();
        if (levelLastData != null) scenesLoading.Add(SceneManager.UnloadSceneAsync(levelLastData.levelName));
        scenesLoading.Add(SceneManager.LoadSceneAsync(levelNextData.levelName, LoadSceneMode.Additive));
        totalProgress = 0;
        currentProgress = 0;
        StartCoroutine(GetSceneLoadProcess());
    }

    //float tottalSceneProgress;
    IEnumerator GetSceneLoadProcess() {
        for (int i = 0; i < scenesLoading.Count; i++)
        {
            while (!scenesLoading[i].isDone)
            {
            //    tottalSceneProgress = 0;
            //    foreach (AsyncOperation operation in scenesLoading)
            //    {
            //        tottalSceneProgress += operation.progress;
            //    }
            //    tottalSceneProgress = Mathf.Round(tottalSceneProgress / scenesLoading.Count);
            //    PanelLoadScene.Instance.ChangeProcess(tottalSceneProgress);
                yield return null;
            }
        }
        ProfileManager.Instance.playerData.LoadDataLevel();
    }
    float totalProgress;
    float currentProgress = 0;
    public void ILoadDataDone() {
        StartCoroutine(GetLevelProgress());
        isLoadDataDone = true;
    }
    public void AddTotalProgress(int value) { totalProgress += value; }
    IEnumerator GetLevelProgress()
    {
        while (currentProgress < totalProgress)
        {
            currentProgress += 1f;
            yield return null;
        }
        CameraController.Instance.ResetCamera();
        UIManager.instance.ShowPanelTotal();
        UIManager.instance.GetTotalPanel().CheckUICondition();
        PanelLoadScene.Instance.DownScene(LoadSceneDone);
        StartCoroutine(SendSetupVersionCoroutine());
    }

    void LoadSceneDone() {
        UIManager.instance.CloseLoadScene();
        TutorialManager.Instance.ResetTutorial();
        GameManager.Instance.GetOfflineRevenue();
        UIManager.instance.GetTotalPanel().adsRewardButton.Reset();
    }

    IEnumerator SendSetupVersionCoroutine()
    {
        while (!ABIFirebaseManager.Instance.IsFirebaseReady)
        {
            yield return 0;
        }
        ProfileManager.Instance.playerData.CheckSetup();
    }
}
