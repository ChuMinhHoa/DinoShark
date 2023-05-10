using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
//using GoogleMobileAdsMediationTestSuite.Api;
namespace SDK {
    public class AdmobMediationController : AdsMediationController {
        public string m_AndroidAdmobID_AppId;
        public AdsID m_AdmobID_Intertitials;
        public AdsID m_AdmobID_RewardVideo;
        public AdsID m_AdmobID_Banner;

        private InterstitialAd m_InterstitialAds;
        private RewardedAd m_RewardVideoAds;
        private BannerView bannerView;
        private bool m_IsWatchSuccess = false;

        public override void Init() {
            base.Init();
            // Initialize the Mobile Ads SDK.
            MobileAds.Initialize((initStatus) => {
                Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
                foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map) {
                    string className = keyValuePair.Key;
                    AdapterStatus status = keyValuePair.Value;
                    switch (status.InitializationState) {
                        case AdapterState.NotReady:
                            // The adapter initialization did not complete.
                            MonoBehaviour.print("Adapter: " + className + " not ready.");
                            break;
                        case AdapterState.Ready:
                            // The adapter was successfully initialized.
                            MonoBehaviour.print("Adapter: " + className + " is initialized.");
                            break;
                    }
                }
            });
        }
        private void Start() {
            //MediationTestSuite.OnMediationTestSuiteDismissed += this.HandleMediationTestSuiteDismissed;
            //ShowMediationTestSuite();
        }
        public void ShowMediationTestSuite() {
            //MediationTestSuite.Show();
        }
        public void HandleMediationTestSuiteDismissed(object sender, EventArgs args) {
            MonoBehaviour.print("HandleMediationTestSuiteDismissed event received");
        }

