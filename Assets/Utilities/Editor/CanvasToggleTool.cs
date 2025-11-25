using UnityEngine;
using UnityEditor;
using System.Linq;

public class CanvasToggleTool : EditorWindow
{
    private Canvas[] allCanvases;
    private Vector2 scrollPos;
    private bool showInstructions = true;

    [MenuItem("Tools/UI/Canvas Toggle Tool")]
    public static void ShowWindow()
    {
        GetWindow<CanvasToggleTool>("Canvas Toggler");
    }
    private void OnEnable()
    {
        FindAllCanvases();
        EditorApplication.hierarchyChanged += FindAllCanvases;
    }

    private void OnDisable()
    {
        EditorApplication.hierarchyChanged -= FindAllCanvases;
    }

    private void FindAllCanvases()
    {
        allCanvases = GameObject.FindObjectsOfType<Canvas>(true);
    }

    // --- 2. GUI Layout and Interaction ---

    private void OnGUI()
    {
        // Set up the style for the instruction box
        GUIStyle helpBoxStyle = new GUIStyle(EditorStyles.helpBox);
        helpBoxStyle.padding = new RectOffset(10, 10, 10, 10);
        helpBoxStyle.fontSize = 11;

        // Window Title and Instructions
        EditorGUILayout.LabelField("Scene Canvas Management", EditorStyles.boldLabel);

        showInstructions = EditorGUILayout.BeginFoldoutHeaderGroup(showInstructions, "Instructions");
        if (showInstructions)
        {
            EditorGUILayout.HelpBox(
                "Use this tool to quickly enable or disable all Canvas objects in the current scene. " +
                "This is especially useful for isolating or debugging different UI systems.",
                MessageType.Info);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();

        // Separate the instructions from the main controls
        EditorGUILayout.Space(10);

        // --- 3. Global Control Buttons ---

        GUI.backgroundColor = new Color(0.6f, 1f, 0.6f); // Greenish tint
        if (GUILayout.Button("ENABLE ALL Canvases"))
        {
            SetAllCanvasesState(true);
        }

        GUI.backgroundColor = new Color(1f, 0.6f, 0.6f); // Reddish tint
        if (GUILayout.Button("DISABLE ALL Canvases"))
        {
            SetAllCanvasesState(false);
        }

        // Reset background color for the rest of the GUI
        GUI.backgroundColor = Color.white;

        EditorGUILayout.Space(10);

        // --- 4. Refresh and Count Display ---

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField($"Found Canvases: {allCanvases?.Length ?? 0}", EditorStyles.miniBoldLabel);

        // Button to manually refresh the list if hierarchyChanged didn't catch something
        if (GUILayout.Button("Refresh List", EditorStyles.miniButton, GUILayout.Width(100)))
        {
            FindAllCanvases();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(5);

        // --- 5. Individual Canvas List ---

        // Start a scroll view for the list of canvases
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        if (allCanvases != null && allCanvases.Length > 0)
        {
            foreach (Canvas canvas in allCanvases)
            {
                if (canvas == null) continue; // Skip if the object was deleted

                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                // 1. Object field to easily select and inspect the canvas in the scene
                // (Object field requires the object, not just the component)
                EditorGUILayout.ObjectField(canvas.gameObject, typeof(GameObject), true);

                // 2. Toggle control for the GameObject's active state
                // Use canvas.gameObject.activeSelf to check the current state
                bool currentState = canvas.gameObject.activeSelf;
                bool newState = EditorGUILayout.Toggle(currentState, GUILayout.Width(20));

                if (newState != currentState)
                {
                    canvas.gameObject.SetActive(newState);
                    // Mark the scene as dirty so the change can be saved
                    EditorUtility.SetDirty(canvas.gameObject);
                }

                EditorGUILayout.EndHorizontal();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("No Canvas objects found in the scene.", MessageType.Warning);
        }

        EditorGUILayout.EndScrollView();
    }

    // --- 6. Core Logic Function ---

    private void SetAllCanvasesState(bool active)
    {
        if (allCanvases == null) return;

        // Record all changes for undo/redo functionality
        Undo.RecordObjects(allCanvases.Select(c => c.gameObject).ToArray(), active ? "Enable All Canvases" : "Disable All Canvases");

        foreach (Canvas canvas in allCanvases)
        {
            if (canvas != null && canvas.gameObject.activeSelf != active)
            {
                // Set the active state of the GameObject the Canvas is on
                canvas.gameObject.SetActive(active);
                // Mark as dirty to ensure the change is saved to the scene file
                EditorUtility.SetDirty(canvas.gameObject);
            }
        }
    }
}