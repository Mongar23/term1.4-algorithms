using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Debug = Mathias.Utilities.Debug;

namespace Mathias
{
	public class NodeGraph : NodeGraphBase
	{
		public enum Level
		{
			High,
			Low
		}

		private readonly Level level;

		protected DungeonBase dungeon;

		public NodeGraph(DungeonBase dungeon, Level level) : base((int)(dungeon.size.Width * dungeon.scale),
			(int)(dungeon.size.Height * dungeon.scale),
			(int)(dungeon.scale / 3f))
		{
			this.dungeon = dungeon;
			this.level = level;
		}


		protected override void generate()
		{
			Stopwatch stopwatch = new();
			stopwatch.Start();

			switch (level)
			{
				case Level.High:
					GenerateHighLevel();
					break;

				case Level.Low:
					GenerateLowLevel();
					break;

				default: throw new ArgumentOutOfRangeException();
			}

			Debug.Initialized(this, stopwatch.ElapsedMilliseconds);
			stopwatch.Stop();
		}

		private void GenerateHighLevel()
		{
			Queue<Room> roomsToNode = new();
			List<Room> queuedRooms = new();
			Dictionary<Door, Node> nodedDoors = new();

			Room firstRoom = dungeon.rooms[0];
			roomsToNode.Enqueue(firstRoom);
			queuedRooms.Add(firstRoom);

			while (roomsToNode.Count > 0)
			{
				Room room = roomsToNode.Dequeue();

				Node roomNode = new(GetRoomCenter(room));
				Debug.Log($"Added {roomNode} for {room}");
				nodes.Add(roomNode);

				foreach (Door door in room.doors)
				{
					Node doorNode;

					if (!nodedDoors.ContainsKey(door))
					{
						doorNode = new Node(GetPointCenter(door.location));
						if (AlgorithmsAssignment.Instance.ExtensiveLogging)
						{
							Debug.Log($"Added {doorNode} for door({door.location})");
						}

						nodedDoors.Add(door, doorNode);
						nodes.Add(doorNode);
					}
					else { doorNode = nodedDoors[door]; }

					AddConnection(roomNode, doorNode);

					if (!queuedRooms.Contains(door.RoomA))
					{
						roomsToNode.Enqueue(door.RoomA);
						queuedRooms.Add(door.RoomA);
					}

					if (queuedRooms.Contains(door.RoomB)) { continue; }

					roomsToNode.Enqueue(door.RoomB);
					queuedRooms.Add(door.RoomB);
				}
			}
		}

		private void GenerateLowLevel()
		{
			int columns = dungeon.size.Width;
			int rows = dungeon.size.Height;


			Point[] doorPoints = dungeon.doors.Select(door => door.location).ToArray();
			Rectangle[] innerRooms = dungeon.rooms.Select(room => room.InnerArea).ToArray();

			for (int i = 0; i < rows * columns; i++)
			{
				Point point = new(i % columns, i / columns);

				if (!innerRooms.Any(innerRoom => innerRoom.Contains(point)) && !doorPoints.Contains(point)) { continue; }

				Node node = new(GetPointCenter(point));
				if (AlgorithmsAssignment.Instance.ExtensiveLogging) { Debug.Log($"Created {node} for ({point.X},{point.Y})"); }

				nodes.Add(node);

				foreach (Node neighborNode in GetNeighbors(node)) { AddConnection(node, neighborNode); }
			}

			IEnumerable<Node> GetNeighbors(Node node)
			{
				List<Node> neighborNodes = new();
				Point scaledPoint = node.GetScaledLocation(dungeon.scale);

				Node leftTop = nodes.Find(n =>
					n.GetScaledLocation(dungeon.scale).X == scaledPoint.X - 1 &&
					n.GetScaledLocation(dungeon.scale).Y == scaledPoint.Y - 1);
				if (leftTop != null) { neighborNodes.Add(leftTop); }

				Node left = nodes.Find(n =>
					n.GetScaledLocation(dungeon.scale).X == scaledPoint.X - 1 && n.location.Y == node.location.Y);
				if (left != null) { neighborNodes.Add(left); }

				Node leftBottom = nodes.Find(n =>
					n.GetScaledLocation(dungeon.scale).X == scaledPoint.X - 1 &&
					n.GetScaledLocation(dungeon.scale).Y == scaledPoint.Y + 1);
				if (leftBottom != null) { neighborNodes.Add(leftBottom); }

				Node top = nodes.Find(
					n => n.location.X == node.location.X && n.GetScaledLocation(dungeon.scale).Y == scaledPoint.Y + 1);
				if (top != null) { neighborNodes.Add(top); }

				Node bottom = nodes.Find(n =>
					n.location.X == node.location.X && n.GetScaledLocation(dungeon.scale).Y == scaledPoint.Y - 1);
				if (bottom != null) { neighborNodes.Add(bottom); }

				Node rightTop = nodes.Find(n =>
					n.GetScaledLocation(dungeon.scale).X == scaledPoint.X + 1 &&
					n.GetScaledLocation(dungeon.scale).Y == scaledPoint.Y - 1);
				if (rightTop != null) { neighborNodes.Add(rightTop); }

				Node right = nodes.Find(n =>
					n.GetScaledLocation(dungeon.scale).X == scaledPoint.X + 1 && n.location.Y == node.location.Y);
				if (right != null) { neighborNodes.Add(right); }

				Node rightBottom = nodes.Find(n =>
					n.GetScaledLocation(dungeon.scale).X == scaledPoint.X + 1 &&
					n.GetScaledLocation(dungeon.scale).Y == scaledPoint.Y + 1);
				if (rightBottom != null) { neighborNodes.Add(rightBottom); }

				return neighborNodes;
			}
		}

		private Point GetRoomCenter(Room room)
		{
			float centerX = (room.area.Left + room.area.Right) * 0.5f * dungeon.scale;
			float centerY = (room.area.Top + room.area.Bottom) * 0.5f * dungeon.scale;

			return new Point((int)centerX, (int)centerY);
		}

		private Point GetPointCenter(Point point)
		{
			float centerX = (point.X + 0.5f) * dungeon.scale;
			float centerY = (point.Y + 0.5f) * dungeon.scale;
			return new Point((int)centerX, (int)centerY);
		}
	}
}