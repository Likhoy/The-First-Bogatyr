using UnityEngine;
using UnityEditor.Callbacks;
using UnityEditor;
using System.Collections.Generic;

public class NodeGraphEditor : EditorWindow
{
    private GUIStyle nodeStyle;
    private GUIStyle nodeSelectedStyle;
    private static NodeGraphSO currentNodeGraph;

    private Vector2 graphOffset;
    private Vector2 graphDrag;

    private NodeSO currentNode = null;

    // Node layout values
    private const float nodeWidth = 350f;
    private const float nodeHeight = 150f;
    private const int nodePadding = 25;
    private const int nodeBorder = 12;

    // Connecting line values
    private const float connectingLineWidth = 3f;
    private const float connectingLineArrowSize = 6f;

    // Grid Spacing
    private const float gridLarge = 100f;
    private const float gridSmall = 25f;

    [MenuItem("Node Graph Editor", menuItem = "Window/Dialog System/Node Graph Editor")]
    private static void OpenWindow()
    {
        GetWindow<NodeGraphEditor>("Node Graph Editor");
    }

    private void OnEnable()
    {
        // Subscribe to the inspector selection changed event
        Selection.selectionChanged += InspectorSelectionChanged;

        // Define node layout style
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
        nodeStyle.normal.textColor = Color.white;
        nodeStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        nodeStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

        // Define selected node style
        nodeSelectedStyle = new GUIStyle();
        nodeSelectedStyle.normal.background = EditorGUIUtility.Load("node1 on") as Texture2D;
        nodeSelectedStyle.normal.textColor = Color.white;
        nodeSelectedStyle.padding = new RectOffset(nodePadding, nodePadding, nodePadding, nodePadding);
        nodeSelectedStyle.border = new RectOffset(nodeBorder, nodeBorder, nodeBorder, nodeBorder);

    }

    private void OnDisable()
    {
        // Unsubscribe from the inspector selection changed event
        Selection.selectionChanged -= InspectorSelectionChanged;
    }

    /// <summary>
    /// Open the node graph editor window if a node graph scriptable object asset is double clicked in the inspector
    /// </summary>
    [OnOpenAsset(0)] // Need the namespace UnityEditor.Callbacks
    public static bool OnDoubleClickAsset(int instanceID, int line)
    {
        NodeGraphSO nodeGraph = EditorUtility.InstanceIDToObject(instanceID) as NodeGraphSO;

        if (nodeGraph != null)
        {
            OpenWindow();

            currentNodeGraph = nodeGraph;

            return true;
        }
        return false;
    }


    private void OnGUI()
    {
        // if a scriptable object of type NodeGraphSO has been selected then process
        if (currentNodeGraph != null)
        {
            // Draw Grid
            DrawBackgroundGrid(gridSmall, 0.2f, Color.grey);
            DrawBackgroundGrid(gridLarge, 0.3f, Color.grey);

            // Draw line if being dragged
            DrawDraggedLine();

            // Process Events
            ProcessEvents(Event.current);

            // Draw Connections Between Nodes
            DrawConnections();

            // Draw Nodes
            DrawNodes();
        }

        if (GUI.changed)
            Repaint();
    }

    /// <summary>
    /// Draw background grid for the node graph editor
    /// </summary>
    private void DrawBackgroundGrid(float gridSize, float gridOpacity, Color gridColor)
    {
        int verticalLineCount = Mathf.CeilToInt((position.width + gridSize) / gridSize);
        int horizontalLineCount = Mathf.CeilToInt((position.height + gridSize) / gridSize);

        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        graphOffset += graphDrag * 0.5f;

        Vector3 gridOffset = new Vector3(graphOffset.x % gridSize, graphOffset.y % gridSize, 0);

        // drawing vertical lines
        for (int i = 0; i < verticalLineCount; i++)
        {
            Handles.DrawLine(new Vector3(gridSize * i, -gridSize, 0) + gridOffset, new Vector3(gridSize * i, position.height + gridSize, 0f) + gridOffset);
        }

        // drawing horizontal lines
        for (int j = 0; j < horizontalLineCount; j++)
        {
            Handles.DrawLine(new Vector3(-gridSize, gridSize * j, 0) + gridOffset, new Vector3(position.width + gridSize, gridSize * j, 0f) + gridOffset);
        }

        // returning to the default color
        Handles.color = Color.white;
    }

