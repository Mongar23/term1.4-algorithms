using System;
using System.Drawing;

/**
 * This class represents (the data for) a Door, at this moment only a position in the dungeon.
 * Changes to this class might be required based on your specific implementation of the algorithm.
 */
public class Door
{
	public Point location { get; }

	public Room RoomA
	{
		get => roomA;
		set => roomA = value ?? throw new ArgumentException("Cannot set RoomA to null");
	}

	public Room RoomB
	{
		get => roomB;
		set => roomB = value ?? throw new ArgumentException("Cannot set RoomA to null");
	}

	//Keeping tracks of the Rooms that this door connects to,
	//might make your life easier during some of the assignments
	private Room roomA;
	private Room roomB;

	public Door(Point pLocation) => location = pLocation;

	public Door(int x, int y) => location = new Point(x, y);

	public override string ToString()
	{
		const int maxDoorInfoLength = 15;
		string doorInfo = $"door({location.X},{location.Y})";
		int whiteSpaceSize = maxDoorInfoLength - doorInfo.Length;
		string whiteSpace = new(' ', whiteSpaceSize);

		return $"{doorInfo} {whiteSpace} connecting rooms {roomA}\t{roomB}";
	}
}