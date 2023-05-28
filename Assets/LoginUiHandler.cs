using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;

public class LoginUiHandler : UiDocHandler
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

        confirmBtn.clicked += () => LogIn();
    }

    public async void LogIn()
    {
        var result = await NetworkSpawner.Runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient, // or GameMode.Shared
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = NetworkSpawner.gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok)
        {
            UiDocControls.Instance.ActiveUiDoc(UiMenus.Lobby);
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }
}
