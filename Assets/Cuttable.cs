using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cuttable : MonoBehaviour
{
    public PlantSpawner plant;
    public Node endNode;
    public Node startNode;
    private void OnMouseDown()
    {
        GetComponent<Renderer>().material.color = Random.ColorHSV();
        List<Node> connectedNodes = endNode.GetConnectedNodes();
        foreach (Node node in connectedNodes)
        {
            Debug.Log(node.position);
        }
        plant.DeleteBranch(connectedNodes, startNode);
    }
}
