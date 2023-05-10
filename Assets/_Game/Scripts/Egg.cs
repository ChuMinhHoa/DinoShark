using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Egg : MonoBehaviour
{
    Charater myCharactor;
    public void SetMyCharactor(Charater charator) {
        this.myCharactor = charator;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        myCharactor.EggBroke();
    }
}
