using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UiDocHandler : MonoBehaviour
{
    public UIDocument UiDoc { get; protected set; }

    public virtual void Active()
    {
        gameObject.SetActive(true);
    }
    public virtual void Deactive()
    {
        gameObject.SetActive(false);
    }
}

