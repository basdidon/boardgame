using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(fileName = "newNodePreset", menuName = "ScriptableObject/NodePreset")]
public class NodePreset : SerializedScriptableObject
{
    [OdinSerialize] public Dictionary<Vector3Int, NodeType> Nodes { get; set; }

    [OdinSerialize]
    [BoxGroup("GenerateFromOneNode")]
    public NodeType PrototypeNode { get; set; }
    [OdinSerialize]
    [BoxGroup("GenerateFromOneNode")]
    public Vector3Int StartAt { get; set; }
    [OdinSerialize]
    [BoxGroup("GenerateFromOneNode")]
    Vector3Int Size { get; set; }
    [Button]
    [BoxGroup("GenerateFromOneNode")]
    public void Generate()
    {
        Nodes = new();

        for(int z = 0; z < Size.z; z++)
        {
            for(int x = 0;x< Size.x; x++)
            {
                Nodes.Add(new Vector3Int(StartAt.x + x, 0,StartAt.z + z), PrototypeNode);
            }
        }
    }

    public bool TryGetValue(Vector3Int cellPos,out NodeType nodeData)
    {
        return Nodes.TryGetValue(cellPos,out nodeData);
    }
}
