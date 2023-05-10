using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
[System.Serializable]
public class Floor {
    public int upgradeIDCondition;
    public List<Transform> trsObjectFloors;
    public Table firstTableOfFloor;
    Vector3 target;

    public void Init() {
        for (int i = 0; i < trsObjectFloors.Count; i++)
            trsObjectFloors[i].gameObject.SetActive(false);
    }

    public void AbleObject() {
        for (int i = 0; i < trsObjectFloors.Count; i++)
            trsObjectFloors[i].gameObject.SetActive(true);
    }

    public void MoveObject(int objectIndex, UnityAction actionDone = null) {
        target = trsObjectFloors[objectIndex].position;
        target.y += 10f;
        trsObjectFloors[objectIndex].position = target;
        target.y -= 10f;
        trsObjectFloors[objectIndex].gameObject.SetActive(true);
        trsObjectFloors[objectIndex].DOMove(target, 1f).OnComplete(()=> {
            if (actionDone != null)
                actionDone();
        });
    }
}

public class FloorManager : MonoBehaviour
{
    public List<Floor> floors;
    public void InitFloorManager()
    {
        for (int i = 0; i < floors.Count; i++)
        {
            floors[i].Init();
        }
    }

    int tableRef;
    int floorIndex;

    public void UnlockFloor(int floorIndex, int tableRef) {
        indexObject = 0;
        this.floorIndex = floorIndex;
        StartCoroutine(IE_WaitToMoveNextObject(floorIndex));
        this.tableRef = tableRef;
    }

    int indexObject;
    IEnumerator IE_WaitToMoveNextObject(int floorIndex) {
        while (indexObject < floors[0].trsObjectFloors.Count)
        {
            if (indexObject == floors[floorIndex].trsObjectFloors.Count - 1)
            {
                floors[floorIndex].MoveObject(indexObject, AnimDone);
            }
            else
                floors[floorIndex].MoveObject(indexObject);
            yield return new WaitForSeconds(.25f);
            indexObject++;
        }
    }

    void AnimDone() {
        GameManager.Instance.lobbyManager.BuildTable(tableRef);
        ProfileManager.Instance.playerData.GetUpgradeSave().AddCustomer(floors[floorIndex].firstTableOfFloor.servingPositions.Count);
        GameManager.Instance.lobbyManager.MoveTableOnFollowFloor(tableRef);
        GameManager.Instance.lobbyManager.SpawnNewWaitor();
    }

    public void InitFloor() {
        for (int i = 0; i < floors.Count; i++)
        {
            if (ProfileManager.Instance.playerData.GetUpgradeSave().IsFloorUnlocked(i))
            {
                floors[i].AbleObject();
            }
        }
    }

    public void AddCustomerToFirstTable(int floorIndex) {
        ProfileManager.Instance.playerData.GetUpgradeSave().AddCustomer(floors[floorIndex].firstTableOfFloor.servingPositions.Count);
    }
}
