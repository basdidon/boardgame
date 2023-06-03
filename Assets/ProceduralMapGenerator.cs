using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Fusion;

[System.Serializable]
public class Wave
{
    public float seed;
    [Range(0f, 1f)] public float frequency;
    [Range(0f,1f)] public float amplitude;
}

public class BiomeTempData
{
    public BiomePreset biome;
    public BiomeTempData(BiomePreset preset)
    {
        biome = preset;
    }

    public float GetDiffValue(float height, float moisture, float heat)
    {
        return (height - biome.minHeight) + (moisture - biome.minMoisture) + (heat - biome.minHeat);
    }
}

public class ProceduralMapGenerator : NetworkBehaviour
{
    public static ProceduralMapGenerator Instance { get; private set; }

    [BoxGroup("EmptyTile",Order = -11)]
    [SerializeField] GameObject emptyTilePrefab;

    [BoxGroup("Dimensions", Order = -10)]
    [SerializeField] Vector3Int mapSize = new(100, 0, 100);
    [BoxGroup("Dimensions")]  public float scale = 1.0f;
    [BoxGroup("Dimensions")]  public Vector2 offset;

    [BoxGroup("Waves", Order = -9)]
    [PropertyOrder(-3)]
    public bool isOneSeed;
    [ShowIf("isOneSeed")]
    [BoxGroup("Waves")]
    [PropertyOrder(-2)]
    public static bool isNewSeed = false;
    [ShowIf("isOneSeed")]
    [BoxGroup("Waves")]
    [PropertyOrder(-1)]
    [OnValueChanged("OneSeedChanged")]
    [InlineButton("RandomOneSeed", SdfIconType.Dice6Fill, "Random")]
    public int OneSeed { get; set; }
    public void OneSeedChanged(int newValue) 
    {
        IEnumerator[] enumerators = { heightWaves.GetEnumerator(), heatWaves.GetEnumerator(), moistureWaves.GetEnumerator() };
        foreach(var e in enumerators)
            while (e.MoveNext())
                (e.Current as Wave).seed = newValue;    
    }
    public void RandomOneSeed() 
    {
        OneSeed = Random.Range(0, 10000);
        OneSeedChanged(OneSeed);
    }
    
    [BoxGroup("Waves")]
    [Header("Biomes")]
    public BiomePreset[] biomes;

    [BoxGroup("Waves")]
    [Header("Height Waves")]
    public Wave[] heightWaves;
    private float[,] heightMap;
    [BoxGroup("Waves")]
    [Header("Moisture Waves")]
    public Wave[] moistureWaves;
    private float[,] moistureMap;
    [BoxGroup("Waves")]
    [Header("Heat Waves")]
    public Wave[] heatWaves;
    private float[,] heatMap;

    public List<NodePreset> NodePresets;

    [Space]
    [ShowInInspector]
    Dictionary<Vector3Int, GameObject> cloneDict;

