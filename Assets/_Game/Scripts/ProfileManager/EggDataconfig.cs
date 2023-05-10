using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EggDataconfig", menuName = "ScriptAbleObjects/New EggDataconfig")]
public class EggDataconfig : ScriptableObject
{
    public List<Egg> eggType;

    public Egg GetEggType() {
        int index = Random.Range(0, eggType.Count);
        return eggType[index];
    }
}
