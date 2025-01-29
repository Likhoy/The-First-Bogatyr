using GIGA.AutoRadialLayout.QuerySystem;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace GIGA.AutoRadialLayout
{
    [ExecuteInEditMode]
    public class RadialLayout : MonoBehaviour
    {
#if UNITY_EDITOR
        public static GameObject forceSelectionSkippedObject = null; // used if Forced selection is active, to allow object selection if clicked a second time
        public static GameObject forceSelectionTargetedObject = null; // used if Forced selection is active, to allow object selection if clicked a second time
#endif

        // Constants
        public const string VERSION = "1.0.1";

        // Enums
        public enum AutoRebuildMode { Never, EditorOnly, Always };
        public enum NodesDistribution { Concentric, Branches };
        public enum RelativeRotation { RelativeToParent, KeepOwn }
        public enum LinksRebuildMode {Regenerate,KeepExisting };

        // Auto-rebuild
        public AutoRebuildMode autoRebuildMode = AutoRebuildMode.EditorOnly;

        // Layout settings
        public float circleRadius = 100;
        public float circleSlice = 360;
        public float rotationOffset = 0;
        public RelativeRotation relativeRotationMode = RelativeRotation.RelativeToParent;
        public float[] externalCircleRadiusMultiplier = new float[] { 1.0f };
        public bool enableSeparateExternalRadii;                                // If true, different multipliers can be applied to each depth
        public bool scaleNodeWithRadius = false;
        public float nodeScaleFactor = 100;
        public float nodeScale_min = 0.5f, nodeScale_max = 2f;
        public NodesDistribution nodesDistribution;
        public float fanDistributionCommonSpan = 60;                            // Shared span when using "branches" distribution
        public float branchesCommonLength = 1;                                  // Shared distance when using "branches" distribution (multiplier of the main circle radius)
        public bool showInnerNode = false;
        public float innerNodeScale = 1.0f;
        public bool showInnerLinks = false;
        public LinksRebuildMode linksRebuildMode = LinksRebuildMode.Regenerate;
        public RadialLayoutLink.ProgressMode linksProgressMode;
        public uint linksProgressSpeed = 40;
#if UNITY_EDITOR
        public bool forceNodesSelection = false;                                // Forces selection of node objects in editor
#endif
        // Getters
        public int MaxDepth { get; private set; }
        public bool IsSubLayout { get { return this.GetComponentInParent<RadialLayoutNode>() != null; } }
        public RadialLayout ParentLayout { get; private set; }
        public int NodeCount { get {
                int count = this.Nodes.Count;
                foreach (var subLayout in this.subLayouts)
                    count += subLayout.NodeCount;
                return count;
            } }
        public int SubLayoutsCount { get { return this.GetSubLayouts(true).Length; } }

        // References
        public GameObject innerNode;
        private Canvas _canvas;
        public Canvas Canvas{get{
                if (_canvas == null)
                    _canvas = GetComponentInParent<Canvas>();
                return _canvas;
            } }

        // Prefabs
        public RadialLayoutNode prefab_node;
        public RadialLayoutLink prefab_link;

        // Query System
        [SerializeField]
        private bool _useQuerySystem;
        public bool UseQuerySystem {
            get
            {
                if (!this.IsSubLayout)
                    return this._useQuerySystem;
                else
                {
                    var masterLayout = this.GetMasterLayout();
                    if (masterLayout != this)
                        return masterLayout.UseQuerySystem;
                    else
                    {
                        Debug.LogWarning($"Found a sub-layout that is master of itself. ({this.name})");
                        return this._useQuerySystem;
                    }
                }
            }
            set {
                this.SetupQuerySystem(value);
            } }

        // Editor Flags
#if UNITY_EDITOR
        public bool drawEditorGizmos = true;
        public bool drawEditorGizmos_circleRadius = true;
        public bool drawEditorGizmos_nodes = true;
        public bool drawEditorGizmos_handles = true;
        public bool handle_lockRadius = false;
        public bool handle_lockRotation = false;
        public bool handle_snap = false;
        public bool handle_snap_intValues = false;
        public float handle_snap_resolution = 10;
        public bool keepPrefabLinking = false;
#endif

        public List<RadialLayoutNode> Nodes { get; private set; }
        private List<RadialLayout> subLayouts;
        private int lastChildCount = 0;

        // Cached values for auto-rebuild
        private float lastRotationOffset;
        private float lastCircleRadius;
        private float lastCircleSlice;
        private float lastNodeScaleFactor;
        private float lastNodeScale_min, lastNodeScale_max;
        private bool lastScaleNodeWithRadius;
        private bool lastEnableSeparateExternalRadii;
        private float[] lastExternalCircleRadiusMultiplier;
        private NodesDistribution lastNodesDistribution;
        private float lastFanDistributionCommonSpan;
        private float lastsBranchesCommonLenght;
        private bool lastShowInnerNode;
        private float lastInnerNodeScale;
        private bool lastShowInnerLinks;
        private RelativeRotation lastRelativeRotation;

        public GameObject linksRoot;

        // Internal data
        private List<RadialLayoutNode> linksRebuildList;
        private byte linksRebuildFrameCounter;


        // Events
        public Action<RadialLayout> onRebuild;

        // Flags
        private bool initialized;

		private void OnEnable()
		{
            this.ResetUpdateParameters();
#if UNITY_EDITOR
            Undo.undoRedoPerformed += OnUndoPerformed;
#endif
        }

		private void OnDisable()
		{
#if UNITY_EDITOR
            Undo.undoRedoPerformed -= OnUndoPerformed;
#endif
        }


		void Update()
        {
            if (!initialized)
            {
                this.Initialize();
            }
            else if (this.autoRebuildMode == AutoRebuildMode.Always || !Application.isPlaying && this.autoRebuildMode == AutoRebuildMode.EditorOnly || this.linksRebuildList != null)
            {
                bool rebuild = false;
                if (this.transform.childCount != this.lastChildCount ||
                    this.circleRadius != this.lastCircleRadius ||
					this.circleSlice != this.lastCircleSlice ||
					this.rotationOffset != this.lastRotationOffset ||
                    this.nodeScaleFactor != this.lastNodeScaleFactor ||
                    this.scaleNodeWithRadius != this.lastScaleNodeWithRadius ||
                    this.lastNodeScale_min != this.nodeScale_min ||
                    this.lastNodeScale_max != this.nodeScale_max ||
                    this.enableSeparateExternalRadii != this.lastEnableSeparateExternalRadii ||
                    this.nodesDistribution != this.lastNodesDistribution ||
                    this.fanDistributionCommonSpan != this.lastFanDistributionCommonSpan ||
                    this.branchesCommonLength != this.lastsBranchesCommonLenght ||
                    this.showInnerNode != this.lastShowInnerNode ||
                    this.innerNodeScale != this.lastInnerNodeScale ||
                    this.showInnerLinks != this.lastShowInnerLinks ||
                    this.relativeRotationMode != this.lastRelativeRotation)
                {
                    rebuild = true;
                    this.ResetUpdateParameters();
                }

                if (this.lastExternalCircleRadiusMultiplier == null || this.lastExternalCircleRadiusMultiplier.Length != this.externalCircleRadiusMultiplier.Length)
                {
                    this.lastExternalCircleRadiusMultiplier = new float[this.externalCircleRadiusMultiplier.Length];
                }
                for (int k = 0; k < this.externalCircleRadiusMultiplier.Length; k++)
                    if (this.lastExternalCircleRadiusMultiplier[k] != this.externalCircleRadiusMultiplier[k])
                    {
                        this.lastExternalCircleRadiusMultiplier[k] = this.externalCircleRadiusMultiplier[k];
                        rebuild = true;
                    }

                if(rebuild)
                    this.Rebuild();

                // Links rebuild
                this.RebuildLinks(initialized);

                initialized = true;

            }
            else if (Application.isPlaying && this.autoRebuildMode != AutoRebuildMode.Always)
                this.enabled = false;
        }



		#region Layout

		private void Initialize()
		{
            this.Rebuild();
            this.RebuildLinks(false);
            this.SetupQuerySystem(this._useQuerySystem);
            initialized = true;
        }

        /// <summary>
        /// Rebuilds the entire Node and Link structure
        /// </summary>
        public void Rebuild()
        {
            // Destroying links
            for (int k = this.linksRoot.transform.childCount - 1; k >= 0; k--)
            {
                if (this.GetMasterLayout().linksRebuildMode == LinksRebuildMode.Regenerate)
                {
                    // Destroying all links
                    DestroyGameObject(this.linksRoot.transform.GetChild(k).gameObject);
                }
                else
                {
                    // Destroying orphans
                    RadialLayoutLink link = this.linksRoot.transform.GetChild(k).GetComponent<RadialLayoutLink>();
                    if(link != null && link.to == null || link.from == null && link.fromLayout == null)
						DestroyGameObject(this.linksRoot.transform.GetChild(k).gameObject);

                    // Destroying inner links if option is turned off
                    if(link.from == null && link.fromLayout == this && !this.showInnerLinks)
						DestroyGameObject(this.linksRoot.transform.GetChild(k).gameObject);
				}
			}

			//Deleting orphans links root
            foreach (var linksRoot in this.transform.GetComponentsInChildren<RadialLayoutLinksRoot>())
			{
				if (linksRoot != this.linksRoot && linksRoot.parentLayout == null)
					DestroyGameObject(linksRoot.gameObject);
			}

			// Resetting data
			this.Nodes = new List<RadialLayoutNode>();
            this.subLayouts = new List<RadialLayout>();
            this.MaxDepth = 0;

            Dictionary<Transform, int> sortingDictionary = new Dictionary<Transform, int>();
            int subLayoutsCount = 0;

            foreach (Transform t in this.transform)
            {
                if (!t.gameObject.activeSelf)
                    continue;

                // Skipping inner node
                if (t.gameObject == this.innerNode)
                {
                    sortingDictionary.Add(t, -1);
                    continue;
                }

                if (t.gameObject == this.linksRoot)
                    sortingDictionary.Add(t, 0);

                if (t.GetComponent<LayoutElement>() == null || !t.GetComponent<LayoutElement>().ignoreLayout)
                {
                    if (t.GetComponent<RadialLayoutNode>() == null)
                        t.gameObject.AddComponent<RadialLayoutNode>();
                    RadialLayoutNode node = t.gameObject.GetComponent<RadialLayoutNode>();
                    this.Nodes.Add(node);
                    node.SetParent(this);
                    node.depth = 0;
                    int maxDepth = node.ScanForSubNodes();

                    // Creating inner links
                    if (this.showInnerLinks && node.depth == 0)
                        this.QueueNodeForLinkRebuild(node);

                    // Searching for sub-layouts
                    if (node.depth == 0)
                    {
                        var foundSubLayouts = node.ScanForSubLayouts(null);
                        if (foundSubLayouts.Length > 0)
                        {
                            bool isSubLayout = this.IsSubLayout;

                            foreach (var subLayout in foundSubLayouts)
                            {
                                if (!this.subLayouts.Contains(subLayout))
                                {
                                    subLayoutsCount++;
                                    this.subLayouts.Add(subLayout);
                                }
                                // Master layout is responsible for rebuilding and sorting links roots
                                if (!isSubLayout)
                                {
                                    if (!sortingDictionary.ContainsKey(subLayout.linksRoot.transform))
                                        sortingDictionary.Add(subLayout.linksRoot.transform, subLayoutsCount);
                                    else
                                    {
                                        // Found shared link root between 2 sub-layouts, creating a new links root
                                        Debug.LogWarning("Found shared links root between sub-layouts, fixing it...");
                                        GameObject linksRoot = new GameObject("LinksRoot(Sub-Layout)", typeof(LayoutElement), typeof(RadialLayoutLinksRoot));
                                        linksRoot.GetComponent<LayoutElement>().ignoreLayout = true;
                                        linksRoot.GetComponent<RadialLayoutLinksRoot>().parentLayout = subLayout;
                                        linksRoot.transform.SetParent(this.transform);
                                        subLayout.linksRoot = linksRoot;
                                        sortingDictionary.Add(subLayout.linksRoot.transform, subLayoutsCount);
                                    }
                                }
                                if (!subLayout.initialized)
                                    subLayout.Initialize();
                                else
                                    subLayout.Rebuild();
                            }
                        }
                    }

                    if (maxDepth > this.MaxDepth)
                        this.MaxDepth = maxDepth;
                }
                else if (t.GetComponent<RadialLayoutNode>() != null)
                {
                    if(!Application.isPlaying)
                        GameObject.DestroyImmediate(t.GetComponent<RadialLayoutNode>());
                    else
                        GameObject.Destroy(t.GetComponent<RadialLayoutNode>());
                }
            }

            if (!this.IsSubLayout)
            {
                var allSubLayouts = this.GetSubLayouts(true);
                foreach (var sublayout in allSubLayouts)
                    if (!sortingDictionary.ContainsKey(sublayout.linksRoot.transform))
                        sortingDictionary.Add(sublayout.linksRoot.transform, subLayoutsCount++);
            }

            // Resorting links roots and inner nodes
            foreach (var obj in sortingDictionary)
            {
                if (obj.Value != -1)
                {
                    if (!this.IsSubLayout && obj.Key.gameObject.GetComponent<RadialLayoutLinksRoot>() != null)
                    {
                        obj.Key.SetParent(this.transform);
                        obj.Key.transform.position = obj.Key.gameObject.GetComponent<RadialLayoutLinksRoot>().parentLayout.transform.position;
                        obj.Key.transform.localScale = Vector3.one;
                    }
                    obj.Key.SetSiblingIndex(obj.Value);
                }
            }
            foreach (var obj in sortingDictionary)
            {
                if (obj.Value == -1)
                {
                    if (obj.Key.gameObject == this.innerNode.gameObject)
                    {
                        if(!this.IsSubLayout)
                            obj.Key.SetSiblingIndex(subLayoutsCount + 1);
                        else
							obj.Key.SetSiblingIndex(0);
					}
				}
            }

            // Placing on circle
            foreach (var node in this.Nodes)
            {
                node.PlaceOnCircle();

                // Scaling
                if (!this.IsSubLayout)
                {
                    if (this.scaleNodeWithRadius && this.circleRadius >= 0)
                    {
                        if (node.depth == 0)
                        {
                            float scaleFactor = this.circleRadius / this.nodeScaleFactor;
                            node.transform.localScale = Vector3.one * Mathf.Clamp(scaleFactor, this.nodeScale_min, this.nodeScale_max);
                        }
                        else
                        {
                            // Subnodes will have the scale of the parent node
                            node.transform.localScale = Vector3.one;
                        }
                    }
                    else
                        node.transform.localScale = Vector3.one;
                }
                else
                {
                    if (node.depth == 0)
                    {
                        float scaleFactor = 1;
                        if (this.scaleNodeWithRadius)
                        {
                            // Overriding master scale
                            scaleFactor = this.circleRadius / this.nodeScaleFactor;
                        }
                        else
                        {
                            // Using master scale
                            node.transform.localScale = Vector3.one;
                        }

                        node.transform.localScale = Vector3.one * Mathf.Clamp(scaleFactor, this.nodeScale_min, this.nodeScale_max);
                    }
                    else
                    {
                        // Subnodes will have the scale of the parent node
                        node.transform.localScale = Vector3.one;
                    }
                }

            }

            // Applying customization
            this.ShowInnerNode(this.showInnerNode);

#if UNITY_EDITOR
            // Links rebuild (called here to mitigate editor frame throttling effect
            this.RebuildLinks(initialized);
#endif

            this.onRebuild?.Invoke(this);

        }

        private void RebuildLinks(bool deferred)
        {
#if UNITY_EDITOR
#if UNITY_2021_3_OR_NEWER
            // Links are not created in prefab staging scene
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
                return;
#else
            // Links are not created in prefab staging scene
            if (UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null)
                return;
#endif
#endif

            if (this.linksRebuildList != null)
            {
                if (this.linksRebuildFrameCounter == 0 || !deferred)
                {
                    foreach (var node in this.linksRebuildList)
                        node.CreateLink();

                    this.linksRebuildList = null;
                    this.linksRebuildFrameCounter = 0;
                }
                else
                {
                    this.linksRebuildFrameCounter--;
                }
            }
        }

        /// <summary>
        /// Gets the circle radius multiplier (multiplier of base Circle Radius) for the specified depth.
        /// </summary>
        /// <param name="depth">Desired depth.</param>
        /// <returns></returns>
        public float GetNodesRadiusMultiplier(int depth)
        {
            if (depth == 0 || !this.enableSeparateExternalRadii)
                return 1;
            else
            {
                if (depth - 1 < this.externalCircleRadiusMultiplier.Length)
                    return this.externalCircleRadiusMultiplier[depth - 1];
                else
                {
                    // Auto adjusting array size
                    if (depth - 1 == this.externalCircleRadiusMultiplier.Length)
                    {
                        List<float> temp = new List<float>(this.externalCircleRadiusMultiplier);
                        temp.Add(1);
                        this.externalCircleRadiusMultiplier = temp.ToArray();
                    }

                    return 1;
                }
            }
        }

        /// <summary>
        /// Sets the parent layout for sub-layouts.
        /// </summary>
        public void SetParentLayout(RadialLayout layout)
        {
            this.ParentLayout = layout;
        }

        /// <summary>
        /// Queue the node for link rebuild in the next frame.
        /// </summary>
        public void QueueNodeForLinkRebuild(RadialLayoutNode node)
        {
            if (this.linksRebuildList == null)
                this.linksRebuildList = new List<RadialLayoutNode>();

            if(!this.linksRebuildList.Contains(node))
                this.linksRebuildList.Add(node);

            this.linksRebuildFrameCounter = 1;
            if (!this.enabled)
                this.enabled = true;
        }
        

        /// <summary>
        /// Destroys this layout and every child node
        /// </summary>
		public void Destroy()
		{
            DestroyGameObject(this.linksRoot.gameObject);
            DestroyGameObject(this.gameObject);
        }

#endregion

#region Queries and Search

        public int CountNodesOfDepth(int depth)
        {
            int c = 0;

            foreach (Transform t in this.transform)
                if (t.GetComponent<RadialLayoutNode>() != null && t.GetComponent<RadialLayoutNode>().depth == depth)
                    c++;
            return c;
        }


        /// <summary>
        /// Returns he specified child node by child index (Counting only child nodes).
        /// </summary>
        /// <param name="childIndex">Index of the node to be returned, counting only nodes objects in the Transform.</param>
        /// <returns></returns>
        public RadialLayoutNode GetNodeByChildIndex(int childIndex)
        {
            int k = 0;
            foreach (Transform t in this.transform)
            {
                var node = t.GetComponent<RadialLayoutNode>();
                if (node != null)
                {
                    if (k == childIndex)
                    {
                        return node;
                    }
                    k++;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the list of all sub-layouts.
        /// </summary>
        /// <param name="recursive">TRUE: searches in child sub-layouts also. FALSE: returns only local sub-layouts.</param>
        public RadialLayout[] GetSubLayouts(bool recursive)
        {
            List<RadialLayout> subLayouts = new List<RadialLayout>(this.subLayouts);
            if (recursive)
            {
                for(int k=0;k<this.subLayouts.Count;k++)
                    subLayouts.AddRange(this.subLayouts[k].GetSubLayouts(true));
            }

            return subLayouts.ToArray();
        }

        /// <summary>
        /// Returns the master (root) layout. Returns itself if this layout is the root.
        /// </summary>
        public RadialLayout GetMasterLayout()
        {
            RadialLayout master = this;

            while (master.ParentLayout != null)
                master = master.ParentLayout;

            return master;
        }

        /// <summary>
        /// Returns the first parent layout found, moving from this to toward the root.
        /// </summary>
        public RadialLayout GetParentLayout()
        {
            RadialLayout parent = this;

            while (parent.ParentLayout != null)
                parent = parent.ParentLayout;

            return parent;
        }

#endregion

#region Node Management

        /// <summary>
        /// Adds a new node as child of this layout.
        /// </summary>
        /// <param name="name">Name of the node, default name NODE_## will be used if left blank.</param>
        /// <param name="parentNode">Node to be used as parent, if NULL the layout will be used.</param>
        public void AddNode(string name = "",RadialLayoutNode parentNode = null)
        {
            GameObject newNode = null;

            Transform targetTransform = parentNode == null ? this.transform : parentNode.transform;

            if (this.prefab_node == null)
            {
                // No prefab specified, creating a new RadialLayoutNode GameObject
                if (string.IsNullOrEmpty(name))
                {
                    // Finding first free number
                    int freeNumber = parentNode == null ? this.CountNodesOfDepth(0) : parentNode.GetSiblingsCount();
                    List<int> numbersFound = new List<int>();
                    foreach (Transform t in targetTransform)
                    {
                        if (t.GetComponent<RadialLayoutNode>() != null)
                        {
                            if (t.name.Contains("_"))
                            {
                                string[] splitName = t.name.Split('_');
                                if (splitName.Length == 2 && int.TryParse(splitName[1], out int parsedNumber))
                                    if (!numbersFound.Contains(parsedNumber))
                                        numbersFound.Add(parsedNumber);
                            }
                        }
                    }
                    for (int k = 0; k < freeNumber; k++)
                        if (!numbersFound.Contains(k))
                        {
                            freeNumber = k;
                            break;
                        }

                    name = "Node_" + freeNumber.ToString();
                }
                newNode = new GameObject(name, typeof(RadialLayoutNode), typeof(RectTransform));

#if UNITY_EDITOR
                if (newNode != null)
                    UnityEditor.Undo.RegisterCreatedObjectUndo(newNode.gameObject, $"Add Node to Layout");
#endif
            }
            else
            {
                // Using the specified RadialLayoutNode
#if UNITY_EDITOR
                if(this.keepPrefabLinking)
                    newNode = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(this.prefab_node.gameObject);
                else
                    newNode = GameObject.Instantiate(this.prefab_node.gameObject);

                if (newNode != null)
					UnityEditor.Undo.RegisterCreatedObjectUndo(newNode.gameObject, $"Add Node to Layout");
#else
                newNode = GameObject.Instantiate(this.prefab_node.gameObject);
#endif


            }

            newNode.transform.SetParent(targetTransform);

            this.Rebuild();

            // Query system: adding target component if enabled
            if (this.UseQuerySystem)
            {
                if(newNode.GetComponent<RadialLayoutQueryTarget>() == null)
                    newNode.AddComponent<RadialLayoutQueryTarget>();
                RadialLayoutQueryManager.AssignUniqueIdentifiers(this.GetMasterLayout());
            }
        }

        /// <summary>
        /// Deletes the specified child node by child index (Counting only child nodes).
        /// </summary>
        /// <param name="childIndex">Index of the node to be deleted, counting only nodes objects in the Transform.</param>
        public void DeleteNode(int childIndex,bool destroyChildren)
        {
            this.DeleteNode(this.GetNodeByChildIndex(childIndex),destroyChildren);
        }

        /// <summary>
        /// Deletes the specified node.
        /// </summary>
        /// <param name="node">Node to be deleted</param>
        /// <param name="destroyChildren">If true, child nodes of the target nodes will be destroyed recursively. Will throw an exception if false and target node has children.</param>
        public void DeleteNode(RadialLayoutNode node,bool destroyChildren)
        {
            if (node == null)
            {
                Debug.LogError("Trying do delete a NULL node.");
                return;
            }

            foreach (var registeredNode in this.Nodes)
            {
                if (registeredNode == node)
                {
                    // Checking for children nodes
                    foreach (Transform t in node.transform)
                    {
                        var childNode = t.GetComponent<RadialLayoutNode>();
                        if (childNode != null)
                        {
                            if (destroyChildren)
                                DeleteNode(childNode, true);
                            else
                                throw new Exceptions.DeleteNodeException(node, "Node contains child nodes and cannot be deleted. Use destroyChilderns = true if tyou want to force deletion.");
                        }
                    }

                    this.Nodes.Remove(node);

                    // If sub-layout deleting the links root
                    if (node.IsSubLayout)
                        DestroyGameObject(node.GetSubLayout().linksRoot.gameObject);

                    DestroyGameObject(node.gameObject,!destroyChildren);

                    break;
                }
            }
        }

#endregion

#region Customization

        /// <summary>
        /// Shows/Hides the graphical Inner Node of this layout.
        /// </summary>
        public void ShowInnerNode(bool show)
        {
            this.innerNode.SetActive(show);
            if (show)
                this.innerNode.transform.localScale = Vector3.one * this.innerNodeScale;
        }

#endregion

#region Query System

        private void SetupQuerySystem(bool enabled)
        {
            this._useQuerySystem = enabled;

            if (enabled)
            {
                // Adding RadialLayoutQueryTarget to self
                if(!this.IsSubLayout && this.GetComponent<RadialLayoutQueryTarget>() == null)
                    this.gameObject.AddComponent<RadialLayoutQueryTarget>();

                // Adding RadialLayoutQueryTarget to each node and assigning an ID
                foreach (var node in this.Nodes)
                {
                    if (node.GetComponent<RadialLayoutQueryTarget>() == null)
                    {
                        var qt = node.gameObject.AddComponent<RadialLayoutQueryTarget>();
                    }
                }

                foreach (var sublayout in this.GetSubLayouts(true))
                {
                    foreach (var node in sublayout.Nodes)
                    {
                        if (node.GetComponent<RadialLayoutQueryTarget>() == null)
                        {
                            var qt = node.gameObject.AddComponent<RadialLayoutQueryTarget>();
                        }
                    }
                }

                RadialLayoutQueryManager.AssignUniqueIdentifiers(this);
            }
            else
            {
                // Removing RadialLayoutQueryTarget from self
                if (!this.IsSubLayout && this.GetComponent<RadialLayoutQueryTarget>() != null)
                {
#if UNITY_EDITOR
                    if (Application.isPlaying)
                        Destroy(this.GetComponent<RadialLayoutQueryTarget>());
                    else
                        DestroyImmediate(this.GetComponent<RadialLayoutQueryTarget>());
#else
                        Destroy(this.GetComponent<RadialLayoutQueryTarget>());
#endif
                }

                // Removing RadialLayoutQueryTarget components from nodes
                foreach (var node in this.Nodes)
                    if (node.GetComponent<RadialLayoutQueryTarget>() != null)
                    {
#if UNITY_EDITOR
                        if (Application.isPlaying)
                            Destroy(node.GetComponent<RadialLayoutQueryTarget>());
                        else
                            DestroyImmediate(node.GetComponent<RadialLayoutQueryTarget>());
#else
                        Destroy(node.GetComponent<RadialLayoutQueryTarget>());
#endif
                    }

                foreach (var sublayout in this.GetSubLayouts(true))
                {
                    foreach (var node in sublayout.Nodes)
                    {
                        if (node.GetComponent<RadialLayoutQueryTarget>() != null)
                        {
#if UNITY_EDITOR
                            if (Application.isPlaying)
                                Destroy(node.GetComponent<RadialLayoutQueryTarget>());
                            else
                                DestroyImmediate(node.GetComponent<RadialLayoutQueryTarget>());
#else
                            Destroy(node.GetComponent<RadialLayoutQueryTarget>());
#endif
                        }
                    }
                }
            }

        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// Wrapper function to handle GameObject deletion in Editor vs. Play mode.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="undoable"></param>
        public static void DestroyGameObject(GameObject gameObject, bool undoable = false)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (!undoable)
                    GameObject.DestroyImmediate(gameObject);
                else
                    UnityEditor.Undo.DestroyObjectImmediate(gameObject);
            }
            else
            {
                GameObject.Destroy(gameObject);
            }
#else
            GameObject.Destroy(gameObject);
#endif
        }

        #endregion

        #region Misc
        /// <summary>
        /// Resets update parameters to the current state so, a refresh will not be triggered next frame.
        /// </summary>
        public void ResetUpdateParameters()
        {
            this.lastChildCount = this.transform.childCount;
            this.lastCircleRadius = this.circleRadius;
            this.lastCircleSlice = this.circleSlice;
            this.lastRotationOffset = this.rotationOffset;
            this.lastNodeScaleFactor = this.nodeScaleFactor;
            this.lastScaleNodeWithRadius = this.scaleNodeWithRadius;
            this.lastNodeScale_min = this.nodeScale_min;
            this.lastNodeScale_max = this.nodeScale_max;
            this.lastEnableSeparateExternalRadii = this.enableSeparateExternalRadii;
            this.lastNodesDistribution = this.nodesDistribution;
            this.lastFanDistributionCommonSpan = this.fanDistributionCommonSpan;
            this.lastsBranchesCommonLenght = this.branchesCommonLength;
            this.lastShowInnerNode = this.showInnerNode;
            this.lastInnerNodeScale = this.innerNodeScale;
            this.lastShowInnerLinks = this.showInnerLinks;
            this.lastRelativeRotation = this.relativeRotationMode;
        }


#if UNITY_EDITOR
        private void OnUndoPerformed()
        {
            if (this.autoRebuildMode != AutoRebuildMode.Never)
                this.Rebuild();
        }
#endif

#endregion
    }
}
