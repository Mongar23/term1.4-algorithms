using System;
using System.Collections.Generic;
using System.Linq;

namespace Mathias
{
	public class NodeGraphAgent : NodeGraphAgentBase
	{
		private readonly List<Node> nodesToVisit = new();

		private Node targetNode;
		private Node currentNode;

		public NodeGraphAgent(NodeGraphBase nodeGraph) : base(nodeGraph)
		{
			SetOrigin(width * 0.5f, height * 0.5f);

			if (nodeGraph.nodes.Count < 0) { throw new ArgumentException("The passed in node graph has no nodes"); }

			currentNode = nodeGraph.nodes[28];
			jumpToNode(currentNode);
			nodeGraph.OnNodeLeftClicked += OnNodeClicked;
		}

		protected override void Update()
		{
			if (targetNode == null)
			{
				if (nodesToVisit.Count > 0) { targetNode = nodesToVisit[0]; }

				return;
			}

			if (!moveTowardsNode(targetNode)) { return; }

			currentNode = targetNode;
			nodesToVisit.RemoveAt(0);

			targetNode = nodesToVisit.Count > 0 ? nodesToVisit[0] : null;
		}

		private void OnNodeClicked(Node node)
		{
			if (!currentNode.connections.Contains(node) && !nodesToVisit.Any(n => n.connections.Contains(node))) { return; }

			if (nodesToVisit.Count < 1)
			{
				nodesToVisit.Add(node);
				return;
			}

			if(nodesToVisit.Last() == node) {return;}
			nodesToVisit.Add(node);
		}
	}
}