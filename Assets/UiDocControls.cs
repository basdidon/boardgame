using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class UiDocControls : SerializedMonoBehaviour
{
    public static UiDocControls Instance { get; private set; }

    public ScriptableUiPanel scriptablePanel;

    public Dictionary<string,UiDocHandler> uiDict;

    UiDocHandler ActivatedUiDoc { get; set; }
    public void SetActiveUiDoc(string name)
    {
        if (uiDict.TryGetValue(name,out UiDocHandler handler))
        {
            ActivatedUiDoc?.Deactive();
            ActivatedUiDoc = handler;
            ActivatedUiDoc.Active();
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        uiDict = new Dictionary<string, UiDocHandler>();
        scriptablePanel.Intialize();
        SetActiveUiDoc("LogIn");
    }
}
