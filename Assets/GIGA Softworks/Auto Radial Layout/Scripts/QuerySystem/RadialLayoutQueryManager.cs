using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GIGA.AutoRadialLayout.QuerySystem
{
    public static class RadialLayoutQueryManager
    {
        /// <summary>
        /// Scans all nodes in the given layout and assigns an UniqueId to each of them.
        /// Only nodes with the RadialLayoutQueryTarget component will be considered. This component is added automatically when checking the UseQuerySystem flag of the Layout.
        /// </summary>
        public static void AssignUniqueIdentifiers(RadialLayout layout)
        {
            List<RadialLayout> subLayouts = new List<RadialLayout>();
            List<int> usedIds = new List<int>();
            List<RadialLayoutNode> allNodes = new List<RadialLayoutNode>();
            RadialLayoutQueryTarget qt = null;

            qt = layout.GetComponent<RadialLayoutQueryTarget>();
            if (qt != null && qt.UniqueId != -1)
            {
                usedIds.Add(qt.UniqueId);
            }

            foreach (var node in layout.Nodes)
            {
                qt = node.GetComponent<RadialLayoutQueryTarget>();
                if (qt != null)
                {
                    // Checking consistency & adding id to list
                    if (usedIds.Contains(qt.UniqueId))
                    {
                        Debug.LogWarning($"Found duplicate uniqueId {qt.UniqueId}, resetting to -1");
                        qt.ForceUniqueId(-1);
                    }

                    if (qt.UniqueId >= 0)
                        usedIds.Add(qt.UniqueId);
                }
                allNodes.Add(node);

                if (node.depth == 0)
                    subLayouts.AddRange(node.ScanForSubLayouts(null));
            }

            foreach (var subLayout in subLayouts)
            {
                foreach (var s_node in subLayout.Nodes)
                {
                    qt = s_node.GetComponent<RadialLayoutQueryTarget>();
                    if (qt != null)
                    {
                        // Checking consistency & adding id to list
                        if (usedIds.Contains(qt.UniqueId))
                        {
                            Debug.LogWarning($"Found duplicate uniqueId {qt.UniqueId}, resetting to -1");
                            qt.ForceUniqueId(-1);
                        }

                        if (qt.UniqueId >= 0)
                            usedIds.Add(qt.UniqueId);
                    }
                    allNodes.Add(s_node);
                }
            }

            // Assigning id to the layout
            qt = layout.GetComponent<RadialLayoutQueryTarget>();
            if (qt != null && qt.UniqueId == -1)
            {
                // Assigning the first free number found
                for (int k = 0; k < 99999; k++)
                {
                    if (!usedIds.Contains(k))
                    {
                        qt.ForceUniqueId(k);
                        usedIds.Add(k);
                        break;
                    }
                }
            }

            foreach (var node in allNodes)
            {
                qt = node.GetComponent<RadialLayoutQueryTarget>();
                if (qt != null && qt.UniqueId == -1)
                {
                    // Assigning the first free number found
                    for (int k = 0; k < 99999; k++)
                    {
                        if (!usedIds.Contains(k))
                        {
                            qt.ForceUniqueId(k);
                            usedIds.Add(k);
                            break;
                        }
                    }
                }
            }

            // Checking consistency
            foreach (var nodeA in allNodes)
            {
                RadialLayoutQueryTarget qtA = nodeA.GetComponent<RadialLayoutQueryTarget>();
                if (qtA != null)
                {
                    foreach (var nodeB in allNodes)
                    {
                        if (nodeB != nodeA)
                        {
                            RadialLayoutQueryTarget qtB = nodeB.GetComponent<RadialLayoutQueryTarget>();
                            if (qtB != null && qtA.UniqueId == qtB.UniqueId)
                                Debug.LogError("Found node with duplicate unique IDs : "+ qtB.UniqueId);
                        }
                    }
                }
            }

        }


        #region Queries

        /// <summary>
        /// Starting point for every query function
        /// </summary>
        /// <param name="layout">The Layout on which subsequent queries will be executed.</param>
        /// <returns>A RadialLayoutQueryResult containing the entire structure of the given Layout.</returns>
        public static RadialLayoutQueryResult Begin(RadialLayout layout)
        {
            RadialLayoutQueryResult result = new RadialLayoutQueryResult();

            if (!layout.UseQuerySystem || layout.GetComponent<RadialLayoutQueryTarget>() == null)
                Debug.LogWarning("Called Begin on a layout that with query system disabled. Did you check the UseQuerySystem flag?");

            foreach (var _node in layout.GetComponentsInChildren<RadialLayoutNode>())
                result.AddNode(_node);
            foreach (var _layout in layout.GetComponentsInChildren<RadialLayout>())
            {
                result.AddLayout(_layout);
                foreach (var link in _layout.linksRoot.GetComponentsInChildren<RadialLayoutLink>())
                    result.AddLink(link);
            }

            return result.End();
        }
        

        #endregion
    }
}
