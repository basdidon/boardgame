using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class ButtonDocEventHandler : MonoBehaviour
{
    public string buttonName; 
    UIDocument documentButton;
    Button uiBtn;

    public UnityEvent clickEvent;

    private void OnEnable()
    {
        documentButton = GetComponent<UIDocument>();

        if (documentButton == null)
            Debug.LogError("No Button Document");

        uiBtn = documentButton.rootVisualElement.Q(buttonName) as Button;

        if(uiBtn==null)
            Debug.LogError("No Button Found");

        uiBtn.RegisterCallback<ClickEvent>(OnClickButton);
    }

    private void OnClickButton(ClickEvent @event)
    {
        Debug.Log("did you just clicked");
        clickEvent.Invoke();
    }
}
