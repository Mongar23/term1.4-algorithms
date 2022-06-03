using System;
using System.Collections.Generic;
using System.Linq;
using Mathias.Utilities;

namespace Mathias
{
	public class NodeGraphAgent : NodeGraphAgentBase
	{
		private List<Node> path = new();
		private Node targetNode;
		private Node currentNode;

		public NodeGraphAgent(NodeGraphBase nodeGraph) : base(nodeGraph)
		{
			SetOrigin(width * 0.5f, height * 0.5f);

			if (nodeGraph.nodes.Count < 0) { throw new ArgumentException("The passed in node graph has no nodes"); }

			currentNode = nodeGraph.nodes[AlgorithmsAssignment.Random.Next(0, nodeGraph.nodes.Count)];
			jumpToNode(currentNode);
			nodeGraph.OnNodeLeftClicked += OnNodeClicked;
		}

		protected override void Update()
		{
			if (targetNode == null) { return; }

			if (!moveTowardsNode(targetNode)) { return; }

			currentNode = targetNode;

			if (path.Contains(targetNode)) { path.Remove(targetNode); }

			if (path.Count > 0)
			{
				targetNode = path[0];
				return;
			}

			targetNode = null;
		}

		private void OnNodeClicked(Node node)
		{
			if (!node.connections.Contains(currentNode))
			{
				if (!FindPath(currentNode, node, new List<Node> {currentNode})) { Debug.LogError("Path could not be found"); }
				targetNode = path[0];
				return;
			}

			targetNode = node;
		}


		private bool FindPath(Node checkingNode, Node endNode, List<Node> parentPath)
		{
			if (checkingNode == null) { throw new ArgumentNullException(nameof(checkingNode), "checkingNode cannot be null"); }

			if (checkingNode == endNode)
			{
				path = parentPath;
				return true;
			}

			IEnumerable<Node> notCheckedNodes = checkingNode.connections.Where(n => !parentPath.Contains(n));
			foreach (Node child in notCheckedNodes)
			{
				List<Node> currentPath = new();
				currentPath.AddRange(parentPath);
				currentPath.Add(child);

				if (FindPath(child, endNode, currentPath)) { return true; }
			}

			return false;
		}
	}
}