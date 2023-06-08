using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDiceTexture : MonoBehaviour
{
    public Texture2D Front;
    public Texture2D Top;
    public Texture2D Back;
    public Texture2D Bottom;
    public Texture2D Left;
    public Texture2D Right;

    public Cubemap Cubemap;
    public Texture2D[] FaceTextures;
    private Texture2D[] AllCubeTextures;
    public Texture2D TextureAtlas;
    public Rect[] AtlasUvs;

    private void Start()
    {
        if(TryGetComponent(out MeshRenderer meshRenderer)) { }

        if (TryGetComponent(out MeshFilter meshFilter))
        {
            Mesh mesh = meshFilter.mesh;

            if (mesh == null || mesh.uv.Length != 24)
            {
                Debug.Log("Script needs to be attached to built-in cube");
                return;
            }

            AllCubeTextures = new Texture2D[] { Front, Top, Back, Bottom, Left, Right };

            TextureAtlas = new Texture2D(192, 192);
            AtlasUvs = TextureAtlas.PackTextures(AllCubeTextures, 0);

            for (int i = 0; i < AtlasUvs.Length; i++)
            {
                var atlasUv = AtlasUvs[i];
                mesh.uv[i] = atlasUv.position;
                mesh.uv[i + 1] = new Vector2(atlasUv.xMax, atlasUv.yMin);
                mesh.uv[i + 2] = new Vector2(atlasUv.xMin, atlasUv.xMax);
                mesh.uv[i + 3] = new Vector2(atlasUv.xMax, atlasUv.xMax);
            }

            meshRenderer.material.SetTexture("_Texture2D", TextureAtlas);
        }
        else
        {
            Debug.Log("Need MeshFilterComponent");
        }
    }
}
