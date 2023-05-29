using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public enum UiMenus
{
    Login,
    Lobby,
    Room,
}

[CreateAssetMenu(menuName = "DocUis", fileName = "DocUiElements")]
public class MenuUiScriptableObject : SerializedScriptableObject
{
    public Dictionary<UiMenus, GameObject> DocUiPrefabs;
}
