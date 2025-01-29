using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GIGA.AutoRadialLayout.QuerySystem
{
	public class RadialLayoutQueryResult
	{
		public RadialLayoutNode[] Nodes { get; private set; }
		public RadialLayout[] Layouts { get; private set; }
		public RadialLayoutLink[] Links { get; private set; }

		private List<RadialLayoutNode> nodes = new List<RadialLayoutNode>();
		private List<RadialLayout> layouts = new List<RadialLayout>();
		private List<RadialLayoutLink> links = new List<RadialLayoutLink>();

		internal void AddNode(RadialLayoutNode node)
		{
			if (node != null && !this.nodes.Contains(node))
				nodes.Add(node);
		}

		internal void AddLayout(RadialLayout layout)
		{
			if (layout != null && !this.layouts.Contains(layout))
				layouts.Add(layout);
		}

		internal void AddLink(RadialLayoutLink link)
		{
			if (link != null && !this.links.Contains(link))
				links.Add(link);
		}

		internal RadialLayoutQueryResult End()
		{
			RadialLayoutQueryResult result = new RadialLayoutQueryResult()
			{
				Nodes = this.nodes.ToArray(),
				Layouts = this.layouts.ToArray(),
				Links = this.links.ToArray(),
			};
			return result;
		}

		#region Operators overloading

		// Operators overload
		public static RadialLayoutQueryResult operator |(RadialLayoutQueryResult a, RadialLayoutQueryResult b) // OR Operator
		{
			RadialLayoutQueryResult result = new RadialLayoutQueryResult();

			result.Nodes = a.Nodes.Union(b.Nodes).ToArray();
			result.Layouts = a.Layouts.Union(b.Layouts).ToArray();
			result.Links = a.Links.Union(b.Links).ToArray();

			return result;
		}

		public static RadialLayoutQueryResult operator &(RadialLayoutQueryResult a, RadialLayoutQueryResult b) // AND Operator
		{
			RadialLayoutQueryResult result = new RadialLayoutQueryResult();

			result.Nodes = a.Nodes.Intersect(b.Nodes).ToArray();
			result.Layouts = a.Layouts.Intersect(b.Layouts).ToArray();
			result.Links = a.Links.Intersect(b.Links).ToArray();

			return result;
		}

		public static RadialLayoutQueryResult operator -(RadialLayoutQueryResult a, RadialLayoutQueryResult b) // SUBTRACT Operator
		{
			RadialLayoutQueryResult result = new RadialLayoutQueryResult();

			result.Nodes = a.Nodes.Except(b.Nodes).ToArray();
			result.Layouts = a.Layouts.Except(b.Layouts).ToArray();
			result.Links = a.Links.Except(b.Links).ToArray();

			return result;
		}

		public static RadialLayoutQueryResult operator +(RadialLayoutQueryResult a, RadialLayoutQueryResult b) // ADD Operator (same as OR)
		{
			return a | b;
		}

		#endregion
	}

	/// <summary>
	/// Extension methods of the RadialLayoutQueryResult class, used to build the query
	/// </summary>
	public static class RadialLayoutQueryResultExtensions
	{
		#region Node search

		/// <summary>
		/// Returns the query result containing the node with the corresponding unique ID
		/// </summary>
		/// <param name="id">The id to search for</param>
		/// <returns>A RadialLayoutQueryResult containing the node, if found.</returns>
		public static RadialLayoutQueryResult GetNodeResultWithUniqueId(this RadialLayoutQueryResult prevResult, int id)
		{
			RadialLayoutQueryResult result = new RadialLayoutQueryResult();

			foreach (var node in prevResult.Nodes)
			{
				RadialLayoutQueryTarget qt = node.GetComponent<RadialLayoutQueryTarget>();
				if (qt != null)
				{
					if (qt.UniqueId == id)
					{
						result.AddNode(qt.GetComponent<RadialLayoutNode>());
						return result.End();
					}
				}

			}

			return result.End();
		}

		/// <summary>
		/// Returns the the node with the corresponding unique ID
		/// </summary>
		/// <param name="id">The id to search for</param>
		/// <returns>A RadialLayoutNode with the specified Id, if found.</returns>
		public static RadialLayoutNode GetNodeWithUniqueId(this RadialLayoutQueryResult prevResult, int id)
		{
			foreach (var node in prevResult.Nodes)
			{
				RadialLayoutQueryTarget qt = node.GetComponent<RadialLayoutQueryTarget>();
				if (qt != null)
				{
					if (qt.UniqueId == id)
					{
						return qt.GetComponent<RadialLayoutNode>();
					}
				}

			}

			return null;
		}

		/// <summary>
		/// Returns all nodes in the layout. NOTE: only nodes with the RadialLayoytQueryTarget component will be considered.
		/// </summary>
		/// <param name="includeLayouts">If TRUE, nodes that are also sub-layouts will be included in the result (default TRUE)</param>
		public static RadialLayoutQueryResult GetAllNodes(this RadialLayoutQueryResult prevResult, bool includeLayouts = true)
		{
			RadialLayoutQueryResult result = new RadialLayoutQueryResult();

			foreach (var node in prevResult.Nodes)
			{
				RadialLayoutQueryTarget qt = node.GetComponent<RadialLayoutQueryTarget>();
				if (qt != null)
				{
					if (includeLayouts || !node.IsSubLayout)
					{
						result.AddNode(node);

						if (includeLayouts && node.IsSubLayout)
							result.AddLayout(node.GetSubLayout());

					}
				}
			}

			if (includeLayouts)
			{
				foreach (var layout in prevResult.Layouts)
				{
					RadialLayoutQueryTarget qt = layout.GetComponent<RadialLayoutQueryTarget>();
					if (qt != null)
					{
						result.AddLayout(layout);
					}
				}
			}

			return result.End();
		}


		/// <summary>
		/// Returns all the nodes containing the specified tag.
		/// </summary>
		/// <param name="tags">The tags to search for. Can be a comma separated list that works as an OR. (Returns nodes with at least one of the tags specified)</param>
		/// <param name="includeLayouts">If TRUE, nodes that are also sub-layouts will be included in the result (default TRUE)</param>
		public static RadialLayoutQueryResult GetNodesWithTags(this RadialLayoutQueryResult prevResult, string tags, bool includeLayouts = true)
		{
			RadialLayoutQueryResult result = new RadialLayoutQueryResult();

			if (!string.IsNullOrEmpty(tags))
			{
				var splitTags = tags.Split(',');

				foreach (var node in prevResult.Nodes)
				{
					RadialLayoutQueryTarget qt = node.GetComponent<RadialLayoutQueryTarget>();
					if (qt != null)
					{
						foreach (var tag in splitTags)
						{
							if (qt.tags.Contains(tag))
							{
								if (includeLayouts || !node.IsSubLayout)
								{
									result.AddNode(node);

									if (includeLayouts && node.IsSubLayout)
										result.AddLayout(node.GetSubLayout());
									break;

								}
							}
						}
					}
				}

				if (includeLayouts)
				{
					foreach (var layout in prevResult.Layouts)
					{
						RadialLayoutQueryTarget qt = layout.GetComponent<RadialLayoutQueryTarget>();
						if (qt != null)
						{
							foreach (var tag in splitTags)
							{
								if (qt.tags.Contains(tag))
								{
									result.AddLayout(layout);
									break;
								}
							}
						}
					}
				}
			}
			else
				result = result.End() | prevResult;

			return result.End();
		}

		#endregion

		#region Layout search

		/// <summary>
		/// Returns all layouts and sublayouts. NOTE: only RadialLayoytQueryTarget components will be considered.
		/// </summary>
		/// <param name="subLayoutsOnly">If TRUE, returns only sub-layouts.</param>
		public static RadialLayoutQueryResult GetAllLayouts(this RadialLayoutQueryResult prevResult, bool subLayoutsOnly = false)
		{
			RadialLayoutQueryResult result = new RadialLayoutQueryResult();

			foreach (var layout in prevResult.Layouts)
			{
				if(!layout.IsSubLayout && !subLayoutsOnly)
				{
					RadialLayoutQueryTarget qt = layout.GetComponent<RadialLayoutQueryTarget>();
					if (qt != null)
					{
						result.AddLayout(layout);
					}
				}
				else
				{
					// Looking for RadialLayoutQueryTarget component in parent node for sub-layouts
					RadialLayoutQueryTarget qt = layout.GetComponentInParent<RadialLayoutQueryTarget>();
					if (qt != null)
					{
						result.AddLayout(layout);
					}
				}
			}

			return result.End();
		}

		/// <summary>
		/// Returns all the layouts containing the specified tag.
		/// </summary>
		/// <param name="tags">The tags to search for. Can be a comma separated list that works as an OR.</param>
		public static RadialLayoutQueryResult GetLayoutsWithTags(this RadialLayoutQueryResult prevResult, string tags, bool subLayoutsOnly = true)
		{
			RadialLayoutQueryResult result = new RadialLayoutQueryResult();

			if (!string.IsNullOrEmpty(tags))
			{
				var splitTags = tags.Split(',');

				foreach (var layout in prevResult.Layouts)
				{
					RadialLayoutQueryTarget qt = null;

					if (!layout.IsSubLayout && !subLayoutsOnly)
					{
						qt = layout.GetComponent<RadialLayoutQueryTarget>();
					}
					else
					{
						// Looking for RadialLayoutQueryTarget component in parent node for sub-layouts
						qt = layout.GetComponentInParent<RadialLayoutQueryTarget>();
					}

					if (qt != null)
					{
						foreach (var tag in splitTags)
						{
							if (qt.tags.Contains(tag))
							{
								result.AddLayout(layout);
								break;
							}
						}
					}
				}
			}
			else
				result = result.End() | prevResult;

			return result.End();
		}

		#endregion

		#region Link search

		/// <summary>
		/// Returns all the links
		/// </summary>
		/// <param name="includeSubLayouts">Includes sub-layouts in the search</param>
		public static RadialLayoutQueryResult GetAllLinks(this RadialLayoutQueryResult prevResult, bool includeSubLayouts = true)
		{
			RadialLayoutQueryResult result = new RadialLayoutQueryResult();

			foreach (var layout in prevResult.Layouts)
			{
				if (!layout.IsSubLayout || includeSubLayouts)
				{
					foreach (var link in layout.linksRoot.GetComponentsInChildren<RadialLayoutLink>())
						result.AddLink(link);
				}
			}

			return result.End();
		}

		/// <summary>
		/// Returns the link (contained in a RadialLayoutQueryResult) connecting the 2 provided nodes
		/// </summary>
		/// <returns>The RadialLayoutQueryResult containing the link if found, or an emtpy RadialLayoutQueryResult otherwise.</returns>
		public static RadialLayoutQueryResult GetLink(this RadialLayoutQueryResult prevResult, RadialLayoutNode nodeA,RadialLayoutNode nodeB)
		{
			RadialLayoutQueryResult result = new RadialLayoutQueryResult();

			foreach (var link in prevResult.Links)
			{
				if (link.from != null && link.to != null)
				{
					if (link.from == nodeA && link.to == nodeB || link.from == nodeB && link.to == nodeA)
					{
						result.AddLink(link);
						break;
					}
				}
				else if (link.fromLayout != null)
				{
					if(nodeA.GetSubLayout() != null && nodeA.GetSubLayout() == link.fromLayout && link.to == nodeB ||
					   nodeB.GetSubLayout() != null && nodeB.GetSubLayout() == link.fromLayout && link.to == nodeA)
					{
						result.AddLink(link);
						break;
					}
				}
			}

			return result.End();

		}

		/// <summary>
		/// Returns the (inner) link connecting the layout and the provided node
		/// </summary>
		/// <returns></returns>
		public static RadialLayoutQueryResult GetLink(this RadialLayoutQueryResult prevResult, RadialLayout layout, RadialLayoutNode node)
		{
			RadialLayoutQueryResult result = new RadialLayoutQueryResult();

			foreach (var link in prevResult.Links)
			{
				if (link.fromLayout != null)
				{
					if (link.fromLayout == layout && link.to == node)
					{
						result.AddLink(link);
						break;
					}
				}
			}

			return result.End();

		}


		/// <summary>
		/// Returns all the links starting from the provided node
		/// </summary>
		/// <returns></returns>
		public static RadialLayoutQueryResult GetLinksStartingFrom(this RadialLayoutQueryResult prevResult, RadialLayoutNode node)
		{
			RadialLayoutQueryResult result = new RadialLayoutQueryResult();

			foreach (var link in prevResult.Links)
			{
				if (link.from != null && link.from == node)
				{
					result.AddLink(link);
				}
				else if (link.fromLayout != null && node.IsSubLayout && node.GetSubLayout() == link.fromLayout)
				{
					result.AddLink(link);
				}
			}

			return result.End();
		}

		/// <summary>
		/// Returns all the links starting from any of the provided nodes
		/// </summary>
		/// <returns></returns>
		public static RadialLayoutQueryResult GetLinksStartingFrom(this RadialLayoutQueryResult prevResult, RadialLayoutNode[] nodes)
		{
			RadialLayoutQueryResult result = new RadialLayoutQueryResult();

			foreach (var node in nodes)
			{
				var localResult = prevResult.GetLinksStartingFrom(node);
				foreach (var link in localResult.Links)
					result.AddLink(link);
			}

			return result.End();
		}

		/// <summary>
		/// Returns all the links starting from the provided layout
		/// </summary>
		/// <returns></returns>
		public static RadialLayoutQueryResult GetLinksStartingFrom(this RadialLayoutQueryResult prevResult, RadialLayout layout)
		{
			RadialLayoutQueryResult result = new RadialLayoutQueryResult();

			foreach (var link in prevResult.Links)
			{
				if (link.fromLayout != null && link.fromLayout == layout)
				{
					result.AddLink(link);
				}
			}

			return result.End();
		}


		/// <summary>
		/// Returns all the links going to the provided node
		/// </summary>
		/// <returns></returns>
		public static RadialLayoutQueryResult GetLinksGoingTo(this RadialLayoutQueryResult prevResult, RadialLayoutNode node)
		{
			RadialLayoutQueryResult result = new RadialLayoutQueryResult();

			foreach (var link in prevResult.Links)
			{
				if (link.to != null && link.to == node)
				{
					result.AddLink(link);
				}
			}

			return result.End();
		}

		/// <summary>
		/// Returns all the links going to any of the provided nodes
		/// </summary>
		/// <returns></returns>
		public static RadialLayoutQueryResult GetLinksGoingTo(this RadialLayoutQueryResult prevResult, RadialLayoutNode[] nodes)
		{
			RadialLayoutQueryResult result = new RadialLayoutQueryResult();

			foreach (var node in nodes)
			{
				var localResult = prevResult.GetLinksGoingTo(node);
				foreach (var link in localResult.Links)
					result.AddLink(link);
			}

			return result.End();
		}

		/// <summary>
		/// Returns the link connecting the two specified nodes. Null if not found.
		/// </summary>
		/// <returns></returns>
		public static RadialLayoutLink GetLinkConnecting(this RadialLayoutQueryResult prevResult, RadialLayoutNode nodeA,RadialLayoutNode nodeB)
		{
			if (nodeA != null && nodeB != null)
			{
				foreach (var link in prevResult.Links)
				{
					if (link.from == nodeA && link.to == nodeB || link.from == nodeB && link.to == nodeA)
						return link;
					else if (link.fromLayout != null && nodeA.IsSubLayout && link.fromLayout == nodeA.GetSubLayout() && link.to == nodeB)
						return link;
					else if (link.fromLayout != null && nodeB.IsSubLayout && link.fromLayout == nodeB.GetSubLayout() && link.to == nodeA)
						return link;
				}
			}

			return null;
		}

		/// <summary>
		/// Returns the link (as a direct RadialLayoutLink component) connecting the specified layout and node. Null if not found.
		/// </summary>
		/// <returns>The RadialLayoutLink component. Null if not found.</returns>
		public static RadialLayoutLink GetLinkConnecting(this RadialLayoutQueryResult prevResult, RadialLayout layout, RadialLayoutNode node)
		{
			if (layout != null && node != null)
			{
				foreach (var link in prevResult.Links)
				{
					if (link.fromLayout != null && link.fromLayout == layout && link.to == node)
						return link;
				}
			}

			return null;
		}



		#endregion

	}
}
