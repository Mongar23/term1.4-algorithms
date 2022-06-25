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

		public PathFinder(NodeGraphBase pGraphBase, SearchType searchType) : base(pGraphBase)
		{
			this.searchType = searchType;
			path = new List<Node>();
		}

		protected override List<Node> generate(Node from, Node to)
		{
			Debug.Log("generate !!!!!!!!!");

			Stopwatch stopwatch = new();
			stopwatch.Start();

			Debug.Log(searchType);

			path = new List<Node>();

			switch (searchType)
			{
				case SearchType.Recursive:
					FindPathRecursive(from, to, new List<Node>());
					break;

				case SearchType.Iterative: 
					path = FindPathIterative(from, to);
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

		private List<Node> FindPathIterative(Node from, Node endNode)
		{
			Queue<Node> nodeQueue = new();
			Debug.Log($"{from.parentNode}, {endNode.parentNode}");

			nodeQueue.Enqueue(from);

			while (nodeQueue.Count > 0)
			{
				Node node = nodeQueue.Dequeue();

				if(from == endNode)
				{
					List<Node> endList = new();
					Node current = node;

					while (current != null && current != from)
					{
						endList.Add(current);
						current = current.parentNode;
					}

					if(current == null) { return null; }

					endList.Add(current);
					endList.Reverse();

					return endList;
				}

				List<Node> childNodes = node.connections.ToList();

				foreach (Node childNode in childNodes)
				{
					if(childNode.parentNode != null)
					{
						continue;
					}

					childNode.parentNode = node;
					nodeQueue.Enqueue(childNode);
				}
			}

			return null;
		}
	}
}