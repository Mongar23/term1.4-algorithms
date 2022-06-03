﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Mathias
{
	public class PathFinder : PathFinderBase
	{
		private List<Node> path = new();

		public PathFinder(NodeGraphBase pGraphBase) : base(pGraphBase) { }

		protected override List<Node> generate(Node from, Node to)
		{
			return FindPath(from, to, new List<Node> { from }) ? path : null;
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