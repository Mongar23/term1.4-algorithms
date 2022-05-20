using System.Collections.Generic;
using System.Drawing;

/**
 * This class represents (the data for) a Room, at this moment only a rectangle in the dungeon.
 */
public class Room
{
	public readonly List<Door> doors = new();
	public bool IsSplitHorizontally { get; set; } = true;
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
		position = area.Location;
		size = area.Size;
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

	public override string ToString()
	{
		return $"room(({Position.X}, {Position.Y}), {Size.Width}*{Size.Height}) \t\t doors{doors.Count}";
	}
}