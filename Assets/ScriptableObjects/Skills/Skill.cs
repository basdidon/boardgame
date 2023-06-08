using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public abstract class Skill : SerializedScriptableObject
{
    public DiceFaceData[] cost;
    public abstract void TryGetCost(Player player);
    public abstract void Start();
    public abstract void OnActiveted();
}
