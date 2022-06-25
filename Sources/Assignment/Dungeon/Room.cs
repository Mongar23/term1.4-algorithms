using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mathias.Utilities;

/**
 * This class represents (the data for) a Room, at this moment only a rectangle in the dungeon.
 */
public class Room
{
	public bool IsSplitHorizontally { get; set; } = true;
	public Color Color { get; set; }
	public Point Position { get; }

	/// <summary>
	/// The area of the room without the walls.
	/// </summary>
	public Rectangle InnerArea { get; }
	public Size Size { get; }
	public List<Door> doors;
	public Rectangle area;

	private static Rectangle CalculateInnerArea(Rectangle full)
	{
		Rectangle rectangle = full;
		rectangle.X += 1;
		rectangle.Width -= 2;
		rectangle.Y += 1;
		rectangle.Height -= 2;
		return rectangle;
	}

	public Point[] GetCorners()
	{
		return new[]
		{
			Position,
			new Point((Position.X + Size.Width) - 1, Position.Y),
			new Point((Position.X + Size.Width) - 1, (Position.Y + Size.Height) - 1),
			new Point(Position.X, (Position.Y + Size.Height) - 1)
		};
	}

	/// <summary>
	///     Get an <see cref="Room" /> <see langword="array" /> containing the current <see cref="Room" />'s neighbors. To
	///     determine the neighbors all of the <see cref="Room" />s in <paramref name="roomsToCheck" /> will be evaluated
	///     whether their area intersects with the current <see cref="Room" />'s area.
	/// </summary>
	/// <param name="roomsToCheck">The collection to search through to find intersecting rooms.</param>
	/// <returns>An array of <see cref="Room" />s which are intersecting with the current <see cref="Room" /></returns>
	public Room[] GetNeighbors(IEnumerable<Room> roomsToCheck)
	{
		return roomsToCheck.Where(room => !Equals(room) && area.IntersectsWith(room.area)).ToArray();
	}

	/// <summary>
	///     Returns the number of <see cref="Door" />s the room is intersecting with.
	/// </summary>
	/// <param name="doorsToCheck">In case the door list is not set, this will be the collection to set the rooms with.</param>
	/// <param name="forceUpdate">When true, the doors list will be set again.</param>
	/// <returns>Count of the doors lists.</returns>
	public int GetDoorCount(IEnumerable<Door> doorsToCheck, bool forceUpdate = false)
	{
		if (doors == null || forceUpdate) { SetDoors(doorsToCheck); }

		return doors.Count;
	}

	public override string ToString() => $"room(({Position.X}, {Position.Y}), {Size.Width}*{Size.Height})";

	public override bool Equals(object obj) => Equals(obj as Room);

	public override int GetHashCode() => base.GetHashCode();

	public void SetDoors(IEnumerable<Door> doorsToCheck)
	{
		List<Door> doors = new();

		foreach (Door door in doorsToCheck)
		{
			if (!area.Contains(door.location)) { continue; }

			if (door.RoomA == null)
			{
				door.RoomA = this;
				doors.Add(door);
				continue;
			}

			if (door.RoomB == null)
			{
				door.RoomB = this;
				doors.Add(door);
				continue;
			}

			Debug.LogWaring($"This door is already connecting two rooms: {door}");
		}

		this.doors = doors;
	}

	private bool Equals(Room other)
	{
		if (Position.X != other.Position.X || Position.Y != other.Position.Y) { return false; }

		return Size.Width == other.Size.Width && Size.Height == other.Size.Height;
	}

	#region Constuctors

	public Room(Rectangle pArea)
	{
		area = pArea;
		InnerArea = CalculateInnerArea(area);
		Position = area.Location;
		Size = area.Size;
	}

	public Room(Point position, Size size)
	{
		area = new Rectangle(position.X, position.Y, size.Width, size.Height);
		InnerArea = CalculateInnerArea(area);
		Position = area.Location;
		Size = area.Size;
	}

	public Room(int x, int y, int width, int height)
	{
		area = new Rectangle(x, y, width, height);
		InnerArea = CalculateInnerArea(area);
		Position = area.Location;
		Size = area.Size;
	}

	#endregion
}