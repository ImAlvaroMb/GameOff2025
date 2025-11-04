using UnityEngine;
using System.Collections.Generic;

public class NodeGenerator : MonoBehaviour
{
    [Header("Generation Area")]
    [Tooltip("The size of the rectangular area where nodes will be generated.")]
    public Vector2 AreaSize = new Vector2(10f, 10f);

    [Header("Node Settings")]
    [Tooltip("The distance between the center of one node and the next.")]
    public float NodeSpacing = 1.0f;

    [Tooltip("The Prefab of the Node object to instantiate.")]
    public GameObject NodePrefab;

    [Header("Obstacle Settings")]
    [Tooltip("The radius of the check used to avoid obstacles.")]
    public float ObstacleCheckRadius = 0.4f;

    [Tooltip("The layer(s) that represent obstacles (e.g., Walls).")]
    public LayerMask ObstacleLayer;

    // A list to hold all generated nodes for cleanup later
    [HideInInspector]
    public List<GameObject> GeneratedNodes = new List<GameObject>();

    public void GenerateNodes()
    {
        ClearNodes();

        if (NodePrefab == null)
        {
            Debug.LogError("Node Prefab is not assigned on the NodeGenerator.");
            return;
        }

        // calculate the world space starting point (bottom-left corner)
        Vector3 origin = transform.position - new Vector3(AreaSize.x / 2f, AreaSize.y / 2f, 0);

        int nodesCreated = 0;

        // loop through the X and Y axis to calculate the potential position and check if it intersects with an obstacle
        for (float x = 0; x <= AreaSize.x; x += NodeSpacing)
        {
            for (float y = 0; y <= AreaSize.y; y += NodeSpacing)
            {
                Vector3 nodePosition = origin + new Vector3(x, y, 0);
                if (Physics2D.OverlapCircle(nodePosition, ObstacleCheckRadius, ObstacleLayer))
                {
                    continue;
                }

                GameObject newNode = Instantiate(NodePrefab, nodePosition, Quaternion.identity, this.transform);
                newNode.name = $"Node ({nodesCreated})";
                GeneratedNodes.Add(newNode);

                nodesCreated++;
            }
        }
        AStarManager.Instance?.ConnectAllNodes();

        Debug.Log($"Node Generation complete. Created {nodesCreated} nodes.");
    }

    public void ClearNodes()
    {
        foreach (GameObject node in GeneratedNodes)
        {
            if (node != null)
            {
                DestroyImmediate(node);
            }
        }
        GeneratedNodes.Clear();
        Debug.Log("Cleared all previously generated nodes.");
    }
}
