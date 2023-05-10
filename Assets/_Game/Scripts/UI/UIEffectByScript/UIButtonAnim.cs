using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class UIButtonAnim : MonoBehaviour, IPointerDownHandler
{
    bool onAnim;
    [SerializeField] Vector3 vectorScale;
    [SerializeField] Vector3 vectorDefault = new Vector3(1, 1, 1);
    [SerializeField] Vector3 vectorRotage = new Vector3(0, 0, 5f);
    float pointY;
    public void OnPointerDown(PointerEventData eventData)
    {
        OnActive();
    }

    public void OnActive() {
        if (onAnim)
            return;
        onAnim = true;
        transform.localScale = vectorDefault;
        transform.DORotate(vectorRotage, .1f).OnComplete(() => {
            transform.DORotate(Vector3.zero, .1f);
        });
        transform.DOScale(vectorScale, .1f).OnComplete(()=> {
            transform.DOScale(new Vector3(1.1f, 0.9f, 1f), .1f).OnComplete(()=> {
                transform.DOScale(vectorDefault, .1f).OnComplete(() =>
                {
                    onAnim = false;
                });
            });
        });
    }

    public void OnActive2(float offSetY = 50f) {
        if (onAnim)
            return;
        onAnim = true;
        transform.localScale = vectorDefault;
        transform.DORotate(vectorRotage, .25f).OnComplete(() => {
            transform.DORotate(Vector3.zero, .25f);
        });
        transform.DOScale(vectorScale, .25f).OnComplete(() => {
            transform.DOScale(new Vector3(1.1f, 0.9f, 1f), .25f).OnComplete(() => {
                transform.DOScale(vectorDefault, .25f).OnComplete(() =>
                {
                    onAnim = false;
                });
            });
        });
        pointY = transform.position.y + offSetY;
        transform.DOMoveY(pointY, .25f).OnComplete(()=> {
            transform.DOMoveY(pointY - offSetY, .25f);
        });
    }
}