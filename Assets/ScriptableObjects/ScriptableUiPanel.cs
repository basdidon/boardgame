using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;

[CreateAssetMenu(menuName = "DocUi",fileName = "DocUiElement")]
public class ScriptableUiPanel : SerializedScriptableObject
{
    public Dictionary<string, GameObject> uiDocPrefab;

    public void Intialize()
    {
        foreach(var pair in uiDocPrefab)
        {
            var go = Instantiate(pair.Value,UiDocControls.Instance.transform);
            UiDocControls.Instance.uiDict.Add(pair.Key, go.GetComponent<UiDocHandler>());
            go.SetActive(false);
        }
    }
}
