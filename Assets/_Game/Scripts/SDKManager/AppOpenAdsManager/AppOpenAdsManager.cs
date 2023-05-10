using System;
using System.Collections;
using Firebase.RemoteConfig;
using GoogleMobileAds.Api;
using Unity.VisualScripting;
using UnityEngine;
namespace SDK {
    public class AppOpenAdsManager : Singleton<AppOpenAdsManager> {
#if UNITY_ANDROID
        private string AD_UNIT_ID = "ca-app-pub-9819920607806935/9028968660";
#elif UNITY_IOS
    private const string AD_UNIT_ID = "ca-app-pub-9819920607806935/5831414867";
#else
        private const string AD_UNIT_ID = "unexpected_platform";
#endif
        //public static string key_ads_show_first_time = "key_ads_show_first_time";
        private AppOpenAd m_Ad;

        private bool m_IsActive;
        private bool m_IsShowingAd = false;
        private bool m_IsDoneShowAdsFirstTime;
        public bool IsDoneShowAdsFirstTime{
            get { return m_IsDoneShowAdsFirstTime; }
            set { 
                m_IsDoneShowAdsFirstTime = value;
                Debug.Log("Done Show AOA First " + value);
            } 
        }

        private DateTime m_StartAdsTime;
        private DateTime m_CloseAdsTime = new DateTime(1970, 1, 1);
        private DateTime m_StartPauseTime;

        private bool m_IsActiveShowAdsFirstTime = false;
        private bool m_IsLockingOpenAds = false;
        private double m_AoaTimeBetweenStep = 15;
        private double m_AoaPauseTimeNeedToShowAds = 15;

        private Coroutine m_IgnoreShowAds;
        private bool IsAdAvailable {
            get {
                double num = (DateTime.UtcNow - m_CloseAdsTime).TotalMinutes;
                //Debug.Log("Time Close " + num + " thresh " + m_AoaTimeBetweenStep);
                return m_Ad != null &&  num > m_AoaTimeBetweenStep;
            }
        }

        public bool IsActive { 
            get {
                return m_IsActive && !ProfileManager.Instance.playerData.globalResourceSave.removeAds;
            }
            set => m_IsActive = value; 
        }

        protected override void Awake() {
            base.Awake();
            m_IsDoneShowAdsFirstTime = false;
            IsActive = PlayerPrefs.GetInt(ABI.Keys.key_remote_aoa_active, 0) == 1;
            Debug.Log("----------------- AppOpenAds Settings ------------------");
            EventManager.AddListener("UpdateRemoteConfigs", UpdateRemoteConfigs);
            EventManager.AddListener("ShowAdsFirstTime", ShowAdsFirstTime);
            EventManager.AddListener("DeactiveAOA", () => SetActiveAOA(false, 0));
            EventManager.AddListener("ActiveAOA", () => SetActiveAOA(true, 1));

            LoadAd();
        }
        private void Update() {
            if (Input.GetKeyDown(KeyCode.C)) {
                ShowAds();
            }
        }

