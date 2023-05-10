using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BoxMachine : MonoBehaviour
{
    Machine myMachine;

    public void ChangeMymachine(Machine machine) {
        myMachine = machine;
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        myMachine.ChangeMachineStatus(MachineStatus.CanUsing);
        myMachine.SmokeEffectNow();
    }
}
