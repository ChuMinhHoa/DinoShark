using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MachineStatus
{
    Lock,
    Box,
    CanUsing
}
public class Machine : MonoBehaviour
{
    [SerializeField] Transform pointUsing;
    [SerializeField] bool free;
    [SerializeField] int levelActive;
    [SerializeField] BoxMachine boxMachine;
    [SerializeField] GameObject objBox;
    [SerializeField] GameObject objMachine;
    [SerializeField] GameObject smokeEffect;
    IMachineController machineController;
    [SerializeField] Collider myCollider;
    [SerializeField] Animator animator;
    MachineStatus currentStatus = MachineStatus.Lock;
    [SerializeField] GameObject objEffect;

    private void Start()
    {
        boxMachine.ChangeMymachine(this);
        objEffect.SetActive(false);
    }

    public Transform GetPointUsingMachine() {
        return pointUsing;
    }

    public void UsingMachine() { free = false; }

    public void ResetMachine() { free = true; }

    public bool IsFree() { return free; }

    public int GetLevelActive() { return levelActive; }

    public void SetLevelActive(int level) { levelActive = level ; }

    public void ChangeMachineStatus(MachineStatus machineStatus) {
        switch (machineStatus)
        {
            case MachineStatus.Lock:
                myCollider.enabled = false;
                objBox.SetActive(false);
                objMachine.SetActive(false);
                free = false;
                break;
            case MachineStatus.Box:
                myCollider.enabled = false;
                objBox.SetActive(true);
                objMachine.SetActive(false);
                free = false;
                break;
            case MachineStatus.CanUsing:
                myCollider.enabled = true;
                objBox.SetActive(false);
                objMachine.SetActive(true);
                free = true;
                GameManager.Instance.lobbyManager.CallFreeChefDoWork();
                break;
            default:
                break;
        }
    }

    public void SmokeEffectNow() {
        smokeEffect.SetActive(true);
        StartCoroutine(IE_WaitToturnOfSmoke());
    }

    IEnumerator IE_WaitToturnOfSmoke() {
        yield return new WaitForSeconds(1.5f);
        smokeEffect.SetActive(false);
        currentStatus = MachineStatus.CanUsing;
    }

    public void SetMachineController(IMachineController controller)
    {
        machineController = controller;
    }
    public void MachineOnClick()
    {
        UIManager.instance.ShowPanelMachineInfor(machineController);
        if (currentStatus == MachineStatus.CanUsing) machineController.OpenControlerInfo();
    }

    public void UseMachine()
    {
        animator.SetBool("Using", true);
        objEffect.SetActive(true);
    }

    public void DoneUseMachine()
    {
        animator.SetBool("Using", false);
        objEffect.SetActive(false);
    }
}
