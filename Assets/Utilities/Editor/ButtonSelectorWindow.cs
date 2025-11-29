using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSelectorWindow : EditorWindow
{
    private Button[] allButtons;
    private Vector2 scrollPosition;

    // Add a menu item to open the window
    [MenuItem("Tools/UI/Button Selector")]
    public static void ShowWindow()
    {
        // Get existing open window or if none, make a new one:
        GetWindow<ButtonSelectorWindow>("Button Selector");
    }

    private void OnEnable()
    {
        // Automatically find all buttons when the window is opened
        FindAllButtons();
    }

    void OnGUI()
    {
        GUILayout.Label("Button Selection Tool", EditorStyles.boldLabel);

        // --- 1. Find/Refresh Button ---
        if (GUILayout.Button("Find All Buttons in Scene (" + (allButtons != null ? allButtons.Length : 0) + ")"))
        {
            FindAllButtons();
        }

        EditorGUILayout.Space(10);

        // --- 2. Select All/Deselect All Buttons ---
        GUI.enabled = allButtons != null && allButtons.Length > 0;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Select All Found"))
        {
            // Set the Unity Selection to all found GameObjects
            Selection.objects = allButtons.Select(b => b.gameObject).ToArray();
        }
        if (GUILayout.Button("Deselect All"))
        {
            // Clear the Unity Selection
            Selection.objects = new Object[0];
        }
        EditorGUILayout.EndHorizontal();
        GUI.enabled = true;

        EditorGUILayout.Space(10);

        // --- 3. List and Filter ---
        if (allButtons != null && allButtons.Length > 0)
        {
            GUILayout.Label("Found Buttons:", EditorStyles.miniLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (Button button in allButtons)
            {
                // Ensure the object hasn't been destroyed
                if (button == null) continue;

                EditorGUILayout.BeginHorizontal();

                // Object Field: Allows the user to select the Button's GameObject in the Hierarchy
                EditorGUILayout.ObjectField(button.gameObject, typeof(GameObject), true);

                // Toggle Button: Allows manual deselect/select
                bool isSelected = Selection.Contains(button.gameObject);
                if (EditorGUILayout.Toggle(isSelected, GUILayout.Width(20)))
                {
                    // If it was not selected and is now toggled on, add it to the selection
                    if (!isSelected)
                    {
                        Selection.objects = Selection.objects.Append(button.gameObject).ToArray();
                    }
                }
                else
                {
                    // If it was selected and is now toggled off, remove it from the selection
                    if (isSelected)
                    {
                        Selection.objects = Selection.objects.Where(o => o != button.gameObject).ToArray();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
        else if (allButtons != null && allButtons.Length == 0)
        {
            EditorGUILayout.HelpBox("No Button components found in the active scene.", MessageType.Info);
        }
    }

    /// <summary>
    /// Finds all active and inactive Buttons in the current scene.
    /// </summary>
    private void FindAllButtons()
    {
        // FindObjectsOfType is an expensive operation, but suitable for an Editor tool.
        // The 'true' parameter is crucial: it includes inactive GameObjects.
        allButtons = Resources.FindObjectsOfTypeAll<Button>()
            // Filter out assets (buttons inside prefabs/project folder) and only keep scene objects
            .Where(b => b.hideFlags == HideFlags.None && b.gameObject.scene.isLoaded)
            .ToArray();

        // Repaint the window to show the updated count/list
        Repaint();
    }
}
