using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;
    [SerializeField] Vector3 pointBeginDrag;
    [SerializeField] Vector3 pointCurrent;
    [SerializeField] Vector3 newPosition;
    [SerializeField] Vector3 positionDefaultLevel;

    [SerializeField] LayerMask layerMask;

    [SerializeField] Camera myCamera;
    [SerializeField] AnimationCurve curveZoom;
    [SerializeField] AnimationCurve curveMove;
    [SerializeField] float speedZoom;
    [SerializeField] float speedMove;

    bool onZoom;
    bool onFollowtarget;
    bool onZoomNormal;

    float zoomValue;
    float currentZoom;
    float zoomDefault;
    float timeZoom;
    float timeMove;

    float limitMin;
    float limitMax;
    float sizeLimit;
    float currentSizeLimit;

    [SerializeField] float maxZoom;
    Vector3 positonStart;

    Vector3 targetMove;

    UnityAction actionDone;
    int currentLevel;
    [SerializeField] float cameraSizePersent = 1440 / 2960;
    [SerializeField] float currentSizePersent;

    Vector2 secondTouchPos;
    Vector2 firstTouchPos;

    Touch touchFirst;
    Touch touchSecond;

    Vector2 firstTouchPrevPos;
    Vector2 secondTouchPrevPos;

    float prevMagnitude;
    float currentMagnitude;
    float differrent;
    bool stay;

    private void Awake()
    {
        Instance = this;
        currentSizePersent = (float)Screen.width / (float)Screen.height;
    }

    private void Update()
    {
        if (!UIManager.instance.IsHavePopUpOnScene() && !UIManager.instance.IsHaveTutorial())
            HandleMovement();

        if (onZoom)
            ZoomCamera();

        if (onFollowtarget)
            MoveCamera();
    }

    void HandleMovement() {
        if (Input.GetMouseButtonDown(0))
        {
            pointBeginDrag = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        if (Input.GetMouseButton(0) && !stay)
        {
            pointCurrent = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = pointBeginDrag - pointCurrent;
            newPosition = transform.position + direction;
        }
        if (newPosition.y > limitMax) {
            newPosition.y = limitMax;
        }

        if (newPosition.y < limitMin)
        {
            newPosition.y = limitMin;
        }

        if (Input.touchCount == 2)
        {
            touchFirst = Input.GetTouch(0);
            touchSecond = Input.GetTouch(1);

            firstTouchPos = touchFirst.position - touchFirst.deltaPosition;
            secondTouchPos = touchSecond.position - touchSecond.deltaPosition;

            prevMagnitude = (firstTouchPos - secondTouchPos).magnitude;
            currentMagnitude = (touchFirst.position - touchSecond.position).magnitude;

            differrent = currentMagnitude - prevMagnitude;
            onZoomNormal = true;
            Zoom(differrent * speedZoom);
        }
        else onZoomNormal = false;

        if (newPosition.x > currentSizeLimit)
        {
            newPosition.x = currentSizeLimit;
        }

        if (newPosition.x < -currentSizeLimit)
        {
            newPosition.x = -currentSizeLimit;
        }

        if (newPosition.z > currentSizeLimit)
        {
            newPosition.z = currentSizeLimit;
        }

        if (newPosition.z < -currentSizeLimit)
        {
            newPosition.z = -currentSizeLimit;
        }

        if (onZoomNormal)
            return;
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * speedMove);
        if (Vector3.Distance(transform.position, newPosition) <= 0.1f)
            transform.position = newPosition;
    }
    float currentZoomSize;
    void Zoom(float increament) {
        currentZoomSize = Mathf.Clamp(myCamera.orthographicSize - increament, maxZoom, zoomDefault);
        if (currentZoomSize < maxZoom)
            currentZoomSize = maxZoom;
        myCamera.orthographicSize = currentZoomSize;
        newPosition = transform.position;
        StayCamera();
    }

    public void ResetCamera() {
        currentLevel = ProfileManager.Instance.playerData.currentLevel;
        LevelData levelData = ProfileManager.Instance.dataConfig.GetLevelData(currentLevel);
        positionDefaultLevel = levelData.vectorCameraDefault;
        newPosition = positionDefaultLevel;
        zoomDefault = levelData.cameraZoom;
        maxZoom = levelData.cameraZoomMax;
        limitMin = levelData.cameraLimitMin;
        limitMax = levelData.cameraLimitMax;
        sizeLimit = levelData.sizeMax;
        currentSizeLimit = sizeLimit;
        transform.position = newPosition;
        myCamera.orthographicSize = zoomDefault * (cameraSizePersent / currentSizePersent);
        myCamera.transform.eulerAngles = new Vector3(levelData.cameraRotate, 45, 0);
    }

    public void MoveBackToDefault(UnityAction followDone = null) {
        onFollowtarget = true;
        targetMove = positionDefaultLevel;
        positonStart = transform.position;
        timeMove = 0;
        actionDone = followDone;
    }

    public void CameraMoveTarget(Transform target, Vector3 offSet, UnityAction followDone = null) {
        onFollowtarget = true;
        targetMove = target.position;
        targetMove.y = transform.position.y;
        targetMove += offSet;

        positonStart = transform.position;
        actionDone = followDone;
        timeMove = 0;
    }

    public void CameraZoomIn(float zoomInValue, UnityAction zoomDone = null) {
        //Debug.Log("OnZoomIn!");
        onZoom = true;
        currentZoom = myCamera.orthographicSize;
        zoomValue = zoomInValue;
        timeZoom = 0;
        actionDone = zoomDone;
    }

    public void CameraZoomOut(UnityAction zoomDone = null) {
        //Debug.Log("OnZoomOut!");
        onZoom = true;
        currentZoom = myCamera.orthographicSize;
        zoomValue = zoomDefault;
        timeZoom = 0;
        actionDone = zoomDone;
    }

    void ZoomCamera() {
        if (timeZoom <= 1f)
        {
            timeZoom += Time.deltaTime;
            myCamera.orthographicSize = Mathf.Lerp(currentZoom, zoomValue, curveZoom.Evaluate(timeZoom));
        }
        else
        {
            myCamera.orthographicSize = zoomValue;
            if (actionDone != null)
                actionDone();
            onZoom = false;
        }
    }

    void MoveCamera() {
        if (timeMove <= curveMove.keys[curveMove.length - 1].time)
        {
            timeMove += Time.deltaTime;
            transform.position = Vector3.Lerp(positonStart, targetMove, curveMove.Evaluate(timeMove));
        }
        else {
            transform.position = targetMove;
            if (actionDone != null)
                actionDone();
            onFollowtarget = false;
        }
    }


    public void StayCamera()
    {
        stay = true;
        StartCoroutine(IE_StopStay());
    }

    IEnumerator IE_StopStay() {
        yield return new WaitForSeconds(.1f);
        stay = false;
    }

}
