using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    [SerializeField] GameObject lockArea;
    [SerializeField] GameObject mainArea;
    [SerializeField] GameObject deliveryBox;
    [SerializeField] GameObject unlockSmoke;

    public void InitArea()
    {
        mainArea.SetActive(true);
        lockArea.SetActive(false);
        GetComponent<Collider>().enabled = false;
    }
    public void DisableArea()
    {
        mainArea.SetActive(false);
        deliveryBox.SetActive(false);
        unlockSmoke.SetActive(false);
        lockArea.SetActive(true);
    }

    public void AreaOnDelivery()
    {
        gameObject.SetActive(true);
        deliveryBox.SetActive(true);
        unlockSmoke.SetActive(false);
    }
    public void OnUnbox()
    {
        deliveryBox.SetActive(false);
        unlockSmoke.SetActive(true);
        
        InitArea();
    }
#if UNITY_EDITOR
    void OnMouseDown()
    {
        OnUnbox();
    }
#endif
    void OnTouchDown()
    {
        OnUnbox();
    }
}