        public void OnApplicationPause(bool paused) {
            Debug.Log("APP PAUSE " + paused + " Locking " + m_IsLockingOpenAds + " Active " + IsActive);
            if (m_IsLockingOpenAds || !IsActive) return;
            if (paused) {
                m_StartPauseTime = DateTime.UtcNow;
            }
            if (!paused) {
                Debug.Log("RESUME GAME");
                double time = (DateTime.Now - m_StartPauseTime).TotalSeconds;
                if (time > m_AoaPauseTimeNeedToShowAds){
                    ShowAdIfAvailable();
                }
                SetActiveAOA(true, 0);
            }
        }
        public void SetActiveAOA(bool value, float time) {
            if (m_IgnoreShowAds != null) {
                StopCoroutine(m_IgnoreShowAds);
            }
            m_IgnoreShowAds = StartCoroutine(CoSetActiveAOA(value, time));
        }
        IEnumerator CoSetActiveAOA(bool value, float time) {
            yield return new WaitForSeconds(time);
            m_IsLockingOpenAds = !value;
        }
        public void ShowAdsFirstTime() {
            Debug.Log("AOA First Time ");
            if (m_IsLockingOpenAds || !IsActive) return;
            //Debug.Log("AOA 2 " + m_IsDoneShowAdsFirstTime + " active first time " + m_IsActiveShowAdsFirstTime);
            if (m_IsDoneShowAdsFirstTime || !m_IsActiveShowAdsFirstTime) return;
            StartCoroutine(WaitFechingSuccessAndShow());
        }
        private void UpdateRemoteConfigs() {
            Debug.Log("Update RemoteConfig");
            {
                ConfigValue configValue = ABIFirebaseManager.Instance.GetConfigValue(ABI.Keys.key_remote_aoa_active);
                IsActive = configValue.BooleanValue;
#if UNITY_EDITOR
                IsActive = true; 
#endif
                Debug.Log("AOA active = " + IsActive);
                int num = IsActive ? 1 : 0;
                PlayerPrefs.SetInt(ABI.Keys.key_remote_aoa_active, num);
            }

            {
                ConfigValue configValue = ABIFirebaseManager.Instance.GetConfigValue(ABI.Keys.key_remote_aoa_show_first_time_active);
                m_IsActiveShowAdsFirstTime = configValue.BooleanValue;
#if UNITY_EDITOR
                m_IsActiveShowAdsFirstTime = true; 
#endif
                Debug.Log("AOA active show first time = " + m_IsActiveShowAdsFirstTime);
            }

            {
                ConfigValue configValue = ABIFirebaseManager.Instance.GetConfigValue(ABI.Keys.key_remote_aoa_time_between_step);
                m_AoaTimeBetweenStep = configValue.DoubleValue;
                Debug.Log("AOA Load time = " + m_AoaTimeBetweenStep);
            }

            {
                ConfigValue configValue = ABIFirebaseManager.Instance.GetConfigValue(ABI.Keys.key_remote_aoa_pause_time_need_to_show_ads);
                m_AoaPauseTimeNeedToShowAds = configValue.DoubleValue;
                Debug.Log("AOA Pause time = " + m_AoaPauseTimeNeedToShowAds);
            }

        }
        public void LoadAd() {
            Debug.Log("Start Load 1");
            if (IsAdAvailable) return;
            Debug.Log("Start Load 2");
            AdRequest request = new AdRequest.Builder().Build();

            // Load an app open ad for portrait orientation
            AppOpenAd.LoadAd(AD_UNIT_ID, ScreenOrientation.Portrait, request, (appOpenAd, error) => {
                if (error != null) {
                    // Handle the error.
                    Debug.LogFormat("Failed to load the ad. (reason: {0})", error.LoadAdError.GetMessage());
                    return;
                }

                Debug.Log("Load Ads Success");
                // App open ad is loaded.
                m_Ad = appOpenAd;
                m_StartAdsTime = DateTime.UtcNow;

            });
        }
        public void ShowAdIfAvailable() {
            Debug.Log("AOA" + "\n"+
                "IsAdAvailable = " + IsAdAvailable + "\n" +
                "IsShowingAd = " + m_IsShowingAd + "\n" +
                "IsActive = " + IsActive);
            if (!IsAdAvailable || m_IsShowingAd || !IsActive) {
                return;
            }
            Debug.Log("Open Ads Success");
#if !UNITY_EDITOR
            ShowAds();
#endif
        }

        public void ShowAds() {
            m_Ad.OnAdDidDismissFullScreenContent += HandleAdDidDismissFullScreenContent;
            m_Ad.OnAdFailedToPresentFullScreenContent += HandleAdFailedToPresentFullScreenContent;
            m_Ad.OnAdDidPresentFullScreenContent += HandleAdDidPresentFullScreenContent;
            m_Ad.OnAdDidRecordImpression += HandleAdDidRecordImpression;
            m_Ad.OnPaidEvent += HandlePaidEvent;
            m_Ad.Show();
        }
        IEnumerator WaitFechingSuccessAndShow() {
            while (m_Ad == null) {
                yield return new WaitForSeconds(0.5f);
            }

            if (!m_IsDoneShowAdsFirstTime) {
                ShowAdIfAvailable();

                m_IsDoneShowAdsFirstTime = true;
                Debug.Log("Show App Open Ads First Time");
            }

        }
        private void HandleAdDidDismissFullScreenContent(object sender, EventArgs args) {
            Debug.Log("Closed app open ad");
            // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
            m_Ad = null;
            m_IsShowingAd = false;
            m_CloseAdsTime = DateTime.UtcNow;
            LoadAd();
        }

        private void HandleAdFailedToPresentFullScreenContent(object sender, AdErrorEventArgs args) {
            Debug.LogFormat("Failed to present the ad (reason: {0})", args.AdError.GetMessage());
            // Set the ad to null to indicate that AppOpenAdManager no longer has another ad to show.
            m_Ad = null;
            LoadAd();
        }

        private void HandleAdDidPresentFullScreenContent(object sender, EventArgs args) {
            Debug.Log("Displayed app open ad");
            m_IsShowingAd = true;
        }

        private void HandleAdDidRecordImpression(object sender, EventArgs args) {
            Debug.Log("Recorded ad impression");
        }

        private void HandlePaidEvent(object sender, AdValueEventArgs args) {
            Debug.LogFormat("Received paid event. (currency: {0}, value: {1}",
                    args.AdValue.CurrencyCode, args.AdValue.Value);
        }
    }
}
