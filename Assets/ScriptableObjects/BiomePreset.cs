using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(fileName = "NewBiome", menuName = "ScriptableObject/Biome")]
public class BiomePreset : SerializedScriptableObject
{
    [OdinSerialize] public NodeType[] NodeTypes { private get; set; }
    public float minHeight;
    public float minMoisture;
    public float minHeat;

    public NodeType GetNodeType()
    {
        return NodeTypes[Random.Range(0, NodeTypes.Length)];
    }

    public bool MatchCondition(float height, float moisture, float heat)
    {
        return height >= minHeight && moisture >= minMoisture && heat >= minHeat;
    }
}
