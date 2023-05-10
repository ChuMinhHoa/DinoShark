using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class StreetLine
{
    public List<Transform> pointInLine;
    public bool lineFree;
}
public class CarDecoreManager : MonoBehaviour
{
    public List<CarDecore> carDecores;
    public List<StreetLine> streetLines;
    float timeIdle = 3f;
    private void Awake()
    {
        for (int i = 0; i < carDecores.Count; i++)
        {
            carDecores[i].SetTimeIdleSetting(timeIdle + i);
        }
    }
    public void Update()
    {
        for (int i = 0; i < carDecores.Count; i++)
        {
            if (carDecores[i].onIdle && carDecores[i].timeIdle >= carDecores[i].timeIdleSetting)
            {
                carDecores[i].SetStreetLine(streetLines[i % 2]);
            }
        }
    }
}
