using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[CreateAssetMenu(fileName = "newNodeData", menuName = "ScriptableObject/NodeData")]
public class NodeType : SerializedScriptableObject
{
    [OdinSerialize] public List<Texture2D> Texture2DList { get; set; }
    [OdinSerialize] public bool IsWalkable { get; set; }

    public Texture2D GetTexture2D()
    {
        return Texture2DList[Random.Range(0, Texture2DList.Count)];
    }
}