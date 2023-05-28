using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

[RequireComponent(typeof(UIDocument))]
public class UiDocControls : SerializedMonoBehaviour
{
    public static UiDocControls Instance { get; private set; }
    public UIDocument UIDocument { get; private set; }

    public ScriptableUiPanel ScriptableUiPanel;
    public Dictionary<UiMenus, MonoBehaviour> handlers;

    public void ActiveUiDoc(UiMenus uiMenus)
    {
        if (ScriptableUiPanel.uiDocs.TryGetValue(uiMenus, out VisualTreeAsset asset))
        {
            UIDocument.visualTreeAsset = asset;
        }

        foreach (var component in handlers.Values)
        {
            component.enabled = false;
        }

        if (handlers.TryGetValue(uiMenus, out MonoBehaviour handler))
        {
            handler.enabled = true;
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        UIDocument = GetComponent<UIDocument>();
    }
    
    private void Start()
    {
        handlers = new Dictionary<UiMenus, MonoBehaviour>
        {
            { UiMenus.Login, gameObject.AddComponent<LoginUiHandler>() }
        };

        ActiveUiDoc(UiMenus.Login);
    }
}
