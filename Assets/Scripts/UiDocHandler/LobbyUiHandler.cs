using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Sirenix.OdinInspector;

public class RoomListEntryController
{
    Label NameLabel;

    //This function retrieves a reference to the 
    //character name label inside the UI element.

    public void SetVisualElement(VisualElement visualElement)
    {
        NameLabel = visualElement.Q<Label>("RoomName");
    }

    //This function receives the character whose name this list 
    //element displays. Since the elements listed 
    //in a `ListView` are pooled and reused, it's necessary to 
    //have a `Set` function to change which character's data to display.
    
    public void SetNameLabel(string name)
    {
        NameLabel.text = name;
    }
}

public class LobbyUiHandler : UiDocHandler
{
    public NetworkSpawner NetworkSpawner { get { return NetworkSpawner.Instance; } }

    TextField roomNameTextField;
    Button createRoomBtn;
    ListView roomListView;

    // UXML template for list entries
    public VisualTreeAsset ListEntryTemplate;

    private void OnEnable()
    {
        UiDoc = GetComponent<UIDocument>();

        roomNameTextField = UiDoc.rootVisualElement.Q<TextField>("RoomName");
        createRoomBtn = UiDoc.rootVisualElement.Q<Button>("CreateRoom");
        roomListView = UiDoc.rootVisualElement.Q<ListView>("RoomList");

        if (roomNameTextField == null) Debug.LogError("RoomName not found");
        if (createRoomBtn == null) Debug.LogError("CreateRoom not found");
        if (roomListView == null) Debug.LogError("roomList not found");

        createRoomBtn.clicked += () => NetworkSpawner.StartHost(roomNameTextField.text);

        roomListView.makeItem = () =>
        {
            // Instantiate the UXML template for the entry
            var newListEntry = ListEntryTemplate.Instantiate();

            // Instantiate a controller for the data
            var newListEntryLogic = new RoomListEntryController();

            // Assign the controller script to the visual element
            newListEntry.userData = newListEntryLogic;

            // Initialize the controller script
            newListEntryLogic.SetVisualElement(newListEntry);

            // Return the root of the instantiated visual tree
            return newListEntry;
        };

        roomListView.bindItem = (item, index) =>
        {
            (item.userData as RoomListEntryController).SetNameLabel(NetworkSpawner.Instance.sessionInfoList[index].Name);
        };

        roomListView.itemsSource = NetworkSpawner.Instance.sessionInfoList;

        // let this class update SessionList, when INetworkRunnerCallbacks.OnSessionListUpdated was called
        NetworkSpawner.Instance.OnSessionListUpdatedAction += () => roomListView.itemsSource = NetworkSpawner.Instance.sessionInfoList;
    }
}
