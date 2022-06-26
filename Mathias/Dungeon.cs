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
		private readonly GradeType gradeType;
		private readonly List<Point> blackListedPoints = new();
		private readonly List<RoomPair> roomPairs = new();
		private int minimumRoomSize;

		public Dungeon(Size pSize, GradeType gradeType) : base(pSize) => this.gradeType = gradeType;

		protected override void generate(int minimumRoomSize)
		{
			Stopwatch stopwatch = new();
			stopwatch.Start();

			this.minimumRoomSize = minimumRoomSize;

			switch (gradeType)
			{
				case GradeType.Sufficient:
					GenerateRooms();
					GenerateDoors();
					foreach (Room room in rooms) { room.SetDoors(doors); }

					break;

				case GradeType.Good:
					GenerateRooms();
					GenerateDoors();
					RemoveRooms();
					ColorRooms();
					break;

				case GradeType.Excellent: throw new NotImplementedException();
				default: throw new ArgumentOutOfRangeException();
			}

			int gridColumns = AlgorithmsAssignment.Instance.Grid.Columns;
			rooms = rooms.OrderBy(room => room.Position.X + (room.Position.Y * gridColumns)).ToList();

			Debug.Initialized(this, stopwatch.ElapsedMilliseconds);
			stopwatch.Stop();
		}

		protected override void drawRooms(IEnumerable<Room> rooms, Pen wallColor, Brush fillColor = null)
		{
			foreach (Room room in rooms) { drawRoom(room, wallColor, new SolidBrush(room.Color)); }
		}

		/// <summary>
		///     Creates a room the size of the entire dungeon, then it will start splitting this room into smaller rooms until the
		///     rooms are too small to split. This will be determined by the <see cref="minimumRoomSize" /> variable.
		/// </summary>
		private void GenerateRooms()
		{
			rooms.Add(new Room(0, 0, size.Width, size.Height)); //Create a room the size of the entire dungeon.
			List<Room> unsplitableRooms = new();

			while (rooms.Count > unsplitableRooms.Count) // There are still rooms to split.
			{
				int randomRoomNumber = AlgorithmsAssignment.Instance.Random.Next(0, rooms.Count);
				Room splittingRoom = rooms[randomRoomNumber]; // Get a random room to split.

				if (unsplitableRooms.Contains(splittingRoom)) { continue; } // If the room is unsplitable, skip this iteration.

				Tuple<Room, Room> splitRooms = SplitRoom(splittingRoom); //Split the room and save the result.

				if (splitRooms.Item2 == null) // Splitting failed.
				{
					if (AlgorithmsAssignment.Instance.ExtensiveLogging)
					{
						Debug.LogWaring($"Failed to split {splittingRoom}\tadding to unsplitable{unsplitableRooms.Count} collection.");
					}

					unsplitableRooms.Add(splittingRoom); // Add the failed split room to the unsplitable rooms list.
					continue; // Skip the iteration.
				}

				RoomPair roomPair = new(splitRooms.Item1, splitRooms.Item2);
				roomPairs.Add(roomPair); // Create a new room pair 

				rooms.Add(roomPair.A); // Add the newly created rooms to the rooms list.
				rooms.Add(roomPair.B);

				rooms.Remove(splittingRoom); // Remove the split room.

				if (AlgorithmsAssignment.Instance.ExtensiveLogging)
				{
					Debug.Log($"Split {splittingRoom}\tin to{roomPair.A}\tand {roomPair.B}");
				}
			}

			foreach (Point corner in rooms.SelectMany(room => room.GetCorners()))
			{
				blackListedPoints.Add(corner);
			} // add all the corners of the  
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

			if (baseRoom.IsSplitHorizontally) // Next split will be vertical.
			{
				//If the room width - the minimumRoomSize is smaller than the minimum room size it means the room can not be split any more. Return base room and null.
				if (baseRoom.Size.Width - minimumRoomSize < minimumRoomSize) { return new Tuple<Room, Room>(baseRoom, null); }

				// Get a random random number between the minimum room size and the base room's width - minimum room size.
				// Subtract this from the base room's width to get the cut size.
				int cutSize = baseRoom.Size.Width;
				cutSize -= AlgorithmsAssignment.Instance.Random.Next(minimumRoomSize, baseRoom.Size.Width - minimumRoomSize);

				// The first room will be on the base room's position with the same height but the cutSize + 1 as width to create an overlap.
				a = new Room(baseRoom.Position.X, baseRoom.Position.Y, cutSize + 1, baseRoom.Size.Height);
				// The second room will be on the base room's Y position but X + cutSize with the same height as the base room but the width is the base room's width - cut size.
				b = new Room(baseRoom.Position.X + cutSize, baseRoom.Position.Y, baseRoom.Size.Width - cutSize, baseRoom.Size.Height);

				a.IsSplitHorizontally = b.IsSplitHorizontally = false;
			}
			else // Next split will be horizontal.
			{
				//If the room height - the minimumRoomSize is smaller than the minimum room size it means the room can not be split any more. Return base room and null.
				if (baseRoom.Size.Height - minimumRoomSize < minimumRoomSize) { return new Tuple<Room, Room>(baseRoom, null); }

				// Get a random random number between the minimum room size and the base room's height - minimum room size.
				// Subtract this from the base room's height to get the cut size.
				int cutSize = baseRoom.Size.Height;
				cutSize -= AlgorithmsAssignment.Instance.Random.Next(minimumRoomSize, baseRoom.Size.Height - minimumRoomSize);

				// The first room will be on the base room's position with the same width but the cutSize + 1 as height to create an overlap.
				a = new Room(baseRoom.Position.X, baseRoom.Position.Y, baseRoom.Size.Width, cutSize + 1);
				// The second room will be on the base room's X position but Y + cutSize with the same width as the base room but the height is the base room's height - cut size.
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
			Room[] sortedRooms = rooms.OrderBy(r => r.Size.Area()).ToArray(); // Sort the rooms array based on their size.
			int smallestArea =
				sortedRooms.First().Size.Area(); // Get the smallest room's area by getting the area of the first room in the array.
			int biggestArea =
				sortedRooms.Last().Size.Area(); // Get the largest room's area by getting the area of the last room in the array. 

			// Loop through all rooms(in the sorted list to prevent errors) that have the same area as the smallest and largest room.
			foreach (Room room in sortedRooms.Where(room => room.Size.Area() == biggestArea || room.Size.Area() == smallestArea))
			{
				// Remove all of their corners from the blacklisted points collection.
				foreach (Point corner in room.GetCorners()) { blackListedPoints.Remove(corner); }

				// Get a collection from all of the door that are in the removed room area.
				// Remove these rooms from the doors collection.
				Door[] toRemove = doors.Where(d => room.area.Contains(d.location)).ToArray();
				foreach (Door door in toRemove) { doors.Remove(door); }

				// Remove the room from the rooms collection.
				// Add extra doors to the neighbors of the removed room.
				rooms.Remove(room);
				RestoreDoorsForRemovedRoom(room);
			}
		}

		/// <summary>
		///     Assigns colors based on a <see cref="Room" />'s <see cref="Door" /> count. In case a <see cref="Room" /> has 0
		///     <see cref="Door" />s, an extra check will be done to make sure it doesn't connect to anything, if it does a
		///     <see cref="Door" /> will be generated for these <see cref="Room" />s.
		/// </summary>
		/// <exception cref="ArgumentOutOfRangeException">
		///     Will be thrown if the <see cref="Room" />'s <see cref="Door" /> count
		///     will return negative.
		/// </exception>
		private void ColorRooms()
		{
			foreach (Room room in rooms)
			{
				int doorCount = room.GetDoorCount(doors);

				if (doorCount == 0) //Check if room is an island or just pair-less.
				{
					foreach (Room neighbor in room.GetNeighbors(rooms)) // Loop trough all of the room's neighbors
					{
						Door door = GenerateDoor(room, neighbor); // Try to generate a door for the room and the neighbor.

						if (door == null) { continue; } // Skip the iteration if the door cannot be placed.

						// Add the door to the doors collection and increase the door count. Then break out of this loop.
						doors.Add(door);
						doorCount++;
						break;
					}
				}

				// Determine the room's color based on the room's door count.
				room.Color = doorCount switch
				{
					0 => Color.Red,
					1 => Color.Orange,
					2 => Color.FromArgb(252, 227, 0),
					>= 3 => Color.FromArgb(0, 252, 60),
					_ => throw new ArgumentOutOfRangeException()
				};
			}
		}

		#region Door generation

		/// <summary>
		///     Generate doors for all <see cref="RoomPair" />s in the roomPair collection.
		/// </summary>
		private void GenerateDoors()
		{
			foreach (RoomPair roomPair in roomPairs)
			{
				Door door = GenerateDoor(roomPair);

				if (door == null) { continue; }

				doors.Add(door);
			}
		}

		/// <summary>
		///     Calls <see cref="GenerateDoor(RoomPair)" /> with a new temporary <see cref="RoomPair" /> created for
		///     <paramref name="a" /> and <paramref name="b" />.
		/// </summary>
		/// <param name="a">
		///     <see cref="Room" /> to generate a <see cref="Door" /> for, to connect it with <paramref name="b" />
		/// </param>
		/// <param name="b">
		///     <see cref="Room" /> to generate a <see cref="Door" /> for, to connect it with <paramref name="a" />
		/// </param>
		/// <returns>
		///     A newly created door to connect <see cref="Room" />s <paramref name="a" /> and <paramref name="b" /> or
		///     <see langword="null" />
		///     the door cannot be placed.
		/// </returns>
		private Door GenerateDoor(Room a, Room b) => GenerateDoor(new RoomPair(a, b));

		/// <summary>
		///     Creates a new door to connect the <see cref="Room" />s in the <see cref="RoomPair" />. It will place an door
		///     somewhere in the overlapping area. In case this newly placed <see cref="Door" />'s location is in a blacklisted
		///     spot, it will try again to place the door in a new location.
		/// </summary>
		/// <param name="roomPair">The <see cref="RoomPair" /> to connect with the newly created door.</param>
		/// <returns>
		///     A newly created door to connect the rooms <see cref="Room" />s in <paramref name="roomPair" /> or
		///     <see langword="null" /> the door cannot be placed.
		/// </returns>
		private Door GenerateDoor(RoomPair roomPair)
		{
			Door door;

			do
			{
				if (roomPair.IsOverlappingHorizontally)
				{
					// Add 2 to the overlap x, to prevent the doors to generate in the corners.
					int min = roomPair.Overlap.X + 2;
					// Add the overlap width onto the overlap x and subtract 2, to prevent the doors to generate in the corners. 
					int max = (roomPair.Overlap.X + roomPair.Overlap.Width) - 2;

					if (min > max) { return null; } // In case the min is bigger than max no door could be placed.

					// Get a random point on the x-axis to place the door.
					int doorX = AlgorithmsAssignment.Instance.Random.Next(min, max);
					// Create a new door on the overlaps' Y position and the random X position.
					door = new Door(doorX, roomPair.Overlap.Y);
				}
				else
				{
					// Add 2 to the overlap y, to prevent the doors to generate in the corners.
					int min = roomPair.Overlap.Y + 2;
					// Add the overlap height onto the overlap y and subtract 2, to prevent the doors to generate in the corners. 
					int max = (roomPair.Overlap.Y + roomPair.Overlap.Height) - 2;

					if (min > max) { return null; } // In case the min is bigger than max no door could be placed.

					// Get a random point on the y-axis to place the door.
					int doorY = AlgorithmsAssignment.Instance.Random.Next(min, max);
					// Create a new door on the overlaps' X position and the random Y position.
					door = new Door(roomPair.Overlap.X, doorY);
				}
			} while (blackListedPoints.Contains(door.location)); // Continue to do this until the door is not in a black listed spot anymore.

			return door;
		}

		/// <summary>
		///     Add extra <see cref="Door" />s to the neighbors of the removed <see cref="Room" /> and their neighbors.
		/// </summary>
		/// <param name="removedRoom">The room that has been removed and to get the first pair of neighbors from.</param>
		private void RestoreDoorsForRemovedRoom(Room removedRoom)
		{
			foreach (Room neighbor in removedRoom.GetNeighbors(rooms))
			{
				foreach (Room farNeighbor in neighbor.GetNeighbors(rooms))
				{
					Rectangle overLap = Rectangle.Intersect(neighbor.area, farNeighbor.area);

					if (overLap.IsEmpty) { continue; }

					// Already contains a door in the overlap.
					if (doors.Any(door => overLap.Contains(door.location))) { continue; }

					Door door = GenerateDoor(neighbor, farNeighbor);
					if (door == null) { continue; }

					doors.Add(door);
					break;
				}
			}
		}

		#endregion
	}
}