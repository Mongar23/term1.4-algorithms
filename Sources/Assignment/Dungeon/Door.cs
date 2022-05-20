using System.Drawing;

/**
 * This class represents (the data for) a Door, at this moment only a position in the dungeon.
 * Changes to this class might be required based on your specific implementation of the algorithm.
 */
public class Door
{
	public Point location { get; private set; }

	//Keeping tracks of the Rooms that this door connects to,
	//might make your life easier during some of the assignments
	public Room roomA { get; private set; }
	public Room roomB { get; private set; }

	//You can also keep track of additional information such as whether the door connects horizontally/vertically
	//Again, whether you need flags like this depends on how you implement the algorithm, maybe you need other flags
	public bool horizontal = false;

	public Door(Point pLocation) { location = pLocation; }

	public Door(int x, int y) { location = new Point(x, y); }

	public void Move(Point point) { location = point; }

	public void SetRooms(Room a, Room b)
	{
		roomA = a;
		roomB = b;
	}

	public override string ToString() { return $"door({location.X},{location.Y})\t\tconnecting rooms {roomA}, {roomB}"; }
}