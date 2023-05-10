using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
//using DG.Tweening;

public class PanelLoadScene : UIPanel
{
    public static PanelLoadScene Instance;
    public override void Awake()
    {
        panelType = UIPanelType.PanelLoadScene;
        base.Awake();
        Instance = this;
    }

    [SerializeField] SkeletonGraphic loadAnim;
    [SerializeField] Transform logo;
    [SerializeField] Transform showPos;
    [SerializeField] Transform hidePos;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            UpScene(null);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            DownScene(null);
        }
    }

    public void DownScene(UnityAction actionDone) {
        loadAnim.AnimationState.SetAnimation(0, "down", loop: false);
        StartCoroutine(IE_WaitToDoAction(actionDone, 1.3f));
        logo.DOMove(hidePos.position, 2f);
    }
    
    public void UpScene(UnityAction actionDone) {
        loadAnim.AnimationState.SetAnimation(0, "up", loop: false);
        StartCoroutine(IE_WaitToDoAction(actionDone, 1.3f));
        logo.DOMove(showPos.position, 0.5f);
    }

    IEnumerator IE_WaitToDoAction(UnityAction actionDone, float timeWait) {
        yield return new WaitForSeconds(timeWait);
        actionDone();
    }
    
    public void FirstLoad() {
        loadAnim.AnimationState.SetAnimation(0, "idle", loop: true);
    }
}
