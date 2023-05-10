using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using UnityEngine;
using UnityEngine.Events;

namespace SDK
{
    [ScriptOrder(-10)]
    public class ABIFirebaseManager : MonoBehaviour
    {
        private bool m_IsLoaded = false;
        public FirebaseAnalyticsManager m_FirebaseAnalyticsManager;
        public FirebaseRemoteConfigManager m_FirebaseRemoteConfigManager;

        private static ABIFirebaseManager m_Instance;
        public static ABIFirebaseManager Instance
        {
            get
            {
                return m_Instance;
            }
        }

        private void Awake()
        {
            m_Instance = this;
            DontDestroyOnLoad(gameObject);
            Init();

        }
        public void Init()
        {
            m_FirebaseAnalyticsManager = new FirebaseAnalyticsManager();
            m_FirebaseRemoteConfigManager = new FirebaseRemoteConfigManager();
            Debug.Log("Start Config");
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
                if (task.Result == Firebase.DependencyStatus.Available)
                {
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
                }
            });
        }
        private void InitializeFirebase()
        {
            System.Threading.Tasks.Task.WhenAll(
                m_FirebaseRemoteConfigManager.InitRemoteConfig()
                ).ContinueWithOnMainThread(task => {
                    m_IsLoaded = true;
                    OnFetchSuccess();
                    FirebaseDataManager.Instance.Setup();
                });
        }
        private void OnFetchSuccess()
        {
            Debug.Log("Fetch Success");
            Debug.Log("---------------------Update All RemoteConfigs----------------------");
            EventManager.AddEventNextFrame(() => EventManager.TriggerEvent("UpdateRemoteConfigs"));
            //EventManager.AddEventNextFrame(() => EventManager.TriggerEvent("ShowAdsFirstTime"));
        }
        public bool IsFirebaseReady
        {
            get
            {
                return m_IsLoaded;
            }
        }
        public void LogFirebaseEvent(string eventName, string eventParamete, double eventValue)
        {
            if (IsFirebaseReady)
            {
                m_FirebaseAnalyticsManager.LogEvent(eventName, eventParamete, eventValue);
            }
        }
        public void LogFirebaseEvent(string eventName, Parameter[] paramss)
        {
            if (IsFirebaseReady)
            {
                m_FirebaseAnalyticsManager.LogEvent(eventName, paramss);
            }
        }
        public void LogFirebaseEvent(string eventName)
        {
            if (IsFirebaseReady)
            {
                m_FirebaseAnalyticsManager.LogEvent(eventName);
            }
        }
        public void SetUserProperty(string propertyName, string property)
        {
            if (IsFirebaseReady)
            {
                m_FirebaseAnalyticsManager.SetUserProperty(propertyName, property);
            }
        }
        public void FetchData(UnityAction successCallback)
        {
            m_FirebaseRemoteConfigManager.FetchData(successCallback);
        }
        public ConfigValue GetConfigValue(string key)
        {
            return m_FirebaseRemoteConfigManager.GetValues(key);
        }
    }
}

