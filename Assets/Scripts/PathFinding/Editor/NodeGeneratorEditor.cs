using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement; 

[CustomEditor(typeof(NodeGenerator))]
public class NodeGeneratorEditor : Editor
{
    private NodeGenerator generator;

    // Use a SerializedProperty to work with the AreaSize and NodeSpacing properties
    private SerializedProperty areaSizeProp;
    private SerializedProperty nodeSpacingProp;
    private SerializedProperty nodePrefabProp;
    private SerializedProperty obstacleCheckRadiusProp;
    private SerializedProperty obstacleLayerProp;

    private void OnEnable()
    {
        generator = (NodeGenerator)target;
        // Find the properties to work with the SerializedObject
        areaSizeProp = serializedObject.FindProperty("AreaSize");
        nodeSpacingProp = serializedObject.FindProperty("NodeSpacing");
        nodePrefabProp = serializedObject.FindProperty("NodePrefab");
        obstacleCheckRadiusProp = serializedObject.FindProperty("ObstacleCheckRadius");
        obstacleLayerProp = serializedObject.FindProperty("ObstacleLayer");
    }

    public override void OnInspectorGUI()
    {
        // Must call this at the start for properties to work correctly
        serializedObject.Update();

        // --- Generation Area Header & AreaSize ---
        EditorGUILayout.LabelField("Generation Area", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(areaSizeProp);

        EditorGUILayout.Space(5);

        // --- Node Settings Header & NodeSpacing Slider ---
        EditorGUILayout.LabelField("Node Settings", EditorStyles.boldLabel);

        // 1. Create a Slider for NodeSpacing, clamping the minimum value to prevent freezing
        // This ensures the spacing is never too small, which would cause an infinite loop in SceneView grid preview.
        float currentSpacing = nodeSpacingProp.floatValue;
        float newSpacing = EditorGUILayout.Slider(
            new GUIContent(nodeSpacingProp.displayName, nodeSpacingProp.tooltip),
            currentSpacing,
            0.1f, // Minimum value for the slider to prevent tiny spacing and freezing
            5.0f  // A reasonable maximum value (adjust as needed)
        );

        // Update the value and apply changes
        if (newSpacing != currentSpacing)
        {
            nodeSpacingProp.floatValue = newSpacing;
        }

        EditorGUILayout.PropertyField(nodePrefabProp);

        EditorGUILayout.Space(5);

        // --- Obstacle Settings Header & Fields ---
        EditorGUILayout.LabelField("Obstacle Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(obstacleCheckRadiusProp);
        EditorGUILayout.PropertyField(obstacleLayerProp);


        // Apply changes to the actual target script
        serializedObject.ApplyModifiedProperties();

        // --- Buttons Section ---
        EditorGUILayout.Space(10);

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("GENERATE NODES"))
        {
            Undo.RecordObject(generator, "Generate Nodes");
            generator.GenerateNodes();
            EditorSceneManager.MarkSceneDirty(generator.gameObject.scene);
        }

        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("CLEAR ALL GENERATED NODES"))
        {
            Undo.RecordObject(generator, "Clear Nodes");
            generator.ClearNodes();
            EditorSceneManager.MarkSceneDirty(generator.gameObject.scene);
        }
        GUI.backgroundColor = Color.white;
    }

    private void OnSceneGUI()
    {
        if (generator == null) return;

        // ensure changes are recorded for undo functionality
        Undo.RecordObject(generator, "Adjust Node Area");

        Vector3 position = generator.transform.position;
        Vector2 halfSize = generator.AreaSize / 2f;

        // draw the Area Box

        Handles.color = new Color(0.2f, 0.8f, 1f, 0.5f); // light blue

        // define the four corners relative to the center position and fill the center
        Vector3 topLeft = position + new Vector3(-halfSize.x, halfSize.y, 0);
        Vector3 topRight = position + new Vector3(halfSize.x, halfSize.y, 0);
        Vector3 bottomLeft = position + new Vector3(-halfSize.x, -halfSize.y, 0);
        Vector3 bottomRight = position + new Vector3(halfSize.x, -halfSize.y, 0);
        Vector3[] verts = new Vector3[] { topLeft, topRight, bottomRight, bottomLeft };
        Handles.DrawSolidRectangleWithOutline(verts, Handles.color, Color.cyan);

        // draw Resizing Handles (Using FreeMoveHandle for Corners)

        float handleSize = HandleUtility.GetHandleSize(position) * 0.1f;
        Vector3 newTopRight = Handles.FreeMoveHandle(topRight, handleSize, Vector3.zero, Handles.SphereHandleCap);
        Vector3 newTopLeft = Handles.FreeMoveHandle(topLeft, handleSize, Vector3.zero, Handles.SphereHandleCap);

        // calculate new size based on the changes to the corners

        // new X Size: Distance between the new right edge and the new left edge
        float newX = Mathf.Abs(newTopRight.x - newTopLeft.x);

        // New Y Size: Distance between the new top edge and the new bottom edge (assuming center is fixed in Y)
        // Since we only dragged the top, we use the distance from the top corner to the center's Y position
        float newY = Mathf.Abs(newTopRight.y - position.y) * 2f;

        // ensure it's not negative
        newX = Mathf.Max(0.1f, newX);
        newY = Mathf.Max(0.1f, newY);

        Vector2 newSize = new Vector2(newX, newY);

        if (newSize != generator.AreaSize)
        {
            generator.AreaSize = newSize;
            EditorUtility.SetDirty(generator);
        }

        // draw Node Spacing Preview (Unchanged)

        Handles.color = new Color(1f, 1f, 1f, 0.2f);
        float spacing = generator.NodeSpacing;

        // draw vertical lines
        for (float x = -halfSize.x; x <= halfSize.x; x += spacing)
        {
            Vector3 start = position + new Vector3(x, -halfSize.y, 0);
            Vector3 end = position + new Vector3(x, halfSize.y, 0);
            Handles.DrawLine(start, end);
        }

        // draw horizontal lines
        for (float y = -halfSize.y; y <= halfSize.y; y += spacing)
        {
            Vector3 start = position + new Vector3(-halfSize.x, y, 0);
            Vector3 end = position + new Vector3(halfSize.x, y, 0);
            Handles.DrawLine(start, end);
        }
    }
}