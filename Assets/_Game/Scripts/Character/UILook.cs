using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILook : MonoBehaviour
{
    Vector3 lookAngle = new Vector3(40f, 45, 0f);
    private void Update()
    {
        transform.eulerAngles = lookAngle;
    }
}
