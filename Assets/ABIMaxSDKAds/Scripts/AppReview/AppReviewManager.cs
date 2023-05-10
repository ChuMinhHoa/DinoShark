#if UNITY_ANDROID
//using Google.Play.Review;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AppReviewManager : MonoBehaviour {
//    private static AppReviewManager m_Instance;
//    public static AppReviewManager Instance {
//        get {
//            return m_Instance;
//        }
//    }
//#if UNITY_ANDROID
//    private ReviewManager m_ReviewManager;
//    private PlayReviewInfo m_PlayerReviewInfo = null;
//    private UnityAction m_RequestSuccessCallback = null;
//#endif

//    private void Awake() {
//        m_Instance = this;
//        DontDestroyOnLoad(gameObject);
//    }
//    public void StartRequestReviewInfo(UnityAction callback = null) {
//#if UNITY_ANDROID
//        m_PlayerReviewInfo = null;
//        m_RequestSuccessCallback = callback;
//        m_ReviewManager = new ReviewManager();
//        StartCoroutine(coRequestReviewInfo());
//#endif
//    }
//    public void ShowReview() {
//#if UNITY_ANDROID
//        StartCoroutine(coShowReview());
//#endif
//    }

//#if UNITY_ANDROID
//    public bool IsReviewReady() {
//        return m_PlayerReviewInfo != null;
//    }
//    IEnumerator coShowReview() {
//        var launchFlowOperation = m_ReviewManager.LaunchReviewFlow(m_PlayerReviewInfo);
//        yield return launchFlowOperation;
//        m_PlayerReviewInfo = null; // Reset the object
//        if (launchFlowOperation.Error != ReviewErrorCode.NoError) {
//            // Log error. For example, using requestFlowOperation.Error.ToString().
//            yield break;
//        }
//        // The flow has finished. The API does not indicate whether the user
//        // reviewed or not, or even whether the review dialog was shown. Thus, no
//        // matter the result, we continue our app flow.
//    }
//    IEnumerator coRequestReviewInfo() {
//        var requestFlowOperation = m_ReviewManager.RequestReviewFlow();
//        yield return requestFlowOperation;
//        if (requestFlowOperation.Error != ReviewErrorCode.NoError) {
//            // Log error. For example, using requestFlowOperation.Error.ToString().
//            yield break;
//        }
//        m_PlayerReviewInfo = requestFlowOperation.GetResult();
//        if(m_PlayerReviewInfo != null) {
//            if(m_RequestSuccessCallback!= null) {
//                m_RequestSuccessCallback();
//            }
//        }
//    } 
//#endif
}