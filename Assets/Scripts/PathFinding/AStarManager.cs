using System.Collections.Generic;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
using Utilities;

public class AStarManager : AbstractSingleton<AStarManager>
{
    public float NodeConnectionDistance = 1.5f;
    private List<Node> _sceneNodeList = new List<Node>();


    protected override void Start()
    {
        base.Start();
        LoadSceneNodeList();
        //ConnectAllNodes();
    }

    public void OnNewScene()
    {
        LoadSceneNodeList();
    }

    private void LoadSceneNodeList()
    {
        _sceneNodeList.Clear();
        foreach(Node node in FindObjectsByType<Node>(FindObjectsSortMode.None))
        {
            if(!_sceneNodeList.Contains(node)) _sceneNodeList.Add(node);
        }
    }

    public List<Node> GeneratePath(Node start, Node end)
    {
        List<Node> openSet = new List<Node>(); // list of nodes that are discovered bbut havent been examined yet

        foreach (Node n in _sceneNodeList)
        {
            n.gScore = float.MaxValue;// make the node not reacheable
        }

        start.gScore = 0;
        start.hScore = Vector2.Distance(start.transform.position, end.transform.position);
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            int lowestF = default;

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FinalScore() < openSet[lowestF].FinalScore())
                {
                    lowestF = i;
                }
            }

            Node currentNode = openSet[lowestF];
            openSet.Remove(currentNode);

            if (currentNode == end)
            {
                List<Node> path = new List<Node>();

                path.Insert(0, end);

                while (currentNode != start)
                {
                    currentNode = currentNode.CameFrom;
                    path.Add(currentNode);
                }

                path.Reverse();
                return path;
            }

            foreach (Node connectedNode in currentNode.Connections) // we find the faster / most optimal path (less cost)
            {
                float heldGScore = currentNode.gScore + Vector2.Distance(currentNode.transform.position, connectedNode.transform.position);

                if (heldGScore < connectedNode.gScore)
                {
                    connectedNode.CameFrom = currentNode;
                    connectedNode.gScore = heldGScore;
                    connectedNode.hScore = Vector2.Distance(connectedNode.transform.position, end.transform.position);

                    if (!openSet.Contains(connectedNode))
                    {
                        openSet.Add(connectedNode);
                    }
                }
            }
        }

        return null;
    }

    public Node FindNearestNode(Vector2 pos)
    {
        Node foundNode = null;
        float minDistance = float.MaxValue;

        foreach (Node node in _sceneNodeList)
        {
            float currentDistance = Vector2.Distance(pos, node.transform.position);

            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                foundNode = node;
            }
        }

        return foundNode;
    }

    public Node FindFurthestNode(Vector2 pos)
    {
        Node foundNode = null;
        float maxDistance = default;

        foreach (Node node in _sceneNodeList)
        {
            float currentDistance = Vector2.Distance(pos, node.transform.position);
            if (currentDistance > maxDistance)
            {
                maxDistance = currentDistance;
                foundNode = node;
            }
        }

        return foundNode;
    }

    [ContextMenu("Connect nodes")]
    public void ConnectAllNodes()
    {
        LoadSceneNodeList();
        foreach (Node node in _sceneNodeList)
        {
            node.Connections.Clear();
        }

        for (int i = 0; i < _sceneNodeList.Count; i++)
        {
            
            for (int j = i + 1; j < _sceneNodeList.Count; j++)
            {
                if (Vector2.Distance(_sceneNodeList[i].transform.position, _sceneNodeList[j].transform.position) <= NodeConnectionDistance)
                {
                    _sceneNodeList[i].Connections.Add(_sceneNodeList[j]);
                    _sceneNodeList[j].Connections.Add(_sceneNodeList[i]);
                }
            }
        }

#if UNITY_EDITOR
        foreach (Node node in _sceneNodeList)
        {
            EditorUtility.SetDirty(node);
        }
        EditorSceneManager.MarkSceneDirty(gameObject.scene);
#endif
    }



}
