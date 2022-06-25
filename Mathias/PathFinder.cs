using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Debug = Mathias.Utilities.Debug;

namespace Mathias
{
	public class PathFinder : PathFinderBase
	{
		public enum SearchType
		{
			Recursive,
			Iterative
		}

		private readonly SearchType searchType;

		private List<Node> path;

		public PathFinder(NodeGraphBase pGraph, SearchType searchType) : base(pGraph)
		{
			this.searchType = searchType;
			path = new List<Node>();
		}

		protected override List<Node> generate(Node from, Node to)
		{
			Stopwatch stopwatch = new();
			stopwatch.Start();

			path = new List<Node>();

			switch (searchType)
			{
				case SearchType.Recursive:
					FindPathRecursive(from, to, new List<Node>());
					break;

				case SearchType.Iterative:
					List<Node> nodes = FindPathIterative(from, to);
					path = nodes;
					break;

				default: throw new ArgumentOutOfRangeException();
			}

			Debug.Log($"Path found of {path.Count} nodes");
			Debug.Initialized(this, stopwatch.ElapsedMilliseconds);
			stopwatch.Stop();
			return path;
		}

		private void FindPathRecursive(Node checkingNode, Node endNode, List<Node> parentPath)
		{
			List<Node> currentPath = parentPath.ToList();
			currentPath.Add(checkingNode);

			//If the path is set and the currentPath is already longer than the path, the currentPath is irrelevant. 
			if(path.Count != 0 && currentPath.Count > path.Count) { return; }

			//Path is found.
			if(checkingNode == endNode) { path = currentPath; }

			//If the parent path does not contain the connection node call the function again.
			foreach (Node connectedNode in checkingNode.connections)
			{
				if(parentPath.Contains(connectedNode)) { continue; }

				FindPathRecursive(connectedNode, endNode, currentPath);
			}
		}

		private List<Node> FindPathIterative(Node from, Node to)
		{
			LinkedList<Node> nodes = new();
			Dictionary<Node, Node> childParentSet = new();

			nodes.AddLast(from);
			
			while (nodes.Count > 0)
			{
				Node node = nodes.First();
				nodes.RemoveFirst();
				if(node == to)
				{
					List<Node> endList = new();
					Node current = node;
					while (current != null && current != from)
					{
						endList.Add(current);
						current = childParentSet[current];
					}

					if(current == null) { return null; }

					endList.Add(from);
					endList.Reverse();
					foreach (Node n in endList) { Console.WriteLine(n); }

					return endList;
				}
				
				foreach (Node connectedNode in node.connections)
				{
					if(childParentSet.ContainsKey(connectedNode))
					{
						continue;
					}

					childParentSet.Add(connectedNode, node);
					nodes.AddLast(connectedNode);
				}
			}

			return null;
		}
	}
}