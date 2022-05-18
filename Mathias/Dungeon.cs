using System;
using System.Collections.Generic;
using System.Drawing;
using GXPEngine;

namespace Mathias
{
	public class Dungeon : DungeonBase
	{
		private int minimumRoomSize;

		public Dungeon(Size pSize) : base(pSize) { }

		protected override void generate(int minimumRoomSize)
		{
			this.minimumRoomSize = minimumRoomSize;
			GenerateRooms();
		}

		private void GenerateRooms()
		{
			rooms.Add(new Room(0, 0, size.Width, size.Height));
			List<Room> unsplitableRooms = new();

			while (rooms.Count > unsplitableRooms.Count)
			{
				Room splittingRoom = rooms[new Random().Next(0, rooms.Count)];

				if (unsplitableRooms.Contains(splittingRoom)) { continue; }

				if (splittingRoom.Size.Width < minimumRoomSize * 2 && splittingRoom.Size.Height < minimumRoomSize * 2)
				{
					unsplitableRooms.Add(splittingRoom);
					continue;
				}

				Tuple<Room, Room> splitRooms = SplitRoom(splittingRoom);

				if (splitRooms.Item2 == null) { continue; } //Failed splitting.

				rooms.Add(splitRooms.Item1);
				rooms.Add(splitRooms.Item2);

				rooms.Remove(splittingRoom);
				unsplitableRooms.Clear();
			}
		}

		private Tuple<Room, Room> SplitRoom(Room baseRoom)
		{
			Room a = baseRoom;
			Room b = new(0, 0, 0, 0);

			bool horizontalSplit;

			if (baseRoom.Size.Height - minimumRoomSize < minimumRoomSize) { horizontalSplit = false; }
			else if (baseRoom.Size.Width - minimumRoomSize < minimumRoomSize) { horizontalSplit = true; }
			else { horizontalSplit = new Random().Next(0, 2) == 0; }


			if (horizontalSplit)
			{
				int cutSize = baseRoom.Size.Height - new Random().Next(minimumRoomSize, baseRoom.Size.Height - minimumRoomSize);

				a = new Room(baseRoom.Position.X, baseRoom.Position.Y, baseRoom.Size.Width, cutSize + 1);
				b = new Room(baseRoom.Position.X, baseRoom.Position.Y + cutSize, baseRoom.Size.Width, baseRoom.Size.Height - cutSize);
			}
			else
			{
				int cutSize = baseRoom.Size.Width - new Random().Next(minimumRoomSize, baseRoom.Size.Width - minimumRoomSize);

				a = new Room(baseRoom.Position.X, baseRoom.Position.Y, cutSize + 1, baseRoom.Size.Height);
				b = new Room(baseRoom.Position.X + cutSize, baseRoom.Position.Y, baseRoom.Size.Width - cutSize, baseRoom.Size.Height);
			}


			if (b.Size.Width < minimumRoomSize || b.Size.Height < minimumRoomSize) { return new Tuple<Room, Room>(baseRoom, null); }


			return new Tuple<Room, Room>(a, b);
		}
	}
}