using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class UiDocControls : SerializedMonoBehaviour
{
    public static UiDocControls Instance { get; private set; }
    public UIDocument UIDocument { get; private set; }

    [OdinSerialize] public MenuUiScriptableObject ScriptableMenuUi { get; set; }
    [OdinSerialize] public Dictionary<UiMenus, UiDocHandler> DocHandlers { get; set; }
    
    public void ActiveUiDoc(UiMenus uiMenus)
    {
        foreach (var _handler in DocHandlers.Values)   
            _handler.Hide();

        if (DocHandlers.TryGetValue(uiMenus, out UiDocHandler handler))
            handler.Show();
        
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        UIDocument = GetComponent<UIDocument>();
        DocHandlers = new Dictionary<UiMenus, UiDocHandler>();

        foreach (var menuUi in ScriptableMenuUi.DocUiPrefabs)
        {
            var clone = Instantiate(menuUi.Value, transform);

            if (clone.TryGetComponent(out UiDocHandler handler))
            {
                DocHandlers.Add(menuUi.Key, handler);
            }
        }
    }
    
    private void Start()
    {
        ActiveUiDoc(UiMenus.Login);
    }
}
