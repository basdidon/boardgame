using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

public class PlayerData : SerializedMonoBehaviour
{
    public static PlayerData Instance { get; set; }

    [OdinSerialize] public string PlayerName { get; set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        if (Instance != null && Instance != this) Destroy(this);
    }
}
