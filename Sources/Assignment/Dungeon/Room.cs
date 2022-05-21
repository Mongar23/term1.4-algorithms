using System.Collections.Generic;
using System.Drawing;

/**
 * This class represents (the data for) a Room, at this moment only a rectangle in the dungeon.
 */
public class Room
{
	public bool IsSplitHorizontally { get; set; } = true;
	public Color Color { get; set; }
	public Point Position { get; }
	public Size Size { get; }

	public Rectangle area;

	public Room(Rectangle pArea)
	{
		area = pArea;
		Position = area.Location;
		Size = area.Size;
	}

	public Room(Point position, Size size)
	{
		area = new Rectangle(position.X, position.Y, size.Width, size.Height);
		Position = area.Location;
		Size = area.Size;
	}

	public Room(int x, int y, int width, int height)
	{
		area = new Rectangle(x, y, width, height);
		Position = area.Location;
		Size = area.Size;
	}

	public Point[] GetCorners()
	{
		return new[]
		{
			Position,
			new Point(Position.X + Size.Width, Position.Y),
			new Point(Position.X + Size.Width, Position.Y + Size.Height),
			new Point(Position.X, Position.Y + Size.Height)
		};
	}

	public int GetDoorCount(IEnumerable<Door> doorsToCheck)
	{
		int count = 0;

		foreach (Door door in doorsToCheck)
		{
			if (door.location.X < Position.X || door.location.X > Position.X + Size.Width) { continue; }

			if (door.location.Y < Position.Y || door.location.Y > Position.Y + Size.Height) { continue; }

			count++;
		}

		return count;
	}

	public override string ToString() { return $"room(({Position.X}, {Position.Y}), {Size.Width}*{Size.Height})"; }

	//TODO: Cnhage to equalto

	public override bool Equals(object obj) { return Equals(obj as Room); }

	private bool Equals(Room other)
	{
		if (Position.X != other.Position.X || Position.Y != other.Position.Y) { return false; }

		return Size.Width == other.Size.Width && Size.Height == other.Size.Height;
	}
}