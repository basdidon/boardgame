using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[System.Serializable]
public class Dice
{
    [field:SerializeField] public DiceData DiceData { get; set; }
    [field:SerializeField] public DiceFaceData Result { get; set; }

    public void Roll()
    {
        Result = DiceData.diceFaces[Random.Range(0, DiceData.diceFaces.Length)];
    }

    public void Reset()
    {
        Result = null;
    }
}
