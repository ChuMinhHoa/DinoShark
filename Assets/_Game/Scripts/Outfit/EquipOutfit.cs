using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipOutfit : MonoBehaviour
{
    //[SerializeField] MeshFilter hatMesh;
    [SerializeField] Transform hatPos;
    [SerializeField] GameObject hat;
    //[SerializeField] MeshFilter clothesMesh;
    [SerializeField] Transform clothesPos;
    [SerializeField] GameObject clothes;
    //[SerializeField] MeshFilter toolMesh;
    [SerializeField] Transform toolPos;
    [SerializeField] GameObject tool;

    Vector3 hatScale = new Vector3(1f, 1f, 1f);

    public void Equip(int outfitID, OutfitType outfitType) {
        OutfitData outfitData = ProfileManager.Instance.dataConfig.outfitDataConfig.GetOutfitData(outfitID, outfitType);
        if (outfitData.outfitObj == null)
            return;
        switch (outfitType)
        {
            case OutfitType.Hat:
                //hatMesh.mesh = outfitData.mesh;
                Destroy(hat);
                hat = Instantiate(outfitData.outfitObj, hatPos);
                break;
            case OutfitType.Clothes:
                //clothesMesh.mesh = outfitData.mesh;
                Destroy(clothes);
                clothes = Instantiate(outfitData.outfitObj, clothesPos);
                break;
            case OutfitType.Tool:
                //toolMesh.mesh = outfitData.mesh;
                Destroy(tool);
                tool = Instantiate(outfitData.outfitObj, toolPos);
                break;
            default:
                break;
        }
    }

    public void EquipDefault(OutfitType outfitType)
    {
        GameObject outfit= ProfileManager.Instance.dataConfig.outfitDataConfig.GetDefaultOutfitByType(outfitType);
        if (outfit == null)
            return;
        switch (outfitType)
        {
            case OutfitType.Hat:
                Destroy(hat);
                hat = Instantiate(outfit, hatPos);
                break;
            case OutfitType.Clothes:
                Destroy(clothes);
                clothes = Instantiate(outfit, clothesPos);
                break;
            case OutfitType.Tool:
                Destroy(tool);
                tool = Instantiate(outfit, toolPos);
                break;
            default:
                break;
        }
    }

    public void InitOutfit()
    {
        //hatMesh.transform.localScale = hatScale;
        //toolMesh.gameObject.SetActive(true);
        tool.SetActive(true);
        hat.transform.localScale = hatScale;
    }
}