        #region Banner Ads
        public override void InitBannerAds() {
            base.InitBannerAds();
            RequestBannerAds();
        }
        public override void RequestBannerAds() {
            base.RequestBannerAds();

#if UNITY_ANDROID
            string adUnitId = GetBannerID();
#elif UNITY_IPHONE
            string adUnitId = "";
#else
            string adUnitId = "unexpected_platform";
#endif
            // Create a 320x50 banner at the top of the screen.
            bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
            // Called when an ad request has successfully loaded.
            bannerView.OnAdLoaded += HandleOnBannerAdLoaded;
            // Called when an ad request failed to load.
            bannerView.OnAdFailedToLoad += HandleOnBannerAdFailedToLoad;
            // Called when an ad is clicked.
            bannerView.OnAdOpening += HandleOnBannerAdOpened;
            // Called when the user returned from the app after an ad click.
            bannerView.OnAdClosed += HandleOnBannerAdClosed;
            // Called when the ad click caused the user to leave the application.
            AdRequest request = new AdRequest.Builder().Build();

            // Load the banner with the request.
            bannerView.LoadAd(request);
        }
        public override void ShowBannerAds() {
            base.ShowBannerAds();
            bannerView.Show();
        }
        public override void HideBannerAds() {
            base.HideBannerAds();
            bannerView.Hide();
        }
        public void HandleOnBannerAdLoaded(object sender, EventArgs args) {
            MonoBehaviour.print("HandleAdLoaded event received");
            m_AdmobID_Banner.Refresh();
        }
        public void HandleOnBannerAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
            MonoBehaviour.print("HandleFailedToReceiveAd event received with message: " + args.LoadAdError.GetMessage());
            m_AdmobID_Banner.ChangeID();
        }
        public void HandleOnBannerAdOpened(object sender, EventArgs args) {
            MonoBehaviour.print("HandleAdOpened event received");
        }
        public void HandleOnBannerAdClosed(object sender, EventArgs args) {
            MonoBehaviour.print("HandleAdClosed event received");
        }
        public void HandleOnAdLeavingApplication(object sender, EventArgs args) {
            MonoBehaviour.print("HandleAdLeavingApplication event received");
        }
        public string GetBannerID() {
            return m_AdmobID_Banner.ID;
        }
        #endregion

        #region Interstitial
        public override void InitInterstitialAd(UnityAction adClosedCallback, UnityAction adLoadSuccessCallback, UnityAction adLoadFailedCallback, UnityAction adShowSuccessCallback, UnityAction adShowFailCallback) {
            base.InitInterstitialAd(adClosedCallback, adLoadSuccessCallback, adLoadFailedCallback, adShowSuccessCallback, adShowFailCallback);

            string adUnitId = GetInterstitialAdUnit();

            m_InterstitialAds = new InterstitialAd(adUnitId);

            m_InterstitialAds.OnAdClosed += OnCloseInterstitialAd;
            m_InterstitialAds.OnAdLoaded += OnAdInterstitialSuccessToLoad;
            m_InterstitialAds.OnAdFailedToLoad += OnAdInterstitialFailedToLoad;
            m_InterstitialAds.OnAdOpening += OnAdInterstitialOpening;
            m_InterstitialAds.OnAdFailedToShow += OnAdInterstitialFailToShow;

            AdRequest request = new AdRequest.Builder().Build();
            m_InterstitialAds.LoadAd(request);
        }

        public override void RequestInterstitialAd() {
            base.RequestInterstitialAd();
            Debug.Log("Request interstitial ads");
            string adUnitId = GetInterstitialAdUnit();

            m_InterstitialAds = new InterstitialAd(adUnitId);

            m_InterstitialAds.OnAdClosed += OnCloseInterstitialAd;
            m_InterstitialAds.OnAdLoaded += OnAdInterstitialSuccessToLoad;
            m_InterstitialAds.OnAdFailedToLoad += OnAdInterstitialFailedToLoad;
            m_InterstitialAds.OnAdOpening += OnAdInterstitialOpening;
            m_InterstitialAds.OnAdFailedToShow += OnAdInterstitialFailToShow;


            AdRequest request = new AdRequest.Builder().Build();
            // Load the interstitial with the request.
            m_InterstitialAds.LoadAd(request);
        }
        public override bool IsInterstitialLoaded() {
            if (m_InterstitialAds != null) {
                return m_InterstitialAds.IsLoaded();
            }
            return false;
        }
        public override void ShowInterstitialAd() {
            base.ShowInterstitialAd();
            if (m_InterstitialAds.IsLoaded()) {
                m_InterstitialAds.Show();
            }
        }
        private void OnCloseInterstitialAd(object sender, EventArgs e) {
            if (m_InterstitialAdCloseCallback != null) {
                m_InterstitialAdCloseCallback();
            }
            m_InterstitialAds.Destroy();
            Debug.Log("Close Interstitial");
        }
        private void OnAdInterstitialSuccessToLoad(object sender, EventArgs e) {
            if (m_InterstitialAdLoadSuccessCallback != null) {
                m_InterstitialAdLoadSuccessCallback();
            }
            m_AdmobID_Intertitials.Refresh();
            Debug.Log("Load Interstitial success");
        }
        private void OnAdInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs e) {
            if (m_InterstitialAdLoadFailCallback != null) {
                m_InterstitialAdLoadFailCallback();
            }
            m_AdmobID_Intertitials.ChangeID();
            Debug.Log("Load Interstitial failed Admob");
        }
        private void OnAdInterstitialOpening(object sender, EventArgs e) {
            if (m_InterstitialAdShowSuccessCallback != null) {
                m_InterstitialAdShowSuccessCallback();
            }
        }
        private void OnAdInterstitialFailToShow(object sender, EventArgs e) {
            if (m_InterstitialAdShowFailCallback != null) {
                m_InterstitialAdShowFailCallback();
            }
        }
        public string GetInterstitialAdUnit() {
            return m_AdmobID_Intertitials.ID;
        }
        #endregion

        #region Rewarded Ads
        public override void InitRewardVideoAd(UnityAction videoClosed, UnityAction videoLoadSuccess, UnityAction videoLoadFailed, UnityAction videoStart) {
            base.InitRewardVideoAd(videoClosed, videoLoadSuccess, videoLoadFailed, videoStart);
            Debug.Log("Init Reward Video");
        }

        public override void RequestRewardVideoAd() {
            base.RequestRewardVideoAd();
            string adUnitId = GetRewardedAdID();
            Debug.Log("RewardedVideoAd ADMOB Reloaded ID " + adUnitId);
            if (string.IsNullOrEmpty(adUnitId)) {
                if (m_RewardedVideoLoadFailedCallback != null) {
                    m_RewardedVideoLoadFailedCallback();
                }
                m_AdmobID_RewardVideo.ChangeID();
            }
            if (m_RewardVideoAds != null && m_RewardVideoAds.IsLoaded()) return;

            m_RewardVideoAds = new RewardedAd(adUnitId);

            // Called when an ad request has successfully loaded.
            this.m_RewardVideoAds.OnAdLoaded += HandleRewardBasedVideoLoaded;
            // Called when an ad request failed to load.
            this.m_RewardVideoAds.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;
            // Called when an ad is shown.
            this.m_RewardVideoAds.OnAdOpening += HandleRewardedAdOpening;
            // Called when an ad request failed to show.
            this.m_RewardVideoAds.OnAdFailedToShow += HandleRewardedAdFailedToShow;
            // Called when the user should be rewarded for interacting with the ad.
            this.m_RewardVideoAds.OnUserEarnedReward += HandleRewardBasedVideoRewarded;

            // Called when the ad is closed.
            this.m_RewardVideoAds.OnAdClosed += HandleRewardBasedVideoClosed;

            AdRequest request = new AdRequest.Builder().Build();
            m_RewardVideoAds.LoadAd(request);
        }

        public override void ShowRewardVideoAd(UnityAction successCallback, UnityAction failedCallback) {
            base.ShowRewardVideoAd(successCallback, failedCallback);
            if (IsRewardVideoLoaded()) {
                Debug.Log("RewardedVideoAd ADMOB Show");
                m_IsWatchSuccess = false;
                m_RewardVideoAds.Show();
            }
        }
        public override bool IsRewardVideoLoaded() {
#if UNITY_EDITOR
            return false;
#endif
            if (m_RewardVideoAds != null) {
                return m_RewardVideoAds.IsLoaded();
            }
            return false;
        }
        private void HandleRewardBasedVideoLeftApplication(object sender, EventArgs e) {
        }
        private void HandleRewardBasedVideoClosed(object sender, EventArgs e) {
            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                if (m_IsWatchSuccess) {
                    if (m_RewardedVideoEarnSuccessCallback != null) {
                        EventManager.AddEventNextFrame(m_RewardedVideoEarnSuccessCallback);
                    }
                }
            }
            if (m_RewardedVideoCloseCallback != null) {
                EventManager.AddEventNextFrame(m_RewardedVideoCloseCallback);
            }
        }
        private void HandleRewardBasedVideoRewarded(object sender, GoogleMobileAds.Api.Reward e) {
            Debug.Log("RewardedVideoAd ADMOB Rewarded");
            m_IsWatchSuccess = true;
            if (Application.platform == RuntimePlatform.Android) {
                if (m_RewardedVideoEarnSuccessCallback != null) {
                    EventManager.AddEventNextFrame(m_RewardedVideoEarnSuccessCallback);
                }
            }
        }
        private void HandleRewardBasedVideoLoaded(object sender, EventArgs e) {
            Debug.Log("RewardedVideoAd ADMOB Load Success");
            if (m_RewardedVideoLoadSuccessCallback != null) {
                m_RewardedVideoLoadSuccessCallback();
            }
            m_AdmobID_RewardVideo.Refresh();
        }
        private void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs e) {
            Debug.Log("RewardedVideoAd ADMOB Load Fail");
            if (m_RewardedVideoLoadFailedCallback != null) {
                m_RewardedVideoLoadFailedCallback();
            }
            m_AdmobID_RewardVideo.ChangeID();
        }
        public void HandleRewardedAdOpening(object sender, EventArgs args) {
            Debug.Log("RewardedVideoAd ADMOB Start");
        }
        public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args) {
            Debug.Log("RewardedVideoAd ADMOB Show Fail " + args.AdError.GetMessage());
            if (m_RewardedVideoShowFailCallback != null) {
                m_RewardedVideoShowFailCallback();
            }
        }
        private void HandleRewardBasedVideoOpened(object sender, EventArgs e) {
            Debug.Log("Opened video success");
        }
        public string GetRewardedAdID() {
            return m_AdmobID_RewardVideo.ID;
        }
        #endregion

        private void OnApplicationQuit() {
            if (m_InterstitialAds != null) {
                m_InterstitialAds.Destroy();
            }
        }
        public override AdsMediationType GetAdsMediationType() {
            return AdsMediationType.ADMOB;
        }
        public override bool IsActive(AdsType adsType) {
            if (!m_IsActive) return false;
            switch (adsType) {
                case AdsType.BANNER:
                    return m_AdmobID_Banner.IsActive();
                case AdsType.INTERSTITIAL:
                    return m_AdmobID_Intertitials.IsActive();
                case AdsType.REWARDED:
                    return m_AdmobID_RewardVideo.IsActive();
            }
            return false;
        }
    }
    
}
