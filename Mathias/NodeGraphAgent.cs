using System;
using System.Collections.Generic;
using System.Linq;
using Mathias.Utilities;

namespace Mathias
{
	public class NodeGraphAgent : NodeGraphAgentBase
	{
		private readonly Action innerUpdate;
		private readonly List<Node> nodesToVisit = new();

		private Node targetNode;
		private Node currentNode;
		private Node lastVisitedNode;
		private Node endNode;

		public NodeGraphAgent(NodeGraphBase nodeGraph, GradeType gradeType) : base(nodeGraph)
		{
			SetOrigin(width * 0.5f, height * 0.5f);

			if(nodeGraph.nodes.Count < 0) { throw new ArgumentException("The passed in node graph has no nodes"); }

			currentNode = nodeGraph.nodes[28];
			jumpToNode(currentNode);

			switch (gradeType)
			{
				case GradeType.Sufficient:
					nodeGraph.OnNodeLeftClicked += OnNodeClickedSufficient;
					innerUpdate += UpdateSufficient;
					break;

				case GradeType.Good:
					nodeGraph.OnNodeLeftClicked += OnNodeClickedGood;
					innerUpdate += UpdateGood;
					break;

				case GradeType.Excellent: throw new NotImplementedException();
				default: throw new ArgumentOutOfRangeException(nameof(gradeType), gradeType, null);
			}
		}

		protected override void Update() { innerUpdate.Invoke(); }

		private void OnNodeClickedSufficient(Node node)
		{
			if(targetNode == null && !currentNode.connections.Contains(node)) { return; } // Standing still

			if(nodesToVisit.Count == 0)
			{
				nodesToVisit.Add(node);
				return;
			}

			if(nodesToVisit.Count > 0 && !nodesToVisit.Last().connections.Contains(node)) { return; } // Walking path

			if(nodesToVisit.Last() == node) { return; }

			nodesToVisit.Add(node);
		}

		private void UpdateSufficient()
		{
			if(targetNode == null)
			{
				if(nodesToVisit.Count > 0) { targetNode = nodesToVisit[0]; }

				return;
			}

			if(!moveTowardsNode(targetNode)) { return; }

			currentNode = targetNode;
			nodesToVisit.RemoveAt(0);

			targetNode = nodesToVisit.Count > 0 ? nodesToVisit[0] : null;
		}

		private void OnNodeClickedGood(Node node)
		{
			if(targetNode != null) { return; } // Orc is walking towards a target.

			if(currentNode.connections.Contains(node)) // There is a direct connection to the target node.
			{
				endNode = node;
				targetNode = endNode;
				return;
			}

			endNode = node;
			int r = AlgorithmsAssignment.Random.Next(0, currentNode.connections.Count);
			targetNode = currentNode.connections[r];
			}

		private void UpdateGood()
		{
			if(targetNode == null) { return; } //Has no target.

			if(!moveTowardsNode(targetNode)) { return; } //Has not reached target.

			lastVisitedNode = currentNode;
			currentNode = targetNode;

			if(currentNode == endNode) // Has arrived. 
			{
				targetNode = null;
				return;
			}

			if(currentNode.connections.Contains(endNode)) // Has direct path to the end node.
			{
				targetNode = endNode;
				return;
			}

			do
			{
				int r = AlgorithmsAssignment.Random.Next(0, currentNode.connections.Count);
				targetNode = currentNode.connections[r];
			} while (targetNode == lastVisitedNode); //Select random node which is not the last visited node.
		}
	}
}