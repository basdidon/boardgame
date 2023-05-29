using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;


public class LobbyUiHandler : UiDocHandler
{
    ListView roomListView;

    private void OnEnable()
    {
        UiDoc = GetComponent<UIDocument>();

        roomListView = (ListView)UiDoc.rootVisualElement.Q("RoomList");

        if (roomListView == null) Debug.LogError("roomList not found");
        /*
        playerNameTextField = (TextField)UiDoc.rootVisualElement.Q("PlayerName");
        confirmBtn = (Button)UiDoc.rootVisualElement.Q("LogInBtn");
        
        if (playerNameTextField == null) Debug.LogError("playerNameTextField not found.");
        if (confirmBtn == null) Debug.LogError("confirmBtn not found");

        Debug.Log(playerNameTextField.text);

        confirmBtn.clicked += () => LogIn();
        */
    }
}
