using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;

public enum UiMenus
{
    Login,
    Lobby,
    Room,
}

[CreateAssetMenu(menuName = "DocUi",fileName = "DocUiElement")]
public class ScriptableUiPanel : SerializedScriptableObject
{
    public Dictionary<UiMenus, VisualTreeAsset> uiDocs;
}
