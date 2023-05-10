using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

public enum RawType { 
    RawSkinPanel,
    RawInvite,
    RawDaily
}

[System.Serializable]
public class RawController {
    public RawType rawType;
    public List<MyRaw> myRaws;

    public MyRaw GetMyRaw(int indexRaw) { return myRaws[indexRaw]; }
}

[System.Serializable]
public class MyRaw {
    public RenderTexture rawSkin;
    public Camera cameraSkin;
    public Transform model;
    public EquipOutfit equipOutfit;
    public Animator anim;
}

public class RawImageManager : MonoBehaviour
{
    public static RawImageManager Instance;
    [SerializeField] EquipOutfit equipOutfit;
    [SerializeField] Transform staffTransform;
    private void Awake()
    {
        Instance = this;
    }
    [SerializeField] List<RawController> rawControllers;
    RawController GetRawController(RawType rawType) {
        for (int i = 0; i < rawControllers.Count; i++)
        {
            if (rawControllers[i].rawType == rawType)
                return rawControllers[i];
        }
        return null;
    }
    RawController rawController;
    public MyRaw GetMyRaw(RawType rawType,int indexRaw) {
        rawController = GetRawController(rawType);
        return rawController.GetMyRaw(indexRaw);
    }

    public EquipOutfit GetEquipOutfit()
    {
        return equipOutfit;
    }

    public Transform GetStaffTransform()
    {
        return staffTransform;
    }
}
