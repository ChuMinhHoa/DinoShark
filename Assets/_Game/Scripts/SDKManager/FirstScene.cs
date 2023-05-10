using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Diagnostics.Contracts;

namespace SDK
{
    public class FirstScene : MonoBehaviour
    {
        string m_NextScene;
        private AsyncOperation m_AsyncLoad;
        bool startLoad = false;
        public Transform m_SliderBar;
        public Text loadedPercent;
        float loadCounter;
        float loadTime = 3f;
        int countStart = 0;
        private bool IsShowAOA = false;
        private void Start()
        {
            //SceneManager.sceneLoaded += SceneManager_activeSceneChanged;
            //int index = ProfileManager.PlayerData.GetSelectedWorld();
            //if (index < 0) {
            //    if (index == -1) m_NextScene = "Event_NE";
            //}
            //else
            //    m_NextScene = "Level" + index; 
            //StartCoroutine(OnLoadingNextScene(m_NextScene));
            //countStart = PlayerPrefs.GetInt("SessionStart", 0);
            //countStart++;
            //if (countStart <= 10) ABIAnalyticsManager.Instance.TrackSessionStart(countStart);
        }
        private void SceneManager_activeSceneChanged(Scene scene, LoadSceneMode mode)
        {
            //Debug.Log("Change Scene Success");
            ////gameObject.SetActive(false);
        }
        //IEnumerator OnLoadingNextScene(string sceneName)
        //{
        //    m_SliderBar.transform.localScale = new Vector3(0, 1, 1);
        //    loadCounter = 0;
        //    startLoad = false;
        //    m_AsyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        //    m_AsyncLoad.allowSceneActivation = false;
        //    yield return new WaitForEndOfFrame();
        //    while (m_AsyncLoad.progress < 0.9f)
        //    {
        //        yield return new WaitForEndOfFrame();
        //    }
        //    startLoad = true;
        //    m_SliderBar.DOScaleX(1, loadTime);
        //    yield return new WaitForSeconds(loadTime);
        //    m_AsyncLoad.allowSceneActivation = true;
        //}

        //private void Update()
        //{
        //    if(loadCounter < 100 && startLoad)
        //    {
        //        loadCounter += Time.deltaTime * 100f / loadTime;
        //        if (loadCounter > 80f && !IsShowAOA) {
        //            IsShowAOA = true;
        //            if (ABIFirebaseManager.Instance.IsFirebaseReady) {
        //                EventManager.AddEventNextFrame(() => EventManager.TriggerEvent("ShowAdsFirstTime"));
        //            }
        //        }
        //    }      
        //    loadedPercent.text = ((int)loadCounter).ToString() + "%";
        //}
    }
}


