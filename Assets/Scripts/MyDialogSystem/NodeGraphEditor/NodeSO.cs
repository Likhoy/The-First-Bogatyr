using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class NodeSO : ScriptableObject
{
    [HideInInspector] public string id;
    [HideInInspector] public List<string> parentNodeIDList = new List<string>();
    [HideInInspector] public List<string> childNodeIDList = new List<string>();
    [HideInInspector] public NodeGraphSO nodeGraph;

    [HideInInspector] public string nodeText = "¬ведите текст реплики:";



    #region Editor Code
    // the following code should only be run in the Unity Editor
#if UNITY_EDITOR

    [HideInInspector] public Rect rect;
    [HideInInspector] public bool isLeftClickDragging = false;
    [HideInInspector] public bool isSelected = false;

    [HideInInspector] public bool isTextSet = false;

    /// <summary>
    /// Initialise node
    /// </summary>
    public void Initialise(Rect rect, NodeGraphSO nodeGraph)
    {
        this.rect = rect;
        this.id = Guid.NewGuid().ToString();
        this.name = "Node";
        this.nodeGraph = nodeGraph;
        this.nodeText = "¬ведите текст реплики:";
    }


    /// <summary>
    /// Draw node with the node style
    /// </summary>
    public void Draw(GUIStyle nodeStyle)
    {
        // Draw Node Box Using Begin Area
        GUILayout.BeginArea(rect, nodeStyle);

        // EditorGUI.BeginChangeCheck();

        if (!isTextSet)
        {
            nodeText = EditorGUILayout.TextArea(nodeText, GUILayout.Height(100));
        }
        else
        {
            nodeText = nodeText.Trim();
            EditorGUILayout.LabelField(nodeText, GUILayout.Height(100));
        }

        GUILayout.EndArea();
    }

    /// <summary>
    /// Wait for the left ctrl to set the node text
    /// </summary>
    private void ProcessLeftCtrlEvent(Event currentEvent)
    {
        if (currentEvent.keyCode == KeyCode.LeftControl)
            isTextSet = true;
    }

    /// <summary>
    /// Process events for the node
    /// </summary>
    public void ProcessEvents(Event currentEvent)
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

            // Process Mouse Drag Events
            case EventType.MouseDrag:
                ProcessMouseDragEvent(currentEvent);
                break;

            // Process Left Ctrl Events
            case EventType.KeyDown:
                ProcessLeftCtrlEvent(currentEvent);
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Process mouse down events
    /// </summary>
    private void ProcessMouseDownEvent(Event currentEvent)
    {
        // left click down
        if (currentEvent.button == 0)
            ProcessLeftClickDownEvent();
        // right click down
        else if (currentEvent.button == 1)
            ProcessRightClickDownEvent(currentEvent);
    }

    /// <summary>
    /// Process left click down event
    /// </summary>
    private void ProcessLeftClickDownEvent()
    {
        Selection.activeObject = this;

        // Toggle node selection
        isSelected = !isSelected;
    }

    /// <summary>
    /// Process right click down
    /// </summary>
    private void ProcessRightClickDownEvent(Event currentEvent)
    {
        nodeGraph.SetNodeToDrawConnectionLineFrom(this, currentEvent.mousePosition);
    }

    /// <summary>
    /// Process mouse up event
    /// </summary>
    private void ProcessMouseUpEvent(Event currentEvent)
    {
        // left click up
        if (currentEvent.button == 0)
            ProcessLeftClickUpEvent();
    }

    /// <summary>
    /// Process left click up event
    /// </summary>
    private void ProcessLeftClickUpEvent()
    {
        if (isLeftClickDragging)
            isLeftClickDragging = false;
    }

    /// <summary>
    /// Process mouse drag event
    /// </summary>
    private void ProcessMouseDragEvent(Event currentEvent)
    {
        // process left click drag event
        if (currentEvent.button == 0)
            ProcessLeftMouseDragEvent(currentEvent);
    }

    /// <summary>
    /// Process left mouse drag event
    /// </summary>
    private void ProcessLeftMouseDragEvent(Event currentEvent)
    {
        isLeftClickDragging = true;

        DragNode(currentEvent.delta);
        GUI.changed = true;
    }

    /// <summary>
    /// Drag node
    /// </summary>
    public void DragNode(Vector2 delta)
    {
        rect.position += delta;
        EditorUtility.SetDirty(this);
    }

    /// <summary>
    /// Add childID to the node (returns true if the node has been added, false otherwise)
    /// </summary>
    public bool AddChildNodeIDToNode(string childID)
    {
        if (isChildNodeValid(childID))
        {
            childNodeIDList.Add(childID);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check the child node can be validly added to the parent node - return true if it can otherwise return false
    /// </summary>
    public bool isChildNodeValid(string childID)
    {

        // if the node already has a child with this child ID return false
        if (childNodeIDList.Contains(childID))
            return false;

        // if this node ID and the child ID are the same return false
        if (id == childID)
            return false;

        // if this childID is already in the parentID list return false
        if (parentNodeIDList.Contains(childID))
            return false;

        // if the child node already has a parent return false
        // if (nodeGraph.GetRoomNode(childID).parentNodeIDList.Count > 0)
        //      return false;

        return true;
    }

    /// <summary>
    /// Add parentID to the node (returns true if the node has been added, false otherwise)
    /// </summary>
    public bool AddParentNodeIDToNode(string parentID)
    {
        parentNodeIDList.Add(parentID);
        return true;
    }

    /// <summary>
    /// Remove childID from the node (returns true if the node has been removed, false otherwise)
    /// </summary>
    public bool RemoveChildNodeIDFromNode(string childID)
    {
        return childNodeIDList.Remove(childID);     
    }

    /// <summary>
    /// Remove parentID from the node (returns true if the node has been removed, false otherwise)
    /// </summary>
    public bool RemoveParentNodeIDFromNode(string parentID)
    {
        return parentNodeIDList.Remove(parentID); 
    }

#endif
    #endregion Editor Code
}
