using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Mathias
{
	public class Dungeon : DungeonBase
	{
		private readonly List<Point> blackListedPoints = new();
		private int minimumRoomSize;

		public Dungeon(Size pSize) : base(pSize) { }

		protected override void generate(int minimumRoomSize)
		{
			this.minimumRoomSize = minimumRoomSize;
			GenerateRooms();
			CleanDoors();
		}

		/// <summary>
		///     Will split main room up until all rooms are to small to split.
		/// </summary>
		private void GenerateRooms()
		{
			rooms.Add(new Room(0, 0, size.Width, size.Height));
			List<Room> unsplitableRooms = new();

			while (rooms.Count > unsplitableRooms.Count)
			{
				Room splittingRoom = rooms[new Random().Next(0, rooms.Count)];

				if (unsplitableRooms.Contains(splittingRoom)) { continue; }


				Tuple<Room, Room> splitRooms = SplitRoom(splittingRoom);

				if (splitRooms.Item2 == null) // Splitting failed.
				{
					unsplitableRooms.Add(splittingRoom);
					continue;
				}


				rooms.Add(splitRooms.Item1);
				rooms.Add(splitRooms.Item2);

				GenerateDoor(splitRooms.Item1, splitRooms.Item2);

				rooms.Remove(splittingRoom);
			}
			
			foreach (Room room in rooms) { blackListedPoints.AddRange(room.GetCorners()); }
		}


		/// <summary>
		///     This method will split a <see cref="Room" /> horizontally or vertically based on the last split. And will return a
		///     tuple with the two new rooms generated on the <paramref name="baseRoom" />. If the room is too small to be split,
		///     it will return the <paramref name="baseRoom" /> and <see langword="null" />.
		/// </summary>
		/// <param name="baseRoom">The <see cref="Room" /> where the two new room will be based on.</param>
		/// <returns>
		///     If split succeeded, two newly creates rooms. Else <paramref name="baseRoom" /> and <see langword="null" />.
		/// </returns>
		private Tuple<Room, Room> SplitRoom(Room baseRoom)
		{
			Room a;
			Room b;

			if (baseRoom.IsSplitHorizontally)
			{
				if (baseRoom.Size.Width - minimumRoomSize < minimumRoomSize) { return new Tuple<Room, Room>(baseRoom, null); }


				int cutSize = baseRoom.Size.Width - new Random().Next(minimumRoomSize, baseRoom.Size.Width - minimumRoomSize);

				a = new Room(baseRoom.Position.X, baseRoom.Position.Y, cutSize + 1, baseRoom.Size.Height);
				b = new Room(baseRoom.Position.X + cutSize, baseRoom.Position.Y, baseRoom.Size.Width - cutSize, baseRoom.Size.Height);

				a.IsSplitHorizontally = b.IsSplitHorizontally = false;
			}
			else
			{
				if (baseRoom.Size.Height - minimumRoomSize < minimumRoomSize) { return new Tuple<Room, Room>(baseRoom, null); }


				int cutSize = baseRoom.Size.Height - new Random().Next(minimumRoomSize, baseRoom.Size.Height - minimumRoomSize);

				a = new Room(baseRoom.Position.X, baseRoom.Position.Y, baseRoom.Size.Width, cutSize + 1);
				b = new Room(baseRoom.Position.X, baseRoom.Position.Y + cutSize, baseRoom.Size.Width, baseRoom.Size.Height - cutSize);

				a.IsSplitHorizontally = b.IsSplitHorizontally = true;
			}


			return new Tuple<Room, Room>(a, b);
		}

		private void GenerateDoor(Room a, Room b)
		{
			Door door = a.IsSplitHorizontally
				? new Door(new Random().Next(a.Position.X + 2, (a.Position.X + a.Size.Width) - 2), b.Position.Y)
				: new Door(b.Position.X, new Random().Next(a.Position.Y + 2, (a.Position.Y + a.Size.Height) - 2));

			a.doors.Add(door);
			b.doors.Add(door);

			door.SetRooms(a, b);

			doors.Add(door);
		}

		private void CleanDoors()
		{
			Door[] doorsToMove = doors.Where(door => blackListedPoints.Contains(door.location)).ToArray();

			foreach (Door door in doorsToMove)
			{
				while (blackListedPoints.Contains(door.location))
				{
					Point newDoorLocation;

					if (door.roomA.IsSplitHorizontally)
					{
						int x = new Random().Next(door.roomA.Position.X + 2, (door.roomA.Position.X + door.roomA.Size.Width) - 2);
						newDoorLocation = new Point(x, door.roomB.Position.Y);
					}
					else
					{
						int y = new Random().Next(door.roomA.Position.Y + 2, (door.roomA.Position.Y + door.roomA.Size.Height) - 2);
						newDoorLocation = new Point(door.roomB.Position.X, y);
					}


					door.Move(newDoorLocation);
				}
			}
		}
	}
}