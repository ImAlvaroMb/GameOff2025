using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement; 

[CustomEditor(typeof(NodeGenerator))]
public class NodeGeneratorEditor : Editor
{
    private NodeGenerator generator;

    private void OnEnable()
    {
        generator = (NodeGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        // draw the default inspector fields first
        DrawDefaultInspector();

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