using System.Drawing;
using Mathias.Utilities;

namespace Mathias
{
	public class RoomPair
	{
		public Room A { get; }
		public Room B { get; }
		public Rectangle Overlap { get; }
		public bool IsOverlappingHorizontally { get; }

		public int doorCount = 0; 

		public RoomPair(Room a, Room b)
		{
			if (a == b)
			{
				Debug.LogWaring("Trying to link the same room");
				return;
			}

			if (a == null)
			{
				Debug.LogWaring("Trying to link a room to null. a is null");
				return;
			}

			if (b == null)
			{
				Debug.LogWaring("Trying to link a room to null. a is null");
				return;
			}

			Overlap = Rectangle.Intersect(a.area, b.area);
			IsOverlappingHorizontally = Overlap.Width > Overlap.Height;

			A = a;
			B = b;

		}

		public bool Contains(Room room) { return room.Equals(A) || room.Equals(B); }
	}
}