using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertClicker : MonoBehaviour
{
    [SerializeField] IMachineController machineController;

    public void AlertOnClick()
    {
        UIManager.instance.ShowPanelMachineInfor(machineController);
        machineController.OpenControlerInfo();
    }

    public void SetControler(IMachineController controller)
    {
        machineController = controller;
    }
}
