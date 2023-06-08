using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "ScriptableObject/Dice/DiceFace", fileName ="DiceFace")]
public class DiceFaceData : SerializedScriptableObject
{
    [field: SerializeField] public string Name { get; set; }
    [field: SerializeField] public Sprite Sprite { get; set; }
}
