using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

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

public class ProceduralMapGenerator : SerializedMonoBehaviour
{
    [BoxGroup("Dimensions", Order = -10)]
    [SerializeField] Vector3Int mapSize = new Vector3Int(100, 0, 100);
    [BoxGroup("Dimensions")]  public float scale = 1.0f;
    [BoxGroup("Dimensions")]  public Vector2 offset;

    [BoxGroup("Waves", Order = -9)]
    [PropertyOrder(-2)]
    public bool isOneSeed;
    [ShowIf("isOneSeed")]
    [OdinSerialize]
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
    [Header("Height Map")]
    public Wave[] heightWaves;
    private float[,] heightMap;
    [BoxGroup("Waves")]
    [Header("Moisture Map")]
    public Wave[] moistureWaves;
    private float[,] moistureMap;
    [BoxGroup("Waves")]
    [Header("Heat Map")]
    public Wave[] heatWaves;
    private float[,] heatMap;

    [Space]
    [OdinSerialize]
    [ShowInInspector]
    Dictionary<Vector3Int, GameObject> cloneDict;

    [SerializeField] float dalaySpawn = .2f;
    public bool isFallInTiles;
    [ShowIfGroup("isFallInTiles")]
    [BoxGroup("isFallInTiles/FallInTilesProps")]
    [SerializeField] float fallingHeight = 5f, fallingDuration = 1f;

    private void Awake()
    {
        cloneDict = new Dictionary<Vector3Int, GameObject>();
    }

    /*  - genlevel
     *  -- spawn
     *  -- spawnWithFallAnimation
     *
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

    GameObject InstatiateCellTile(Vector3Int cellPos)
    {
        var x = cellPos.x;
        var z = cellPos.z;
        Vector3Int cellPosion = new Vector3Int(x, 0, z);
        GameObject biomeTile = GetBiome(heightMap[x, z], moistureMap[x, z], heatMap[x, z]).GetTilePrefab();
        GameObject clone = Instantiate(biomeTile, cellPosion, Quaternion.identity, transform);
        clone.name = string.Format("Node {0},{1}", x, z);
        cloneDict.Add(cellPosion, clone);
        return clone;
    }

    [Button]
    public void SpawnTiles(bool isNewSeed)
    {
        if (isNewSeed) RandomOneSeed();
        ResetLevel();
        GenerateMap();

        for (int x = 0; x < mapSize.x; ++x)
        {
            for (int z = 0; z < mapSize.z; ++z)
            {
                InstatiateCellTile(new Vector3Int(x,0,z));
            }
        }
    }

    [Button]
    public void SpawnTilesDelay(bool isNewSeed)
    {
        if (isNewSeed) RandomOneSeed();
        ResetLevel();
        GenerateMap();

        StartCoroutine(SpawnTile(Vector3Int.zero));
    }

    [Button]
    public void SpawnTilesWaveDelay(bool isNewSeed)
    {
        if(isNewSeed) RandomOneSeed();
        ResetLevel();
        GenerateMap();

        StartCoroutine(WaveSpawnTiles());
    }

    [ButtonGroup("genLevel")]
    public void ResetLevel()
    {
        foreach (var t in cloneDict.Values)
        {
            Destroy(t);
        }
        cloneDict.Clear();
    }

    BiomePreset GetBiome(float height, float moisture, float heat)
    {
        List<BiomeTempData> biomeTemp = new List<BiomeTempData>();

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

        if(biomeToReturn == null)
        {
            biomeToReturn = biomes[0];
            Debug.Log("returnDefaultTile");
        }

        return biomeToReturn;
    }

    IEnumerator WaveSpawnTiles(int waveCount=0)
    {
        List<Vector3Int> cellsPos = new List<Vector3Int>();
        // where cellPos.x + cellPos.y == c , instantiate it
        for(int x = 0; x < mapSize.x; x++)
        {
            if (x > waveCount) continue;

            for (int z = 0; z < mapSize.z; z++)
            {
                Debug.Log($"searching on : {x},{z}");
                if (x + z == waveCount)
                {
                    cellsPos.Add(new Vector3Int(x, 0, z));
                    break;
                }
            }
        }

        yield return new WaitForSeconds(dalaySpawn);
        foreach(var cell in cellsPos)
        {
            var clone = InstatiateCellTile(cell);
            if (isFallInTiles)
                StartCoroutine(FallInTile(clone.transform));
        }

        if (waveCount < mapSize.x + mapSize.z)
            yield return WaveSpawnTiles(waveCount + 1);
        else
            yield return null;
    }

    IEnumerator SpawnTile(Vector3Int cellPos)
    {
        var clone = InstatiateCellTile(cellPos);

        if (isFallInTiles)
            StartCoroutine(FallInTile(clone.transform));

        if (cellPos.x + 1 < mapSize.x)
        {
            yield return new WaitForSeconds(dalaySpawn);
            yield return SpawnTile(cellPos + Vector3Int.right);
        }
        else
        {
            if (cellPos.z + 1 < mapSize.z)
            {
                yield return new WaitForSeconds(dalaySpawn);
                yield return SpawnTile(new Vector3Int(0, cellPos.y, cellPos.z + 1));
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator FallInTile(Transform transform)
    {
        Vector3 startPos = transform.position + Vector3.up * fallingHeight;
        Vector3 targetPos = transform.position;

        float timeElapsed = 0f;

        while (timeElapsed < fallingDuration)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, timeElapsed / fallingDuration);
            timeElapsed += Time.deltaTime;
            yield return null;
            transform.position = targetPos;
        }
    }
}