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
    Label PlayerCountLabel;

    //This function retrieves a reference to the 
    //character name label inside the UI element.
    public void SetVisualElement(VisualElement visualElement)
    {
        NameLabel = visualElement.Q<Label>("RoomName");
        PlayerCountLabel = visualElement.Q<Label>("PlayerCount");
    }

    //This function receives the character whose name this list 
    //element displays. Since the elements listed 
    //in a `ListView` are pooled and reused, it's necessary to 
    //have a `Set` function to change which character's data to display.
    
    public void SetNameLabel(string name,int playerCount,int maxPlayer)
    {
        NameLabel.text = name;
        PlayerCountLabel.text = $"{playerCount} / {maxPlayer}";
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

    List<SessionInfo> sessions;
    List<SessionInfo> Sessions { get { return sessions; } set { sessions = value; roomListView.itemsSource = sessions; } }

    private void OnEnable()
    {
        if(NetworkSpawner.Runner == null)
        {
            Debug.LogError("Runner can't be null");
        }
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
            var session = Sessions[index];
            (item.userData as RoomListEntryController).SetNameLabel(session.Name,session.PlayerCount,session.MaxPlayers); //NetworkSpawner.Instance.sessionInfoList[index].Name
        };

        roomListView.selectionType = SelectionType.Single;
        roomListView.onSelectionChange += _ => NetworkSpawner.JoinGame((roomListView.selectedItem as SessionInfo).Name);

        // let this class update SessionList, when INetworkRunnerCallbacks.OnSessionListUpdated was called
        NetworkSpawner.Instance.sessionListUpdateDelegate += (newList) => Sessions = newList;
    }
}
