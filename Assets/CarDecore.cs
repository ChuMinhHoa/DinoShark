using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarDecore : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    public bool onIdle = true;
    public StreetLine myStreetLine;
    public float ditanceTurn;
    Vector3 currentPoint;
    int currentPointIndex;
    bool moveNow;
    public float timeIdle;
    public float timeIdleSetting;
    private void Update()
    {
        if (onIdle)
        {
            timeIdle += Time.deltaTime;
            return;
        }

        if (moveNow)
        {
            currentPoint = myStreetLine.pointInLine[currentPointIndex].position;
            CarMove(currentPoint);
        }

        if (Vector3.Distance(transform.position, currentPoint) <= ditanceTurn)
        {
            moveNow = true;
            currentPointIndex++;
        }

        if (currentPointIndex == myStreetLine.pointInLine.Count)
        {
            moveNow = false;
            currentPointIndex = 0;
            currentPoint = Vector3.zero;
            onIdle = true;
            timeIdle = 0;
        }
    }

    public void CarMove(Vector3 pointTarget) {
        agent.SetDestination(pointTarget);
        moveNow = false;
    }

    public void SetStreetLine(StreetLine streetLine) {
        myStreetLine = streetLine;
        moveNow = true;
        onIdle = false;
        transform.position = myStreetLine.pointInLine[0].position;
    }

    public void SetTimeIdleSetting(float timeSetting) {
        timeIdleSetting = timeSetting;
    }
}
