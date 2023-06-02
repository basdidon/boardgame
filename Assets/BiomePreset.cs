using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(fileName = "NewBiome", menuName = "ScriptableObject/Biome")]
public class BiomePreset : SerializedScriptableObject
{
    public GameObject[] tilePrefabs;
    [OdinSerialize] public Texture2D Texture2D { get; set; }
    public float minHeight;
    public float minMoisture;
    public float minHeat;

    public GameObject GetTilePrefab()
    {
        return tilePrefabs[Random.Range(0, tilePrefabs.Length)];
    }

    public bool MatchCondition(float height, float moisture, float heat)
    {
        return height >= minHeight && moisture >= minMoisture && heat >= minHeat;
    }
}
