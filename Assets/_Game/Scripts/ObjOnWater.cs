using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjOnWater : MonoBehaviour
{
    [SerializeField] float waveStrength;
    [SerializeField] float limitRotageX;
    [SerializeField] float limitRotageZ;
    Vector3 vectorRotage = Vector3.zero;
    Vector3 vectorPos = Vector3.zero;
    [SerializeField] float timeRotage;
    [SerializeField] float posY;
    [SerializeField] float speed;
    bool pros = true;
    Transform myTransform;
    [SerializeField] AnimationCurve rotageCurve;
    [SerializeField] AnimationCurve positionCurve;
    private void Start()
    {
        myTransform = transform;
        vectorPos = myTransform.position;
        posY = vectorPos.y;
        timeRotage = Random.Range(0f, 1f);
    }
    private void Update()
    {
        vectorRotage.x = Mathf.Lerp(-limitRotageX, limitRotageX, rotageCurve.Evaluate(timeRotage));
        vectorRotage.z = Mathf.Lerp(+limitRotageZ, -limitRotageZ, rotageCurve.Evaluate(timeRotage));
        vectorPos.y = Mathf.Lerp(posY - waveStrength, posY + waveStrength, positionCurve.Evaluate(timeRotage));
        transform.eulerAngles = vectorRotage;
        transform.position = vectorPos;
        if (pros && timeRotage >= 1f)
        {
            pros = false;
        }
        else if (!pros && timeRotage <= 0f)
        {
            pros = true;
        }

        if (pros)
            timeRotage += Time.deltaTime * speed;
        else timeRotage -= Time.deltaTime * speed;
    }
}
