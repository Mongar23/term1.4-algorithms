using System.Drawing;

/**
 * This class represents (the data for) a Room, at this moment only a rectangle in the dungeon.
 */
public class Room
{
	public Point Position { get; }
	public Size Size { get; }
	public bool IsSplitHorizontally { get; set; } = true;
	public Rectangle area;

	public Room(Rectangle pArea)
	{
		area = pArea;
		Position = area.Location;
		Size = area.Size;
	}

	//TODO: Implement a toString method for debugging?
	//Return information about the type of object and it's data
	//eg Room: (x, y, width, height)

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
}