    [SerializeField] float dalaySpawn = .2f;
    public bool isFallInTiles;
    [ShowIfGroup("isFallInTiles")]
    [BoxGroup("isFallInTiles/FallInTilesProps")]
    [SerializeField] float fallingHeight = 5f, fallingDuration = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null && Instance != this)
        {
            Destroy(this);
        }

        cloneDict = new Dictionary<Vector3Int, GameObject>();

    }

    /*  - genlevel
     *  -- spawn
     *  -- spawnWithFallAnimation
     */

    public void GenerateMap()
    {
        // height map
        heightMap = NoiseGenerator.Generate(mapSize.x, mapSize.z, scale, heightWaves, offset);
        // moisture map
        moistureMap = NoiseGenerator.Generate(mapSize.x, mapSize.z, scale, moistureWaves, offset);
        // heat map
        heatMap = NoiseGenerator.Generate(mapSize.x, mapSize.z, scale, heatWaves, offset);
    }

    [ButtonGroup("genLevel")]
    public void ResetLevel()
    {
        foreach (var t in cloneDict.Values)
        {
            Destroy(t);
        }
        cloneDict.Clear();

        if (isNewSeed)
        {
            RandomOneSeed();
            GenerateMap();
        }else if (heightMap == null || moistureMap == null || heatMap == null)
        {
            GenerateMap();
        }
    }

    public BiomePreset GetBiome(float height, float moisture, float heat)
    {
        List<BiomeTempData> biomeTemp = new();

        foreach (BiomePreset biome in biomes)
        {
            if (biome.MatchCondition(height, moisture, heat))
            {
                biomeTemp.Add(new BiomeTempData(biome));
            }
        }

        float curVal = Mathf.Infinity;
        BiomePreset biomeToReturn = null;

        foreach (BiomeTempData biome in biomeTemp)
        {
            if (biome.GetDiffValue(height, moisture, heat) < curVal)
            {
                biomeToReturn = biome.biome;
                curVal = biome.GetDiffValue(height, moisture, heat);
            }
        }

        if (biomeToReturn == null)
        {
            biomeToReturn = biomes[0];
            Debug.Log("returnDefaultTile");
        }
        else
        {
            Debug.Log(biomeToReturn.name);
        }

        return biomeToReturn;
        
    }

    public BiomePreset GetBiomeByCellPos(Vector3Int cellPos)
    {
        Debug.Log("GetBiome(Vector3Int cellPos)");
        int x = cellPos.x;
        int z = cellPos.z;
        
        return GetBiome(heightMap[x, z], moistureMap[x, z], heatMap[x, z]);
    }

    IEnumerator FallInTile(Transform transform)
    {
        Vector3 startPos = transform.position + Vector3.up * fallingHeight;
        Vector3 targetPos = transform.position;

        float timeElapsed = 0f;

        while (timeElapsed < fallingDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, timeElapsed / fallingDuration);
            yield return null;
            timeElapsed += Time.deltaTime;
            transform.position = targetPos;
        }
    }

    public NetworkObject NetworkSpawnCellTile(Vector3Int cellPos)
    {
        Debug.Log("NetworkSpawnEmptyTile()");
        var x = cellPos.x;
        var z = cellPos.z;

        var clone = NetworkSpawner.Instance.Runner.Spawn(emptyTilePrefab,cellPos,Quaternion.identity);
        if (clone != null)
        {
            clone.name = string.Format("Node {0},{1}", x, z);
            clone.transform.SetParent(transform);

            if(clone.TryGetBehaviour(out Node node))
                node.SetUpNode(cellPos);
            else
                Debug.LogError("Not Found [Node Behaviour]");
            
            cloneDict.Add(cellPos, clone.gameObject);
        }

        return clone;
    }

    [Button]
    public void NetworkSpawnTiles()
    {
        ResetLevel();
        for (int x = 0; x < mapSize.x; ++x)
            for (int z = 0; z < mapSize.z; ++z)
                NetworkSpawnCellTile(new Vector3Int(x, 0, z));
    }

    [Button]
    public void NetworkWaveSpawnTiles() {
        ResetLevel();
        StartCoroutine(NetworkWaveSpawnTilesCoroutine()); 
    }

    IEnumerator NetworkWaveSpawnTilesCoroutine(int waveCount = 0)
    {
        Debug.Log($"IENetworkWaveSpawnTiles:{waveCount}");
        List<Vector3Int> cellsPos = new();

        // where cellPos.x + cellPos.y == waveCount , instantiate it
        for (int x = 0; x < mapSize.x; x++)
        {
            if (x > waveCount) continue;

            for (int z = 0; z < mapSize.z; z++)
            {
                if (x + z == waveCount)
                {
                    cellsPos.Add(new Vector3Int(x, 0, z));
                    break;
                }
            }
        }

        yield return new WaitForSeconds(dalaySpawn);
        foreach (var cell in cellsPos)
        {
            var clone = NetworkSpawnCellTile(cell);
            if (isFallInTiles)
                StartCoroutine(FallInTile(clone.transform));
        }

        if (waveCount < mapSize.x + mapSize.z)
            yield return NetworkWaveSpawnTilesCoroutine(waveCount + 1);
        else
            yield return null;
    }
}