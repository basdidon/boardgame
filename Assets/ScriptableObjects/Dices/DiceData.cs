using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName ="ScriptableObject/Dice/Dice")]
public class DiceData : SerializedScriptableObject
{
    public DiceFaceData[] diceFaces = new DiceFaceData[6];
}
