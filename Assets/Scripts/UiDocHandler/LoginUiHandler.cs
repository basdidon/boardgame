using UnityEngine;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;

public sealed class LoginUiHandler : UiDocHandler
{
    public NetworkSpawner NetworkSpawner { get { return NetworkSpawner.Instance; } }

    [BoxGroup("UiValue")]
    public TextField playerNameTextField;
    [BoxGroup("UiValue")]
    public Button confirmBtn;

    private void OnEnable()
    {
        UiDoc = GetComponent<UIDocument>();
       
        playerNameTextField = (TextField) UiDoc.rootVisualElement.Q("PlayerName");
        confirmBtn = (Button) UiDoc.rootVisualElement.Q("LogInBtn");

        if (playerNameTextField == null) Debug.LogError("playerNameTextField not found.");
        if (confirmBtn == null) Debug.LogError("confirmBtn not found");

        Debug.Log(playerNameTextField.text);
        confirmBtn.clicked += () => NetworkSpawner.JoinLobby(playerNameTextField.text);
    }
}
