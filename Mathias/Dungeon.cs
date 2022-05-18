using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GXPEngine;
using Mathias.Utilities;

namespace Mathias
{
	public class Dungeon : DungeonBase
	{
		private int minimumRoomSize;

		public Dungeon(Size pSize) : base(pSize) { }

		protected override void generate(int minimumRoomSize)
		{
			this.minimumRoomSize = minimumRoomSize;
			int maxRooms = Mathf.Floor(size.Width / minimumRoomSize) * Mathf.Floor(size.Height / minimumRoomSize);

			rooms.AddRange(GenerateRooms(maxRooms + 1)); // +1 because it is exclusive
		}

		private List<Room> GenerateRooms(int n)
		{
			List<Room> tempRooms = new();
			tempRooms.Add(new Room(0, 0, size.Width, size.Height));

			while (tempRooms.Count < n)
			{
				tempRooms = tempRooms.OrderByDescending(r => r.Size.Area()).ToList();

				Room biggest = tempRooms[0];

				// Room is to small to split exit loop.
				if (biggest.Size.Width <= minimumRoomSize * 2 && biggest.Size.Height <= minimumRoomSize * 2) { return tempRooms; }


				Tuple<Room, Room> splitRooms = SplitRoom(biggest);
				tempRooms.Add(splitRooms.Item1);
				tempRooms.Add(splitRooms.Item2);

				tempRooms.Remove(biggest);
			}


			return tempRooms;
		}

		private Tuple<Room, Room> SplitRoom(Room baseRoom)
		{
			Room a = baseRoom;
			Room b = new(0, 0, 0, 0);

			while (a.Size.Width < minimumRoomSize ||
			       a.Size.Height < minimumRoomSize ||
			       b.Size.Width < minimumRoomSize ||
			       b.Size.Height < minimumRoomSize)
			{
				bool horizontalSplit;

				if (baseRoom.Size.Height - minimumRoomSize < minimumRoomSize) { horizontalSplit = false; }
				else if (baseRoom.Size.Width - minimumRoomSize < minimumRoomSize) { horizontalSplit = true; }
				else { horizontalSplit = new Random().Next(0, 2) == 0; }


				if (horizontalSplit)
				{
					int cutSize = baseRoom.Size.Height - new Random(Time.deltaTime).Next(minimumRoomSize, baseRoom.Size.Height - minimumRoomSize);

					a = new Room(baseRoom.Position.X, baseRoom.Position.Y, baseRoom.Size.Width, cutSize + 1);
					b = new Room(baseRoom.Position.X,
						baseRoom.Position.Y + cutSize,
						baseRoom.Size.Width,
						baseRoom.Size.Height - cutSize);
				}
				else
				{
					int cutSize = baseRoom.Size.Width - new Random(Time.time).Next(minimumRoomSize, baseRoom.Size.Width - minimumRoomSize);

					a = new Room(baseRoom.Position.X, baseRoom.Position.Y, cutSize + 1, baseRoom.Size.Height);
					b = new Room(baseRoom.Position.X + cutSize,
						baseRoom.Position.Y,
						baseRoom.Size.Width - cutSize,
						baseRoom.Size.Height);
				}
			}


			return new Tuple<Room, Room>(a, b);
		}
	}
}