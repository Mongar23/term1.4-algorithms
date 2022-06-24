using System.Collections.Generic;
using System.Drawing;

namespace Mathias
{
	public class NodeGraph : NodeGraphBase
	{
		protected DungeonBase dungeon;

		public NodeGraph(DungeonBase dungeon) : base((int)(dungeon.size.Width * dungeon.scale),
			(int)(dungeon.size.Height * dungeon.scale),
			(int)(dungeon.scale / 3f)) => this.dungeon = dungeon;


		protected override void generate()
		{
			Queue<Room> roomsToNode = new();
			List<Room> nodedRooms = new();
			Dictionary<Door, Node> nodedDoors = new();

			Room firstRoom = dungeon.rooms[0];

			roomsToNode.Enqueue(firstRoom);
			nodedRooms.Add(firstRoom);

			while (roomsToNode.Count > 0)
			{
				Room room = roomsToNode.Dequeue();

				Node roomNode = new(GetRoomCenter(room));
				nodes.Add(roomNode);

				foreach (Door door in room.doors)
				{
					Node doorNode;

					if (!nodedDoors.ContainsKey(door))
					{
						doorNode = new Node(GetPointCenter(door.location));

						nodedDoors.Add(door, doorNode);
						nodes.Add(doorNode);
					}
					else { doorNode = nodedDoors[door]; }

					AddConnection(roomNode, doorNode);

					if (!nodedRooms.Contains(door.RoomA))
					{
						roomsToNode.Enqueue(door.RoomA);
						nodedRooms.Add(door.RoomA);
					}

					if (nodedRooms.Contains(door.RoomB)) { continue; }

					roomsToNode.Enqueue(door.RoomB);
					nodedRooms.Add(door.RoomB);
				}
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