using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NodeGraph", menuName = "Scriptable Objects/Dialog System/Node Graph")]
public class NodeGraphSO : ScriptableObject
{ 
    [HideInInspector] public List<NodeSO> nodeList = new List<NodeSO>();
    [HideInInspector] public Dictionary<string, NodeSO> nodeDictionary = new Dictionary<string, NodeSO>();

    private void Awake()
    {
        LoadNodeDictionary();
    }

    /// <summary>
    /// Load the node dictionary from the node list
    /// </summary>
    private void LoadNodeDictionary()
    {
        nodeDictionary.Clear();

        // Populate dictionary
        foreach (NodeSO node in nodeList)
        {
            nodeDictionary[node.id] = node;
        }
    }

    /// <summary>
    /// Get node by nodeID
    /// </summary>
    public NodeSO GetNode(string nodeID)
    {
        if (nodeDictionary.TryGetValue(nodeID, out NodeSO node))
        {
            return node;
        }
        return null;
    }

    /// <summary>
    /// Get child nodes for supplied parent node
    /// </summary>
    public IEnumerable<NodeSO> GetChildNodes(NodeSO parentNode)
    {
        foreach (string childNodeID in parentNode.childNodeIDList)
        {
            yield return GetNode(childNodeID);
        }
    }

    #region Editor Code
    // the following code should only be run in the Unity Editor
#if UNITY_EDITOR

    [HideInInspector] public NodeSO nodeToDrawLineFrom = null;
    [HideInInspector] public Vector2 linePosition;

    // Repopulate node dictionary every time a change is made in the editor
    public void OnValidate()
    {
        LoadNodeDictionary();
    }

    public void SetNodeToDrawConnectionLineFrom(NodeSO node, Vector2 position)
    {
        nodeToDrawLineFrom = node;
        linePosition = position;
    }

#endif

    #endregion Editor Code
}