    private void DrawDraggedLine()
    {
        if (currentNodeGraph.linePosition != Vector2.zero)
        {
            // Draw line from node to line position
            Handles.DrawBezier(currentNodeGraph.nodeToDrawLineFrom.rect.center, currentNodeGraph.linePosition,
                currentNodeGraph.nodeToDrawLineFrom.rect.center, currentNodeGraph.linePosition, Color.white, null, connectingLineWidth);
        }
    }

    private void ProcessEvents(Event currentEvent)
    {
        // Reset graph drag
        graphDrag = Vector2.zero;

        // Get node that mouse is over if it's null or not currently being dragged
        if (currentNode == null || currentNode.isLeftClickDragging == false)
            currentNode = IsMouseOverNode(currentEvent);

        // if mouse isn't over a node
        if (currentNode == null || currentNodeGraph.nodeToDrawLineFrom != null)
            ProcessNodeGraphEvents(currentEvent);
        // else process node events
        else
            currentNode.ProcessEvents(currentEvent);
    }

    /// <summary>
    /// Check to see if the mouse is over a node - if so then return the node else return null
    /// </summary>
    private NodeSO IsMouseOverNode(Event currentEvent)
    {
        for (int i = currentNodeGraph.nodeList.Count - 1; i >= 0; i--)
        {
            if (currentNodeGraph.nodeList[i].rect.Contains(currentEvent.mousePosition))
                return currentNodeGraph.nodeList[i];
        }

        return null;
    }
    /// <summary>
    /// Process Node Graph Events
    /// </summary>
    private void ProcessNodeGraphEvents(Event currentEvent)
    {
        switch (currentEvent.type)
        {
            // Process Mouse Down Events
            case EventType.MouseDown:
                ProcessMouseDownEvent(currentEvent);
                break;

            // Process Mouse Up Events
            case EventType.MouseUp:
                ProcessMouseUpEvent(currentEvent);
                break;

            // Process Mouse Drag Event
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Process mouse down events on the node graph (not over a node)
    /// </summary>
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // Process right click mouse down on graph event (show context menu)
        if (currentEvent.button == 1)
            ShowContextMenu(currentEvent.mousePosition);
        // Process left mouse down on graph event
        else if (currentEvent.button == 0)
        {
            ClearLineDrag();
            ClearAllSelectedNodes();
        }
    }

    /// <summary>
    /// Show the context menu
    /// </summary>
    private void ShowContextMenu(Vector2 mousePosition)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Create Node"), false, CreateNode, mousePosition);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Select All Nodes"), false, SelectAllNodes);
        menu.AddSeparator("");
        menu.AddItem(new GUIContent("Delete Selected Links"), false, DeleteSelectedNodeLinks);
        menu.AddItem(new GUIContent("Delete Selected Nodes"), false, DeleteSelectedNodes);

        menu.ShowAsContext();
    }

    /// <summary>
    /// Create a node at the mouse position
    /// </summary>
    private void CreateNode(object mousePositionObject)
    {
        Vector2 mousePosition = (Vector2)mousePositionObject;

        // create node scriptable object asset
        NodeSO node = ScriptableObject.CreateInstance<NodeSO>();

        // add node to current node graph node list
        currentNodeGraph.nodeList.Add(node);

        // set node values
        node.Initialise(new Rect(mousePosition, new Vector2(nodeWidth, nodeHeight)), currentNodeGraph);

        // add node to node graph scriptable object asset database
        AssetDatabase.AddObjectToAsset(node, currentNodeGraph);

        AssetDatabase.SaveAssets();

        // Refresh graph node dictionary
        currentNodeGraph.OnValidate();
    }

    /// <summary>
    /// Delete selected nodes
    /// </summary>
    private void DeleteSelectedNodes()
    {
        Queue<NodeSO> nodeDeletionQueue = new Queue<NodeSO>();

        // loop through all nodes 
        foreach (NodeSO node in currentNodeGraph.nodeList)
        {
            if (node.isSelected)
            {
                nodeDeletionQueue.Enqueue(node);

                // iterate through child node ids
                foreach (string childNodeID in node.childNodeIDList)
                {
                    // Retrieve child room node 
                    NodeSO childNode = currentNodeGraph.GetNode(childNodeID);

                    if (childNode != null)
                    {
                        // Remove parentID from child node
                        childNode.RemoveParentNodeIDFromNode(node.id);
                    }
                }

                // iterate through parent node ids
                foreach (string parentNodeID in node.parentNodeIDList)
                {
                    // Retrieve parent room node 
                    NodeSO parentNode = currentNodeGraph.GetNode(parentNodeID);

                    if (parentNode != null)
                    {
                        // Remove childID from parent room node
                        parentNode.RemoveChildNodeIDFromNode(node.id);
                    }
                }
            }
        }

        // Delete queued nodes
        while (nodeDeletionQueue.Count > 0)
        {
            // Get room node from queue
            NodeSO nodeToDelete = nodeDeletionQueue.Dequeue();

            // Remove node from dictionary
            currentNodeGraph.nodeDictionary.Remove(nodeToDelete.id);

            // Remove node from list
            currentNodeGraph.nodeList.Remove(nodeToDelete);

            // Remove node from Asset databese
            DestroyImmediate(nodeToDelete, true);

            // Save asset database
            AssetDatabase.SaveAssets();
        }
    }

    /// <summary>
    /// Delete the links between the selected nodes 
    /// </summary>
    private void DeleteSelectedNodeLinks()
    {
        // Iterate through all nodes
        foreach (NodeSO node in currentNodeGraph.nodeList)
        {
            if (node.isSelected && node.childNodeIDList.Count > 0)
            {
                for (int i = node.childNodeIDList.Count - 1; i >= 0; i--)
                {
                    // Get child node
                    NodeSO childNode = currentNodeGraph.GetNode(node.childNodeIDList[i]);

                    // if the child room node is selected 
                    if (childNode != null && childNode.isSelected)
                    {
                        // Remove childID from parent node
                        node.RemoveChildNodeIDFromNode(childNode.id);

                        // Remove parentID from child node
                        childNode.RemoveParentNodeIDFromNode(node.id);
                    }
                }
            }
        }

        // Clear all selected nodes
        ClearAllSelectedNodes();
    }

    /// <summary>
    /// Clear selection from all nodes
    /// </summary>
    private void ClearAllSelectedNodes()
    {
        foreach (NodeSO roomNode in currentNodeGraph.nodeList)
        {
            if (roomNode.isSelected)
            {
                roomNode.isSelected = false;

                GUI.changed = true;
            }
        }
    }

    /// <summary>
    /// Select all nodes
    /// </summary>
    private void SelectAllNodes()
    {
        foreach (NodeSO node in currentNodeGraph.nodeList)
        {
            node.isSelected = true;
        }
        GUI.changed = true;
    }

    /// <summary>
    /// Process mouse up events
    /// </summary>
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        // if releasing the right mouse button and currently dragging a line
        if (currentEvent.button == 1 && currentNodeGraph.nodeToDrawLineFrom != null)
        {
            // Check if over a node
            NodeSO node = IsMouseOverNode(currentEvent);

            if (node != null)
            {
                // if so set it as a child of the parent node if it can be added
                if (currentNodeGraph.nodeToDrawLineFrom.AddChildNodeIDToNode(node.id))
                {
                    // Set parent ID in child node
                    node.AddParentNodeIDToNode(currentNodeGraph.nodeToDrawLineFrom.id);
                }
            }

            ClearLineDrag();
        }

    }

    /// <summary>
    /// Process mouse drag event
    /// </summary>
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        // process right click drag event - draw line
        if (currentEvent.button == 1)
            ProcessRightMouseDragEvent(currentEvent);
        // process left click drag event - drag node graph
        else if (currentEvent.button == 0)
        {
            ProcessLeftMouseDragEvent(currentEvent.delta);
        }
    }

    /// <summary>
    /// Process right mouse drag event - draw line
    /// </summary>
    private void ProcessRightMouseDragEvent(Event currentEvent)
    {
        if (currentNodeGraph.nodeToDrawLineFrom != null)
        {
            DragConnectingLine(currentEvent.delta);
            GUI.changed = true;
        }
    }

    /// <summary>
    /// Process left mouse drag event - drag node graph
    /// </summary>
    private void ProcessLeftMouseDragEvent(Vector2 dragDelta)
    {
        graphDrag = dragDelta;

        for (int i = 0; i < currentNodeGraph.nodeList.Count; i++)
        {
            currentNodeGraph.nodeList[i].DragNode(dragDelta);
        }

        GUI.changed = true;
    }

    /// <summary>
    /// Drag connecting line from node
    /// </summary>
    public void DragConnectingLine(Vector2 delta)
    {
        currentNodeGraph.linePosition += delta;
    }

    /// <summary>
    /// Clear line drag from a node
    /// </summary>
    private void ClearLineDrag()
    {
        currentNodeGraph.nodeToDrawLineFrom = null;
        currentNodeGraph.linePosition = Vector2.zero;
        GUI.changed = true;
    }

    /// <summary>
    /// Draw connection in the graph window between nodes
    /// </summary>
    private void DrawConnections()
    {
        // loop through all nodes
        foreach (NodeSO node in currentNodeGraph.nodeList)
        {
            if (node.childNodeIDList.Count > 0)
            {
                // loop through child nodes
                foreach (string childNodeID in node.childNodeIDList)
                {
                    // get child node from dictionary
                    if (currentNodeGraph.nodeDictionary.ContainsKey(childNodeID))
                    {
                        DrawConnectionLine(node, currentNodeGraph.nodeDictionary[childNodeID]);

                        GUI.changed = true;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draw connection line between the parent node and child node
    /// </summary>
    private void DrawConnectionLine(NodeSO parentNode, NodeSO childNode)
    {
        // get line start and end position
        Vector2 startPosition = parentNode.rect.center;
        Vector2 endPosition = childNode.rect.center;

        // calculate midway point
        Vector2 midPosition = (endPosition + startPosition) / 2f;

        // Vector from start to end position of line
        Vector2 direction = endPosition - startPosition;

        // Calculate normalized perpendicular positions from the mid point
        Vector2 arrowTailPoint1 = midPosition - new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;
        Vector2 arrowTailPoint2 = midPosition + new Vector2(-direction.y, direction.x).normalized * connectingLineArrowSize;

        // Calculate mid point offset position for arrow head
        Vector2 arrowHeadPoint = midPosition + direction.normalized * connectingLineArrowSize;

        // Draw Arrow
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint1, arrowHeadPoint, arrowTailPoint1, Color.white, null, connectingLineWidth);
        Handles.DrawBezier(arrowHeadPoint, arrowTailPoint2, arrowHeadPoint, arrowTailPoint2, Color.white, null, connectingLineWidth);

        // Draw line
        Handles.DrawBezier(startPosition, endPosition, startPosition, endPosition, Color.white, null, connectingLineWidth);

        GUI.changed = true;
    }

    /// <summary>
    /// Draw nodes in the graph window
    /// </summary>
    private void DrawNodes()
    {
        // loop through all nodes and draw them
        foreach (NodeSO node in currentNodeGraph.nodeList)
        {
            if (node.isSelected)
                node.Draw(nodeSelectedStyle);
            else
                node.Draw(nodeStyle);
        }

        GUI.changed = true;
    }

    /// <summary>
    /// Selection changed in the inspector
    /// </summary>
    private void InspectorSelectionChanged()
    {
        NodeGraphSO nodeGraph = Selection.activeObject as NodeGraphSO;

        if (nodeGraph != null)
        {
            currentNodeGraph = nodeGraph;
            GUI.changed = true;
        }
    }
}

