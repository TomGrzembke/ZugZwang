#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class PlayerPrefsEditor : EditorWindow
{
    private Vector2 scrollPosition;
    private string searchFilter = "";
    private List<PlayerPrefEntry> playerPrefs = new List<PlayerPrefEntry>();
    private Dictionary<string, PlayerPrefEntry> trackedPrefs = new Dictionary<string, PlayerPrefEntry>();
    private string newKey = "";
    private string newStringValue = "";
    private int newIntValue = 0;
    private float newFloatValue = 0f;
    private PlayerPrefType newValueType = PlayerPrefType.String;

    private static readonly string[] commonKeys = new string[]
    {
        "graphicsIndex", "level", "xp", "musicVolume", "soundVolume", "highScore1", "highScore2", "highScore3", "highScore4", "highScore5", "highScore6", "highScore7", "highScore8", "highScore9", "highScore10"
    };

    public enum PlayerPrefType
    {
        String,
        Int,
        Float
    }

    [System.Serializable]
    public class PlayerPrefEntry
    {
        public string key;
        public object value;
        public PlayerPrefType type;
        public bool isEditing = false;
        public string editValue = "";

        public PlayerPrefEntry(string key, object value, PlayerPrefType type)
        {
            this.key = key;
            this.value = value;
            this.type = type;
            this.editValue = value?.ToString() ?? "";
        }
    }

    [MenuItem("Tools/PlayerPrefs Editor")]
    public static void ShowWindow()
    {
        GetWindow<PlayerPrefsEditor>("PlayerPrefs Editor");
    }


    private void OnEnable()
    {
        RefreshPlayerPrefs();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();

        EditorGUILayout.LabelField("PlayerPrefs Editor", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Refresh", GUILayout.Width(80)))
        {
            RefreshPlayerPrefs();
        }

        if (GUILayout.Button("Delete All", GUILayout.Width(80)))
        {
            if (EditorUtility.DisplayDialog("Delete All PlayerPrefs",
                    "Are you sure you want to delete ALL PlayerPrefs? This cannot be undone! (Please don't Calais or Kieran)",
                    "Delete All", "Cancel"))
            {
                PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();
                trackedPrefs.Clear();
                RefreshPlayerPrefs();
            }
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("Search:", GUILayout.Width(50));
        searchFilter = EditorGUILayout.TextField(searchFilter, GUILayout.Width(150));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("Add New PlayerPref", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Key:", GUILayout.Width(50));
        newKey = EditorGUILayout.TextField(newKey);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Type:", GUILayout.Width(50));
        newValueType = (PlayerPrefType)EditorGUILayout.EnumPopup(newValueType, GUILayout.Width(100));
        EditorGUILayout.LabelField("Value:", GUILayout.Width(50));

        switch (newValueType)
        {
            case PlayerPrefType.String:
                newStringValue = EditorGUILayout.TextField(newStringValue);
                break;
            case PlayerPrefType.Int:
                newIntValue = EditorGUILayout.IntField(newIntValue);
                break;
            case PlayerPrefType.Float:
                newFloatValue = EditorGUILayout.FloatField(newFloatValue);
                break;
        }

        if (GUILayout.Button("Add", GUILayout.Width(60)))
        {
            AddNewPlayerPref();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();

        var filteredPrefs = GetFilteredPlayerPrefs();
        EditorGUILayout.LabelField($"PlayerPrefs ({filteredPrefs.Count})", EditorStyles.boldLabel);

        if (filteredPrefs.Count == 0)
        {
            EditorGUILayout.HelpBox("No PlayerPrefs found" +
                                    (string.IsNullOrEmpty(searchFilter) ? "." : " matching the search filter."),
                MessageType.Info);
        }
        else
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < filteredPrefs.Count; i++)
            {
                DrawPlayerPrefEntry(filteredPrefs[i]);
                if (i < filteredPrefs.Count - 1)
                {
                    EditorGUILayout.Space(2);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Note: This editor tracks PlayerPrefs that are created/modified during this session. " +
                                "Some existing PlayerPrefs might not appear until they are accessed or you restart Unity. " +
                                "You can manually check for specific keys by adding them.",
            MessageType.Info);
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Press 'Refresh' at the Start.", MessageType.Info);
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("If you don't use Rider, then don't use my tool. Fuck VSC.", MessageType.Warning);
    }

    private void DrawPlayerPrefEntry(PlayerPrefEntry entry)
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.LabelField("Key:", GUILayout.Width(30));
        EditorGUILayout.SelectableLabel(entry.key, EditorStyles.textField,
            GUILayout.Height(EditorGUIUtility.singleLineHeight));

        EditorGUILayout.LabelField("Type:", GUILayout.Width(35));
        EditorGUILayout.LabelField(entry.type.ToString(), GUILayout.Width(50));

        if (entry.isEditing)
        {
            if (GUILayout.Button("Save", GUILayout.Width(50)))
            {
                SavePlayerPrefEntry(entry);
            }

            if (GUILayout.Button("Cancel", GUILayout.Width(60)))
            {
                entry.isEditing = false;
                entry.editValue = entry.value?.ToString() ?? "";
            }
        }
        else
        {
            if (GUILayout.Button("Edit", GUILayout.Width(50)))
            {
                entry.isEditing = true;
                entry.editValue = entry.value?.ToString() ?? "";
            }
        }

        if (GUILayout.Button("Delete", GUILayout.Width(60)))
        {
            if (EditorUtility.DisplayDialog("Delete PlayerPref",
                    $"Are you sure you want to delete '{entry.key}'?",
                    "Delete", "Cancel"))
            {
                PlayerPrefs.DeleteKey(entry.key);
                PlayerPrefs.Save();
                RefreshPlayerPrefs();
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Value:", GUILayout.Width(40));

        if (entry.isEditing)
        {
            entry.editValue = EditorGUILayout.TextField(entry.editValue);
        }
        else
        {
            EditorGUILayout.SelectableLabel(entry.value?.ToString() ?? "null",
                EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    private void RefreshPlayerPrefs()
    {
        playerPrefs.Clear();

        foreach (var kvp in trackedPrefs)
        {
            playerPrefs.Add(kvp.Value);
        }

        LoadCommonPlayerPrefs();

        playerPrefs.Sort((a, b) => string.Compare(a.key, b.key));
    }

    private void LoadCommonPlayerPrefs()
    {
        foreach (string key in commonKeys)
        {
            if (trackedPrefs.ContainsKey(key)) continue;

            if (PlayerPrefs.HasKey(key))
            {
                try
                {
                    int intValue = PlayerPrefs.GetInt(key, int.MinValue);
                    if (intValue != int.MinValue)
                    {
                        var entry = new PlayerPrefEntry(key, intValue, PlayerPrefType.Int);
                        trackedPrefs[key] = entry;
                        continue;
                    }
                }
                catch
                {
                }

                try
                {
                    float floatValue = PlayerPrefs.GetFloat(key, float.MinValue);
                    if (!float.IsNaN(floatValue) && floatValue != float.MinValue)
                    {
                        var entry = new PlayerPrefEntry(key, floatValue, PlayerPrefType.Float);
                        trackedPrefs[key] = entry;
                        continue;
                    }
                }
                catch
                {
                }

                try
                {
                    string stringValue = PlayerPrefs.GetString(key, "");
                    var entry = new PlayerPrefEntry(key, stringValue, PlayerPrefType.String);
                    trackedPrefs[key] = entry;
                }
                catch
                {
                }
            }
        }
    }

    private List<PlayerPrefEntry> GetFilteredPlayerPrefs()
    {
        if (string.IsNullOrEmpty(searchFilter))
        {
            return playerPrefs;
        }

        return playerPrefs.Where(pref =>
            pref.key.IndexOf(searchFilter, System.StringComparison.OrdinalIgnoreCase) >= 0 ||
            pref.value?.ToString().IndexOf(searchFilter, System.StringComparison.OrdinalIgnoreCase) >= 0
        ).ToList();
    }

    private void AddNewPlayerPref()
    {
        if (string.IsNullOrEmpty(newKey))
        {
            EditorUtility.DisplayDialog("Error", "Key cannot be empty!", "OK");
            return;
        }

        object valueToAdd = null;

        switch (newValueType)
        {
            case PlayerPrefType.String:
                PlayerPrefs.SetString(newKey, newStringValue);
                valueToAdd = newStringValue;
                break;
            case PlayerPrefType.Int:
                PlayerPrefs.SetInt(newKey, newIntValue);
                valueToAdd = newIntValue;
                break;
            case PlayerPrefType.Float:
                PlayerPrefs.SetFloat(newKey, newFloatValue);
                valueToAdd = newFloatValue;
                break;
        }

        PlayerPrefs.Save();

        var entry = new PlayerPrefEntry(newKey, valueToAdd, newValueType);
        trackedPrefs[newKey] = entry;

        RefreshPlayerPrefs();

        newKey = "";
        newStringValue = "";
        newIntValue = 0;
        newFloatValue = 0f;
    }

    private void SavePlayerPrefEntry(PlayerPrefEntry entry)
    {
        try
        {
            switch (entry.type)
            {
                case PlayerPrefType.String:
                    PlayerPrefs.SetString(entry.key, entry.editValue);
                    entry.value = entry.editValue;
                    break;
                case PlayerPrefType.Int:
                    int intValue = int.Parse(entry.editValue);
                    PlayerPrefs.SetInt(entry.key, intValue);
                    entry.value = intValue;
                    break;
                case PlayerPrefType.Float:
                    float floatValue = float.Parse(entry.editValue);
                    PlayerPrefs.SetFloat(entry.key, floatValue);
                    entry.value = floatValue;
                    break;
            }

            PlayerPrefs.Save();
            trackedPrefs[entry.key] = entry;
            entry.isEditing = false;
        }
        catch (System.Exception e)
        {
            EditorUtility.DisplayDialog("Error", $"Could not save value: {e.Message}", "OK");
        }
    }
}
#endif