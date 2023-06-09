using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class MapDiceTexture : MonoBehaviour
{
    Inputs Inputs { get; set; }
    public MeshFilter MeshFilter { get; set; }
    public MeshRenderer MeshRenderer { get; set; }
    public Rigidbody Rigidbody { get; set; }

    public FaceTexturesDictionary FaceTextures;

    private readonly Texture2D[] AllCubeTextures = new Texture2D[6];
    public Texture2D TextureAtlas;
    public Rect[] AtlasUvs;

    [OnValueChanged("OnUvsChanged")]
    public Vector2[] uvs;


    [Header("Roll")]
    public float thust = 100f;
    public float torqueForceMin = 100f;
    public float torqueFroceMax = 10000f;

    [Button]
    public void UpdateUvs()
    {
        MeshFilter.mesh.uv = uvs;
    }

    public void OnUvsChanged(Vector2[] newValue)
    {
        MeshFilter.mesh.uv = newValue;
    }

    public enum Faces
    {
        Front,
        Top,
        Back,
        Bottom,
        Left,
        Right,
    }

    private int[] GetFaceUvIndex(Faces faces)
    {
        return faces switch
        {
            Faces.Front => new int[] { 0, 1, 2, 3 },
            Faces.Top => new int[] { 8, 9, 4, 5 },
            Faces.Back => new int[] { 7, 6, 11, 10 },
            Faces.Bottom => new int[] { 12, 15, 13, 14 },
            Faces.Left => new int[] { 16, 19, 17, 18 },
            Faces.Right => new int[] { 20, 23, 21, 22 },
            _ => new int[] { },
        };
    }

    private void OnEnable() => Inputs.Enable();
    private void OnDisable() => Inputs.Disable();

    private void Awake()
    {
        if (TryGetComponent(out MeshRenderer meshRenderer))
        {
            MeshRenderer = meshRenderer;
        }

        if (TryGetComponent(out MeshFilter meshFilter))
        {
            MeshFilter = meshFilter;
        }
        else
        {
            Debug.Log("Need MeshFilterComponent");
        }

        if (TryGetComponent(out Rigidbody rigidbody))
        {
            Rigidbody = rigidbody;
            Rigidbody.mass = 100f;
        }
        
        Inputs = new Inputs();
        Inputs.Dice.Roll.performed += _ =>
        {
            Debug.Log("Rolling");
            Rigidbody.AddForceAtPosition(Vector3.up * thust * Rigidbody.mass, Vector3.down);

            StartCoroutine(AddTorque(3f));

        };
    }

    IEnumerator AddTorque(float duration)
    {
        Vector3 torqueDir = new(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        float timeElapsed = 0;
        while(timeElapsed < duration)
        {
            Debug.Log("Tilting");
            Rigidbody.AddTorque(torqueDir.normalized * Random.Range(torqueForceMin, torqueFroceMax));
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        
    }

    private void Start()
    {

        if (TryGetComponent(out MeshFilter meshFilter))
        {
            MeshFilter = meshFilter;
            Mesh mesh = meshFilter.mesh;

            if (mesh == null || mesh.uv.Length != 24)
            {
                Debug.Log("Script needs to be attached to built-in cube");
                return;
            }

            foreach(var faceTexture in FaceTextures)
            {
                AllCubeTextures[(int)faceTexture.Key] = faceTexture.Value;
            }

            TextureAtlas = new Texture2D(192, 192);
            AtlasUvs = TextureAtlas.PackTextures(AllCubeTextures, 0,192);

            uvs = mesh.uv;

            Debug.Log(mesh.uv.Length);


            foreach (var faceTexture in FaceTextures)
            {
                var atlasUv = AtlasUvs[(int)faceTexture.Key];
                var faceUvIndexs = GetFaceUvIndex(faceTexture.Key);
                uvs[faceUvIndexs[0]] = new Vector2(atlasUv.xMin, atlasUv.yMin);
                uvs[faceUvIndexs[1]] = new Vector2(atlasUv.xMax, atlasUv.yMin);
                uvs[faceUvIndexs[2]] = new Vector2(atlasUv.xMin, atlasUv.yMax);
                uvs[faceUvIndexs[3]] = new Vector2(atlasUv.xMax, atlasUv.yMax);
            }
   
            mesh.uv = uvs;

            MeshRenderer.material.mainTexture = TextureAtlas;
        }
 
    }


}
