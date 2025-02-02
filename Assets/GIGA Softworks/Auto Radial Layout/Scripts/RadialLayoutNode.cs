using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace GIGA.AutoRadialLayout
{

    [ExecuteInEditMode]
    public class RadialLayoutNode : MonoBehaviour
    {
        public RadialLayout Layout { get; private set; }
        public RadialLayoutNode ParentNode { get; private set; }
        [ReadOnly]
        public int depth;
        private int lastChildCount = 0;
        
        // Offsets, etc..
        public float distanceOffset = 0;
        private float fanSpan = 45;
        public float fanSpanOverride = 90;
        public float angleOffset = 0;
        public float fanOffset = 0;
        public float branchLength;             // Used in "branches" distribution
        public float nodeRadius;               // Used to tune links fill effect to real %

        private float lastFanSpan,lastFanSpanOverride, lastAngleOffset,lastDistanceOffset,lastFanOffset,lastBranchLength;
        private bool lastOverrideFanSpan, lastOverrideBranchLength;

        // Flags
        public bool overrideFanSpan;
        public bool overrideBranchLength;
#if UNITY_EDITOR
        public bool showNodeRadiusGizmo;
#endif

        // Getters
        public bool IgnoreLayout { get { return this.GetComponent<LayoutElement>() != null && this.GetComponent<LayoutElement>().ignoreLayout; } }
        public bool IsSubLayout {
            get {
                foreach (Transform t in this.transform)
                    if (t.GetComponent<RadialLayout>() != null)
                        return true;
                return false;
                    } }
        /// <summary>
        /// Returns true if this node has children nodes associated.
        /// </summary>
        public bool HasChildren { get {
                if (this.Layout != null)
                {
                    foreach (var n in this.Layout.Nodes)
                        if (n.ParentNode == this)
                            return true;
                }
                return false;
            } }

        /// <summary>
        /// The span in degree of the branches (links) "fan" generating from this node.
        /// </summary>
        public float FanSpan { get {
                if (this.overrideFanSpan || this.Layout != null && this.Layout.nodesDistribution == RadialLayout.NodesDistribution.Branches)
                {
                    if (this.overrideFanSpan)
                        return this.fanSpanOverride;
                    else
                    {
                        return this.Layout.fanDistributionCommonSpan;
                    }
                }
                else
                    return this.fanSpan;
            } }

        /// <summary>
        /// The lenght of the branches (links) generating from this node.
        /// </summary>
        public float BranchLength
        {
            get
            {
                if (this.Layout.nodesDistribution == RadialLayout.NodesDistribution.Branches)
                {
                    if (this.overrideBranchLength)
                        return this.branchLength + this.Layout.circleRadius;
                    else
                        return this.Layout.circleRadius * this.Layout.branchesCommonLength;
                }
                else
                    return 0;
            }
        }

        /// <summary>
        /// The link that starts from this node.
        /// </summary>
        public List<RadialLayoutLink> DepartingLinks { get; private set; }

		/// <summary>
		/// The link that is going into this node.
		/// </summary>
		public RadialLayoutLink ArrivingLink { get; private set; }

		private void OnEnable()
		{
			if(this.Layout == null)
                this.Layout = this.GetComponentInParent<RadialLayout>();
            this.ResetUpdateParameters();

#if UNITY_EDITOR
            Undo.undoRedoPerformed += Update;
#endif
        }

		private void OnDisable()
		{
#if UNITY_EDITOR
			Undo.undoRedoPerformed -= Update;
#endif
        }

		void Update()
        {
            if (this.Layout != null && (!Application.isPlaying || this.Layout.autoRebuildMode == RadialLayout.AutoRebuildMode.Always && this.Layout.enabled))
            {
                if (this.transform.childCount != this.lastChildCount ||
                    this.fanSpan != lastFanSpan ||
                    this.fanSpanOverride != lastFanSpanOverride ||
                    this.angleOffset != this.lastAngleOffset ||
                    this.distanceOffset != this.lastDistanceOffset ||
                    this.overrideFanSpan != this.lastOverrideFanSpan ||
                    this.branchLength != this.lastBranchLength ||
                    this.overrideBranchLength != this.lastOverrideBranchLength ||
                    this.fanOffset != this.lastFanOffset
                    )
                {
                    if (this.Layout == null)
                        this.Layout = this.GetComponentInParent<RadialLayout>();
                    this.Layout.Rebuild();
                    this.ResetUpdateParameters();
                }
            }

#if UNITY_EDITOR
            if (this.Layout != null && this.Layout.GetMasterLayout().forceNodesSelection && Selection.activeGameObject != null && Selection.activeGameObject != this.gameObject && Selection.activeGameObject.transform.IsChildOf(this.transform) && Selection.activeGameObject.GetComponent<RadialLayout>() == null && Selection.activeGameObject.GetComponent<RadialLayoutNode>() == null && Selection.activeGameObject.GetComponentInParent<RadialLayoutNode>() != null) 
            {
                GameObject target = Selection.activeGameObject.GetComponentInParent<RadialLayoutNode>().gameObject; 

                if ((RadialLayout.forceSelectionTargetedObject == null || RadialLayout.forceSelectionTargetedObject != target) &&(RadialLayout.forceSelectionSkippedObject == null || RadialLayout.forceSelectionSkippedObject != Selection.activeGameObject))
                {
                    if(Selection.activeGameObject.GetComponent<RadialLayout>() == null && Selection.activeGameObject.GetComponent<RadialLayoutNode>() == null)
                        RadialLayout.forceSelectionSkippedObject = Selection.activeGameObject;
                    Selection.activeGameObject = Selection.activeGameObject.GetComponentInParent<RadialLayoutNode>().gameObject;
                    RadialLayout.forceSelectionTargetedObject = target;
                }
            }
#endif
        }

        #region Layout building

        /// <summary>
        /// Sets the parent layout.
        /// </summary>
        public void SetParent(RadialLayout layout)
        {
            this.Layout = layout;
        }

        /// <summary>
        /// Sets the parent node.
        /// </summary>
        public void SetParent(RadialLayoutNode parent)
        {
            this.ParentNode = parent;
        }

        /// <summary>
        /// Scans for subnodes and add them to the layout.
        /// </summary>
        /// <returns>Max depth found.</returns>
        public int ScanForSubNodes()
        {
            int maxDepth = Layout.MaxDepth;
            foreach (Transform t in this.transform)
            {
                RadialLayoutNode node = t.gameObject.GetComponent<RadialLayoutNode>();
                if (node != null && !node.IgnoreLayout)
                {
                    if(!this.Layout.Nodes.Contains(node))
                        this.Layout.Nodes.Add(node);
                    node.SetParent(this.Layout);
                    node.SetParent(this);
                    node.depth = this.depth + 1;
                    if (node.depth > maxDepth)
                        maxDepth = node.depth;
                    this.Layout.QueueNodeForLinkRebuild(node);
                    int subDepth = node.ScanForSubNodes();
                    if (subDepth > maxDepth)
                        maxDepth = subDepth;
                }
                else if (t.GetComponent<RadialLayoutNode>() != null)
                    GameObject.DestroyImmediate(t.GetComponent<RadialLayoutNode>());
            }

            return maxDepth;
        }

        /// <summary>
        /// Finds all sub-layouts starting from this node (included)
        /// </summary>
        /// <returns></returns>
        public RadialLayout[] ScanForSubLayouts(List<RadialLayout> excludeList)
        {
            List<RadialLayout> subLayouts = new List<RadialLayout>();

			var subLayout = this.GetSubLayout();
			if (subLayout != null && !subLayouts.Contains(subLayout) && (excludeList == null || !excludeList.Contains(subLayout)))
			{
				subLayouts.Add(subLayout);

				if (subLayout.ParentLayout == null)
				{
                    foreach (var layout in this.GetComponentsInParent<RadialLayout>())
                    {
                        if (layout.Nodes.Contains(this))
                        {
                            subLayout.SetParentLayout(layout);
                            break;
                        }
                    }
				}
			}

			foreach (Transform t in this.transform)
            {
                RadialLayoutNode node = t.gameObject.GetComponent<RadialLayoutNode>();
                if (node != null)
                {
                    subLayouts.AddRange(node.ScanForSubLayouts(null));
                }
            }

            return subLayouts.ToArray();
        }

        /// <summary>
        /// Creates links terminating in this node.
        /// This function is automatically called on Layout.Rebuild().
        /// </summary>
        public void CreateLink()
        {
            if (this.Layout != null && this.Layout.prefab_link != null)
            {
                RadialLayoutLink newLink = null;

				if (this.ParentNode != null && this != null)
                {
                    // Checking if already existing
                    for (int k = this.Layout.linksRoot.transform.childCount - 1; k >= 0; k--)
                    {
                        RadialLayoutLink link = this.Layout.linksRoot.transform.GetChild(k).GetComponent<RadialLayoutLink>();
                        if (link != null && link.from == this.ParentNode && link.to == this)
                        {
                            if(this.ParentNode.DepartingLinks != null)
							    this.ParentNode.DepartingLinks.Remove(link);
							this.ArrivingLink = null;
                            if (this.Layout.GetMasterLayout().linksRebuildMode == RadialLayout.LinksRebuildMode.Regenerate)
                                RadialLayout.DestroyGameObject(link.gameObject);
                            else
                                newLink = link;
                        }
                    }

                    // Clearing all missing links
                    if (this.ParentNode.DepartingLinks != null)
                    {
                        for (int k = this.ParentNode.DepartingLinks.Count - 1; k >= 0; k--)
                            if (this.ParentNode.DepartingLinks[k] == null)
                                this.ParentNode.DepartingLinks.RemoveAt(k);
                    }

                    if(newLink == null)
                        newLink = GameObject.Instantiate(this.Layout.prefab_link, this.Layout.linksRoot.transform);
                    newLink.Set(this.ParentNode, this);
                    newLink.root = this.Layout.linksRoot.GetComponent<RadialLayoutLinksRoot>();
                    this.ArrivingLink = newLink;
                    if (this.ParentNode.DepartingLinks == null)
                        this.ParentNode.DepartingLinks = new List<RadialLayoutLink>();
                    if(!this.ParentNode.DepartingLinks.Contains(newLink))
                        this.ParentNode.DepartingLinks.Add(newLink);
                }
                else if (this.depth == 0 && this.Layout.showInnerLinks)
                {
					RadialLayoutNode subLayoutNode = null;
					if (this.Layout.IsSubLayout)
						subLayoutNode = this.Layout.GetComponentInParent<RadialLayoutNode>();

					// Checking if already existing
					for (int k = this.Layout.linksRoot.transform.childCount - 1; k >= 0; k--)
                    {
                        RadialLayoutLink link = this.Layout.linksRoot.transform.GetChild(k).GetComponent<RadialLayoutLink>();
                        if (link != null && link.from == null && link.to == this)
                        {
							this.ArrivingLink = null;
                            if (this.Layout.GetMasterLayout().linksRebuildMode == RadialLayout.LinksRebuildMode.Regenerate)
                            {
                                // removing from departing links of sub-layout nodes
                                if (subLayoutNode != null && subLayoutNode.DepartingLinks != null)
                                {
                                    if(subLayoutNode.DepartingLinks.Contains(newLink))
									    subLayoutNode.DepartingLinks.Remove(newLink);
								}

                                RadialLayout.DestroyGameObject(link.gameObject);
                            }
                            else
                                newLink = link;
                        }
					}

					// Clearing all missing links
					if (subLayoutNode != null && subLayoutNode.DepartingLinks != null)
					{
						for (int j = subLayoutNode.DepartingLinks.Count - 1; j >= 0; j--)
							if (subLayoutNode.DepartingLinks[j] == null)
								subLayoutNode.DepartingLinks.RemoveAt(j);
					}

					if (newLink == null)
                        newLink = GameObject.Instantiate(this.Layout.prefab_link, this.Layout.linksRoot.transform);
                    newLink.Set(this.Layout, this);
                    newLink.root = this.Layout.linksRoot.GetComponent<RadialLayoutLinksRoot>();
                    this.ArrivingLink = newLink;

                    // Adding departing links to sub-layout node
                    if (subLayoutNode != null)
                    {
                        if(subLayoutNode.DepartingLinks == null)
                            subLayoutNode.DepartingLinks = new List<RadialLayoutLink>();
                        if (newLink != null &&!subLayoutNode.DepartingLinks.Contains(newLink))
                            subLayoutNode.DepartingLinks.Add(newLink);
                    }
                }
            }
        }

        /// <summary>
        /// Places this Node accordingly to the parent layout settings.
        /// This function is automatically called on Layout.Rebuild().
        /// </summary>
        public void PlaceOnCircle()
        {

            Vector2 positionOffset = Vector2.zero;
            RectTransform rt = this.GetComponent<RectTransform>();
            int siblingIndex = this.GetSiblingIndex();
            int siblingsCount = this.GetSiblingsCount();

            this.fanSpan = this.CalculateDefaultFanSpan();
            this.lastFanSpan = this.fanSpan;

            // If at first depth, it is just the circle around the center
            if (this.depth == 0)
            {
                float rotationOffset = this.Layout.rotationOffset;
				if (this.Layout.IsSubLayout)
				{
					// Applying relative rotation offset if needed
					if (this.Layout.relativeRotationMode == RadialLayout.RelativeRotation.RelativeToParent)
					{
                        RadialLayout parent = this.Layout.ParentLayout;
                        while (parent != null)
                        {
                            rotationOffset += parent.rotationOffset;
                            parent = parent.ParentLayout;
                        }
                    }
                }
                float angle = 0;
                if (this.Layout.circleSlice == 360)
                    angle = (Mathf.PI * 2 / (float)this.Layout.CountNodesOfDepth(0)) * siblingIndex + (-rotationOffset) * Mathf.Deg2Rad + this.angleOffset * Mathf.Deg2Rad;
                else
                {
                    angle = (this.Layout.circleSlice * Mathf.Deg2Rad / ((float)this.Layout.CountNodesOfDepth(0) -1)) * (siblingIndex ) + (-rotationOffset) * Mathf.Deg2Rad + this.angleOffset * Mathf.Deg2Rad;
                }

				positionOffset = new Vector2(Mathf.Sin(angle) * (this.Layout.circleRadius + this.distanceOffset), Mathf.Cos(angle) * (this.Layout.circleRadius + this.distanceOffset));
                if(this.Layout.IsSubLayout)
                    positionOffset /= this.Layout.GetComponentInParent<RadialLayoutNode>().transform.localScale.x;

            }
            else
            {
                Vector2 bisector = Vector2.zero;
                switch (this.Layout.nodesDistribution)
                {
                    case RadialLayout.NodesDistribution.Concentric:
                        
                        bisector = (this.ParentNode.GetComponent<RectTransform>().position - this.Layout.GetComponent<RectTransform>().position).normalized;

                        // Placing nodes in the fan span if more than one sibling
                        if (siblingsCount > 1)
                        {
                            float angleSlice = this.ParentNode.FanSpan * Mathf.Deg2Rad / (float)siblingsCount;
                            if (siblingsCount % 2 == 0)
                            {
                                // even nodes, centering around the bisector
                                int centerIndex = siblingsCount / 2;
                                int offsetIndex = siblingIndex < centerIndex ? siblingIndex - centerIndex : siblingIndex - (centerIndex - 1);

                                bisector = bisector.Rotate(angleSlice * offsetIndex - angleSlice * 0.5f * Mathf.Sign(offsetIndex));
                            }
                            else
                            {
                                // odd nodes, centering the central node on the bisector
                                int centerIndex = siblingsCount / 2;
                                if (siblingIndex != centerIndex)
                                {
                                    int distanceFromCenterIndex = siblingIndex - centerIndex;
                                    bisector = bisector.Rotate(angleSlice * distanceFromCenterIndex);
                                }
                            }
                        }

                        if (this.angleOffset != 0)
                            bisector = bisector.Rotate(this.angleOffset * Mathf.Deg2Rad);
                        if (this.ParentNode != null && this.ParentNode.fanOffset != 0)
                            bisector = bisector.Rotate(this.ParentNode.fanOffset * Mathf.Deg2Rad);

                        positionOffset = bisector * (this.Layout.circleRadius * (this.depth + 1) * this.Layout.GetNodesRadiusMultiplier(this.depth) * this.Layout.Canvas.scaleFactor + this.GetInheritedDistanceOffset()  + this.distanceOffset * this.Layout.Canvas.scaleFactor);
                        positionOffset = this.ReconstructPositionOffset(positionOffset / this.ParentNode.transform.lossyScale.x);
                        break;
                    case RadialLayout.NodesDistribution.Branches:
                        bisector = Vector2.zero;
                        if (this.ParentNode.ParentNode == null)
                            bisector = (this.ParentNode.GetComponent<RectTransform>().position - this.Layout.GetComponent<RectTransform>().position).normalized;
                        else
                            bisector = (this.ParentNode.GetComponent<RectTransform>().position - this.ParentNode.ParentNode.GetComponent<RectTransform>().position).normalized;

                        // Placing nodes in the fan span if more than one sibling
                        if (siblingsCount > 1)
                        {
                            
                            if (siblingsCount % 2 == 0)
                            {
                                // even nodes, centering around the bisector
                                float angleSlice = this.ParentNode.FanSpan * Mathf.Deg2Rad / ((float)siblingsCount -1);
                                int centerIndex = siblingsCount / 2;
                                int offsetIndex = siblingIndex < centerIndex ? siblingIndex - centerIndex : siblingIndex - centerIndex +1;

                                bisector = bisector.Rotate(-this.ParentNode.FanSpan * 0.5f * Mathf.Deg2Rad + angleSlice * siblingIndex /*- angleSlice * 0.5f * Mathf.Sign(offsetIndex)*/);
                            }
                            else
                            {
                                // odd nodes, centering the central node on the bisector
                                float angleSlice = this.ParentNode.FanSpan * Mathf.Deg2Rad / ((float)siblingsCount - 1);
                                int centerIndex = siblingsCount / 2;
                                if (siblingIndex != centerIndex)
                                {
                                    int distanceFromCenterIndex = siblingIndex - centerIndex;
                                    bisector = bisector.Rotate(angleSlice * distanceFromCenterIndex);
                                }
                            }
                        }

                        if (this.angleOffset != 0)
                            bisector = bisector.Rotate(this.angleOffset * Mathf.Deg2Rad);
                        if (this.ParentNode != null && this.ParentNode.fanOffset != 0)
                            bisector = bisector.Rotate(this.ParentNode.fanOffset * Mathf.Deg2Rad);

                        float branchLength = this.ParentNode.BranchLength;

                        positionOffset = bisector * (branchLength * this.Layout.GetNodesRadiusMultiplier(this.depth) + this.distanceOffset);

                        break;
                }
            }

            rt.anchoredPosition = positionOffset;
        }

        public float CalculateDefaultFanSpan()
        {
            if (this.depth == 0)
            {
                return Mathf.Min(60, Mathf.Abs(360f / ((float)this.GetSiblingsCount())));

            }
            else
            {
                int parentSiblingsCount = this.ParentNode != null ? this.ParentNode.GetSiblingsCount() : 0;
                return Mathf.Min(60, Mathf.Abs((ParentNode.fanSpan / ((float)parentSiblingsCount - 4))));
            }
        }

        /// <summary>
        /// Clears all offsets and restores default node position.
        /// </summary>
        public void ClearOffsets()
        {
#if UNITY_EDITOR
            Undo.RecordObject(this, "Node clear offset");
#endif
            this.distanceOffset = 0;
            this.angleOffset = 0;
            this.overrideFanSpan = false;
            this.fanOffset = 0;
        }

        private Vector2 ReconstructPositionOffset(Vector2 fromPos)
        {
            Vector2 pos = fromPos;
            RadialLayoutNode parentNode = this.ParentNode;
            while (parentNode != null)
            {
                pos -= parentNode.GetComponent<RectTransform>().anchoredPosition / parentNode.transform.localScale.x;
                parentNode = parentNode.ParentNode;
                
            }

            return pos;// / this.transform.lossyScale.x;
        }


        private float GetInheritedDistanceOffset()
        {
            float offset = 0;
            var parentNode = this.ParentNode;
            while (parentNode != null)
            {
                offset += parentNode.distanceOffset;
                parentNode = parentNode.ParentNode;
            }
            return offset * this.Layout.Canvas.scaleFactor;
        }

        #endregion

        #region Queries and Search

        /// <summary>
        /// Returns the number of siblings of this Node (nodes with same depth).
        /// </summary>
        public int GetSiblingsCount()
        {
            int c = 0;
            if (this.ParentNode != null)
            {
                foreach (var child in this.ParentNode.GetComponentsInChildren<RadialLayoutNode>())
                    if (child.Layout == this.Layout && child.depth == this.depth)
                        c++;
            }
            else
            {
                // Counting nodes of the main layout
                foreach (var node in this.Layout.Nodes)
                {
                    if (node.Layout == this.Layout && node.depth == 0)
                        c++;
                }
            }
            return c;
        }

        /// <summary>
        /// Returns the index of this node in the parent hierarchy.
        /// </summary>
        public int GetSiblingIndex()
        {
            int siblingIndex = 0;
            foreach (Transform t in this.transform.parent)
                if (t.gameObject.activeSelf && t.GetComponent<RadialLayoutNode>() && !t.GetComponent<RadialLayoutNode>().IgnoreLayout)
                {
                    if (t.gameObject == this.gameObject)
                        break;
                    siblingIndex++;
                }
            return siblingIndex;
        }

        /// <summary>
        /// Returns all child nodes.
        /// </summary>
        /// <returns>Array containing all child nodes.</returns>
        public RadialLayoutNode[] GetChildNodes()
        {
            List<RadialLayoutNode> children = new List<RadialLayoutNode>();
            foreach (Transform t in this.transform)
            {
                var childNode = t.GetComponent<RadialLayoutNode>();
                if (childNode != null)
                {
                    children.Add(childNode);
                }
            }

            return children.ToArray();
        }

        /// <summary>
        /// Returns the sub-layout associated with this node, null if not found.
        /// </summary>
        public RadialLayout GetSubLayout()
        {
            foreach (Transform t in this.transform)
                if (t.GetComponent<RadialLayout>() != null)
                    return t.GetComponent<RadialLayout>();
            return null;
        }

        /// <summary>
        /// Converst this node to a sub-layout.
        /// </summary>
        public void ConvertToSubLayout()
        {
            if (this.IsSubLayout)
                return;

            var childNodes = this.GetChildNodes();

            RadialLayout newLayout = new GameObject("Sub-Layout",typeof(RectTransform) ,typeof(RadialLayout)).GetComponent<RadialLayout>();
            newLayout.transform.SetParent(this.transform);
            newLayout.transform.localPosition = Vector3.zero;
            newLayout.transform.localScale = Vector3.one;
            newLayout.transform.SetAsLastSibling();

            GameObject linksRoot = new GameObject("LinksRoot(Sub-Layout)", typeof(LayoutElement),typeof(RadialLayoutLinksRoot));
            linksRoot.GetComponent<LayoutElement>().ignoreLayout = true;
            linksRoot.GetComponent<RadialLayoutLinksRoot>().parentLayout = newLayout;
            linksRoot.transform.SetParent(this.Layout.GetMasterLayout().transform);
            linksRoot.transform.SetSiblingIndex(this.Layout.linksRoot.transform.GetSiblingIndex() + 1);

            newLayout.linksRoot = linksRoot;

            GameObject innerNode = GameObject.Instantiate(this.Layout.innerNode);
            innerNode.transform.SetParent(newLayout.transform);
            innerNode.transform.localPosition = Vector3.zero;
            innerNode.transform.rotation = Quaternion.identity;
            newLayout.innerNode = innerNode;

            // Copying settings
            newLayout.autoRebuildMode = this.Layout.autoRebuildMode;
            newLayout.nodesDistribution = this.Layout.nodesDistribution;
            newLayout.fanDistributionCommonSpan = this.Layout.fanDistributionCommonSpan;
            newLayout.circleRadius = this.Layout.circleRadius;
            newLayout.enableSeparateExternalRadii = this.Layout.enableSeparateExternalRadii;
            newLayout.externalCircleRadiusMultiplier = this.Layout.externalCircleRadiusMultiplier;
            newLayout.rotationOffset = this.Layout.rotationOffset;
            newLayout.scaleNodeWithRadius = false;
            newLayout.nodeScaleFactor = this.Layout.nodeScaleFactor;
            newLayout.nodeScale_min = this.Layout.nodeScale_min;
            newLayout.nodeScale_max = this.Layout.nodeScale_max;
            newLayout.showInnerNode = this.Layout.showInnerNode;
            newLayout.showInnerLinks = this.Layout.showInnerLinks;
            newLayout.innerNodeScale = this.Layout.innerNodeScale;
            newLayout.prefab_node = this.Layout.prefab_node;
            newLayout.prefab_link = this.Layout.prefab_link;
            newLayout.linksProgressMode = this.Layout.linksProgressMode;
            newLayout.linksProgressSpeed = this.Layout.linksProgressSpeed;
#if UNITY_EDITOR
            // Editor only settings
            newLayout.keepPrefabLinking = this.Layout.keepPrefabLinking;
#endif

            // Moving all child nodes to new sub-layout
            foreach (var n in childNodes)
            {
                n.transform.SetParent(newLayout.transform);
                n.SetParent((RadialLayoutNode)null);
            }

            // Rearranging links
            if (this.DepartingLinks != null)
            {
                for (int k = this.DepartingLinks.Count - 1; k >= 0; k--)
                {
                    if (this.DepartingLinks[k] != null)
                    {
                        this.DepartingLinks[k].transform.SetParent(linksRoot.transform);
                        this.DepartingLinks[k].Set(newLayout, this.DepartingLinks[k].to);
                    }
				}
            }

            newLayout.Rebuild();

			// Removing deleted links from previous rebuild (became inner links)
			if (this.DepartingLinks != null)
			{
				for (int k = this.DepartingLinks.Count - 1; k >= 0; k--)
				{
					if (this.DepartingLinks[k] == null)
					{
						this.DepartingLinks.RemoveAt(k);
					}
				}
			}

			// Editor settings
#if UNITY_EDITOR
			newLayout.forceNodesSelection = this.Layout.forceNodesSelection;
            newLayout.drawEditorGizmos = this.Layout.drawEditorGizmos;
            newLayout.drawEditorGizmos_circleRadius = this.Layout.drawEditorGizmos_circleRadius;
            newLayout.drawEditorGizmos_handles = this.Layout.drawEditorGizmos_handles;
            newLayout.drawEditorGizmos_nodes = this.Layout.drawEditorGizmos_nodes;
            newLayout.handle_lockRadius = this.Layout.handle_lockRadius;
            newLayout.handle_lockRotation = this.Layout.handle_lockRotation;
            newLayout.handle_snap = this.Layout.handle_snap;
            newLayout.handle_snap_intValues = this.Layout.handle_snap_intValues;
            newLayout.handle_snap_resolution = this.Layout.handle_snap_resolution;
#endif

            // registering to parent rebuild event
            newLayout.SetParentLayout(this.Layout);


            newLayout.UseQuerySystem = this.Layout.UseQuerySystem;

        }

        /// <summary>
        /// Reconverts this node to a simple node, if a sub-layout is found;
        /// </summary>
        public void ConvertToNode()
        {
            if (!this.IsSubLayout)
                return;

            // Moving all child outside the sublayout
            List<RadialLayoutNode> subLayoutNodes = new List<RadialLayoutNode>();
            foreach (Transform t in this.GetSubLayout().transform)
            {
                RadialLayoutNode node = t.GetComponent<RadialLayoutNode>();
                if (node != null)
                    subLayoutNodes.Add(node);
            }
            foreach (var node in subLayoutNodes)
            {
                node.transform.SetParent(this.transform);
                node.SetParent(this);
            }

            // Rearranging links
            foreach (var link in this.GetSubLayout().linksRoot.GetComponentsInChildren<RadialLayoutLink>())
            {
                link.Set(this, link.to);
            }

            // Destroy sub-layout
            RadialLayout.DestroyGameObject(this.GetSubLayout().linksRoot);
            RadialLayout.DestroyGameObject(this.GetSubLayout().gameObject);

			this.Layout.Rebuild();
        }

        #endregion

        #region Misc
        /// <summary>
        /// Resets update parameters to the current state so, a refresh will not be triggered next frame.
        /// </summary>
        public void ResetUpdateParameters()
        {
            this.lastChildCount = this.transform.childCount;
            this.lastFanSpan = this.fanSpan;
            this.lastAngleOffset = this.angleOffset;
            this.lastDistanceOffset = this.distanceOffset;
            this.lastFanSpanOverride = this.fanSpanOverride;
            this.lastOverrideFanSpan = this.overrideFanSpan;
            this.lastBranchLength = this.branchLength;
            this.lastOverrideBranchLength = this.overrideBranchLength;
            this.lastFanOffset = this.fanOffset;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
		{
            Color exColor = Gizmos.color;
            if (this.showNodeRadiusGizmo)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawWireSphere(this.transform.position, this.nodeRadius * this.transform.localScale.x * (this.Layout != null ? this.Layout.Canvas.scaleFactor : this.GetComponentInParent<Canvas>().scaleFactor));
            }

            // Links gizmo
            if (this.Layout != null && this.Layout.linksRoot.transform.childCount == 0 && this.Layout.prefab_link != null)
            {
                Gizmos.color = Color.gray;
                if (this.HasChildren)
                {
                    foreach (var child in this.GetChildNodes())
                    {
                        Gizmos.DrawLine(child.transform.position, this.transform.position);
                    }
                }
                else if (this.IsSubLayout)
                {
                    foreach(var node in this.GetSubLayout().Nodes)
                        if(node.depth == 0)
                            Gizmos.DrawLine(node.transform.position, this.transform.position);
                }

                if(this.ParentNode != null)
                    Gizmos.DrawLine(this.ParentNode.transform.position, this.transform.position);
                else if(this.depth == 0)
                    Gizmos.DrawLine(this.Layout.transform.position, this.transform.position);
            }

            Gizmos.color = exColor;
        }
#endif

#endregion


    }
}
