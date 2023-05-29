using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UiDocHandler : MonoBehaviour
{
    public UIDocument UiDoc { get; protected set; }

    public virtual void Show()
    {
        UiDoc.rootVisualElement.style.display = DisplayStyle.Flex;
    }
    public virtual void Hide()
    {
        UiDoc.rootVisualElement.style.display = DisplayStyle.Flex;
    }
}

