using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "SkinDataConfig", menuName = "ScriptAbleObjects/SkinDataConfig")]
public class SkinDataConfig : ScriptableObject
{
    public List<Material> outlineMaterials;
    public List<Material> faceMaterials;
    public Material GetRandomMaterial()
    {
        if (outlineMaterials.Count > 0)
        {
            return outlineMaterials[UnityEngine.Random.Range(0, outlineMaterials.Count)];
        }
        return null;
    }
    public Material GetRandomFaceMaterial()
    {
        if (faceMaterials.Count > 0)
        {
            return faceMaterials[UnityEngine.Random.Range(0, faceMaterials.Count)];
        }
        return null;
    }
}
