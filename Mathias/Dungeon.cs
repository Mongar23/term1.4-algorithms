using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Mathias.Utilities;
using Debug = Mathias.Utilities.Debug;

namespace Mathias
{
	public class Dungeon : DungeonBase
	{
		private readonly List<Point> blackListedPoints = new();

		private int minimumRoomSize;
		private List<RoomPair> roomPairs = new();

		public Dungeon(Size pSize) : base(pSize) { }

		protected override void generate(int minimumRoomSize)
		{
			Stopwatch stopwatch = new();
			stopwatch.Start();

			this.minimumRoomSize = minimumRoomSize;
			GenerateRooms();
			GenerateDoors();
			RemoveRooms();
			ColorRooms();
		}

		protected override void drawRooms(IEnumerable<Room> rooms, Pen wallColor, Brush fillColor = null)
		{
			foreach (Room room in rooms) { drawRoom(room, wallColor, new SolidBrush(room.Color)); }
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

				RoomPair roomPair = new(splitRooms.Item1, splitRooms.Item2);
				roomPairs.Add(roomPair);

				rooms.Add(roomPair.A);
				rooms.Add(roomPair.B);

				rooms.Remove(splittingRoom);
			}

			foreach (Room room in rooms)
			{
				foreach (Point corner in room.GetCorners())
				{
					if (blackListedPoints.Contains(corner)) { continue; }

					blackListedPoints.Add(corner);
				}
			}
		}


		/// <summary>
		///     Will split a <see cref="Room" /> horizontally or vertically based on the last split. And will return a
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


		/// <summary>
		///     Will remove all rooms that have either the biggest or smallest area and remove doors intersecting with it.
		/// </summary>
		private void RemoveRooms()
		{
			Room[] sortedRooms = rooms.OrderBy(r => r.Size.Area()).ToArray();
			int smallestArea = sortedRooms.First().Size.Area();
			int biggestArea = sortedRooms.Last().Size.Area();

			foreach (Room room in sortedRooms.Where(room => room.Size.Area() == biggestArea || room.Size.Area() == smallestArea))
			{
				rooms.Remove(room);

				Door[] toRemove = doors.Where(d => room.area.Contains(d.location)).ToArray();
				foreach (Door door in toRemove) { doors.Remove(door); }
			}
		}

		private void ColorRooms()
		{
			foreach (Room room in rooms)
			{
				int doorCount = room.GetDoorCount(doors);

				if (doorCount == 0) //Check if room is an island or just pairless.
				{
					foreach (Room r in rooms.Where(r => room.area.IntersectsWith(r.area)))
					{
						if (room.Equals(r)) { continue; }

						doors.Add(GenerateDoor(room, r));
						doorCount++;
						Debug.Log($"Saved room {room} from being left out!");
						break;
					}
				}

				room.Color = doorCount switch
				{
					0 => Color.Red,
					1 => Color.Orange,
					2 => Color.Yellow,
					>= 3 => Color.Green,
					_ => throw new ArgumentOutOfRangeException()
				};
			}
		}

		#region Door generation
		private void GenerateDoors()
		{
			foreach (RoomPair roomPair in roomPairs) { doors.Add(GenerateDoor(roomPair)); }
		}

		private Door GenerateDoor(Room a, Room b) { return GenerateDoor(new RoomPair(a, b)); }

		private Door GenerateDoor(RoomPair roomPair)
		{
			Door door;

			do
			{
				if (roomPair.IsOverlappingHorizontally)
				{
					int doorX = new Random().Next(roomPair.Overlap.X + 2, (roomPair.Overlap.X + roomPair.Overlap.Width) - 2);
					door = new Door(doorX, roomPair.Overlap.Y);
				}
				else
				{
					int doorY = new Random().Next(roomPair.Overlap.Y + 2, (roomPair.Overlap.Y + roomPair.Overlap.Height) - 2);
					door = new Door(roomPair.Overlap.X, doorY);
				}
			} while (blackListedPoints.Contains(door.location));

			door.SetRooms(roomPair.A, roomPair.B);
			return door;
		}
		#endregion
	}
}