using System;
using System.Collections.Generic;
using Mathias.Utilities;

namespace Mathias.Agents
{
	public class PathFindingAgent : NodeGraphAgentBase
	{
		private readonly PathFinderBase pathFinder;

		private List<Node> path;
		private Node currentNode;
		private Node target;

		public PathFindingAgent(NodeGraphBase nodeGraph, PathFinderBase pathFinder) : base(nodeGraph)
		{
			this.pathFinder = pathFinder;

			int randomNode = AlgorithmsAssignment.Random.Next(0, nodeGraph.nodes.Count);
			currentNode = nodeGraph.nodes[randomNode];
			jumpToNode(currentNode);

			nodeGraph.OnNodeLeftClicked += OnLeftClicked;
		}

		protected override void Update()
		{
			if(target == null) { return; }

			if(!moveTowardsNode(target)) { return; }

			if(path.Count > 1) // Path not completed.
			{
				path.RemoveAt(0);
				target = path[0];
				return;
			}

			currentNode = target;
			target = null;
		}

		private void OnLeftClicked(Node node)
		{
			if(target != null) { return; } // Is walking.

			path = pathFinder.Generate(currentNode, node);
			if(path == null)
			{
				Debug.LogWaring("Path could not be found");
				return;
			}

			path.RemoveAt(0);
			target = path[0];
		}
	}
}