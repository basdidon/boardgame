using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

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
