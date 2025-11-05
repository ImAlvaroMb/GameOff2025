using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public Node CameFrom;
    public List<Node> Connections;
    
    public float gScore;
    public float hScore;

    public bool DrawGizmos = false;

    public float FinalScore() { return gScore + hScore; }

    private void OnDrawGizmos()
    {
        if(DrawGizmos)
        {
            Gizmos.color = Color.blue;
            if (Connections.Count > 0)
            {
                for (int i = 0; i < Connections.Count; i++)
                {
                    Gizmos.DrawLine(transform.position, Connections[i].transform.position);
                }
            }
        }
    }

}
