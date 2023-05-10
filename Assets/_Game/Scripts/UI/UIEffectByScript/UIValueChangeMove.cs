using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

public enum ValueType { 
    Gem,
    Coin
}

[System.Serializable]
public class SpriteFollowValue {
    public ValueType valueType;
    public Sprite sprIcon;
}

public class UIValueChangeMove : MonoBehaviour
{
    public List<SpriteFollowValue> spriteValues = new List<SpriteFollowValue>();
    [SerializeField] Image imgIcon;
    [SerializeField] Transform trsMove;
    [SerializeField] Text txtCount;
    [SerializeField] CanvasGroup textCountCanvas;
    Transform target;
    UnityAction actionDone;

    Vector3 vectorScaleDefalut = new Vector3(1, 1, 1);
    Vector3 pointMove1, pointMove2, pointMove3;

    public void DoMove(Transform target, Transform startPoint, ValueType valueType, int valueChange, UnityAction actionDone = null) {

        this.target = target;
        trsMove.position = startPoint.position;
        trsMove.localScale = Vector3.zero;

        SetPoint(startPoint);
        SetIcon(valueType);

        trsMove.DOScale(vectorScaleDefalut, 0.25f).OnComplete(Move);
        textCountCanvas.DOFade(1, 0.55f);
        txtCount.text = "+" + valueChange.ToString();

        this.actionDone = actionDone;
        transform.SetAsLastSibling();
    }

    void SetIcon(ValueType valueType) {
        for (int i = 0; i < spriteValues.Count; i++)
        {
            if (valueType == spriteValues[i].valueType)
                imgIcon.sprite = spriteValues[i].sprIcon;
        }
    }

    void SetPoint(Transform startPoint) {
        pointMove1 = startPoint.position;
        pointMove1.y -= 10f;

        pointMove2 = startPoint.position;
        pointMove2.x -= 20f;

        pointMove3 = startPoint.position;
        pointMove3.x -= 30f;
        pointMove3.y -= 10f;
    }

    IEnumerator IE_WaitToMoveTarget()
    {
        yield return new WaitForSeconds(.5f);
        trsMove.DOMove(target.position, 0.5f);
        trsMove.DOScale(Vector3.zero, 0.5f).OnComplete(() => {
             if (actionDone != null)
                actionDone();
            gameObject.SetActive(false);
        });
    }

    void Move() {
        trsMove.DOMove(pointMove1, 0.1f).OnComplete(() => {
            trsMove.DOMove(pointMove2, 0.1f).OnComplete(() => {
                trsMove.DOMove(pointMove3, 0.1f).OnComplete(() => {
                    textCountCanvas.DOFade(0, 0.1f);
                    StartCoroutine(IE_WaitToMoveTarget());
                });
            });
        });
    }
}
