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

		public GradeType gradeType;
		private Node targetNode;
		private Node currentNode;
		private Node lastVisitedNode;
		private Node endNode;

		public NodeGraphAgent(NodeGraphBase nodeGraph, GradeType gradeType) : base(nodeGraph)
		{
			System.Diagnostics.Stopwatch stopwatch = new ();
			stopwatch.Start ();

			SetOrigin(width * 0.5f, height * 0.5f);

			if (nodeGraph.nodes.Count < 0) { throw new ArgumentException("The passed in node graph has no nodes"); }

			int randomNodeNumber = AlgorithmsAssignment.Random.Next(0, nodeGraph.nodes.Count);
			currentNode = nodeGraph.nodes[randomNodeNumber];
			jumpToNode(currentNode);

			this.gradeType = gradeType;
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

			Debug.Initialized(this, stopwatch.ElapsedMilliseconds);
			stopwatch.Stop();
		}

		protected override void Update() { innerUpdate.Invoke(); }

		#region Sufficient

		private void OnNodeClickedSufficient(Node node)
		{
			if (targetNode == null && !currentNode.connections.Contains(node)) { return; } // Standing still

			if (nodesToVisit.Count == 0)
			{
				nodesToVisit.Add(node);
				return;
			}

			if (nodesToVisit.Count > 0 && !nodesToVisit.Last().connections.Contains(node)) { return; } // Walking path

			if (nodesToVisit.Last() == node) { return; }

			nodesToVisit.Add(node);
		}

		private void UpdateSufficient()
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

		#endregion

		#region Good

		private void OnNodeClickedGood(Node node)
		{
			if (targetNode != null) { return; } // Orc is walking towards a target.

			if (currentNode.connections.Contains(node)) // There is a direct connection to the target node.
			{
				endNode = node;
				targetNode = endNode;
				return;
			}

			endNode = node;

			if (currentNode.connections == null || currentNode.connections.Count == 0)
			{
				Debug.LogError($"{currentNode} has no connections!");
				AlgorithmsAssignment.Instance.Destroy();
				return;
			}
			int randomNodeIndex = AlgorithmsAssignment.Random.Next(0, currentNode.connections.Count);
			targetNode = currentNode.connections[randomNodeIndex];
		}

		private void UpdateGood()
		{
			if (targetNode == null) { return; } //Has no target.

			if (!moveTowardsNode(targetNode)) { return; } //Has not reached target node.

			lastVisitedNode = currentNode;
			currentNode = targetNode;

			if (currentNode == endNode) // Has arrived. 
			{
				targetNode = null;
				return;
			}

			if (currentNode.connections.Contains(endNode)) // Has direct path to the end node.
			{
				targetNode = endNode;
				return;
			}

			if(currentNode.connections.Count == 1) //Has only one direction to go to.
			{
				targetNode = currentNode.connections[0];
				return;
			}

			do
			{
				int r = AlgorithmsAssignment.Random.Next(0, currentNode.connections.Count);
				targetNode = currentNode.connections[r];
			} while (targetNode == lastVisitedNode); //Select random node which is not the last visited node.
		}

		#endregion
	}
}