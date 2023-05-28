using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginUiHandle : SimulationBehaviour
{
    public TextMeshProUGUI playerNameInput;

    public async void LogIn()
    {
        if (playerNameInput.text.Length < 1)
        {
            Debug.LogError("Input your name.");
            return;
        }

        var result = await NetworkSpawner._runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient, // or GameMode.Shared
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (result.Ok)
        {
            // all good
        }
        else
        {
            Debug.LogError($"Failed to Start: {result.ShutdownReason}");
        }
    }
}
