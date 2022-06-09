using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mathias.Utilities;

namespace Mathias
{
	public class TiledDungeonView : TiledViewBase
	{
		private readonly DungeonBase dungeon;
		private List<Point> doorPoints = new();
		private List<Rectangle> shrunkRooms = new();

		public TiledDungeonView(DungeonBase dungeon, TileType pDefaultTileType) : base(dungeon.size.Width,
			dungeon.size.Height,
			(int)dungeon.scale,
			pDefaultTileType) => this.dungeon = dungeon;

		protected override void generate()
		{
			doorPoints = dungeon.doors.Select(dungeonDoor => dungeonDoor.location).ToList();
			shrunkRooms = dungeon.rooms.Select(dungeonRoom => ShrinkRoom(dungeonRoom.area)).ToList();

			for (int i = 0; i < rows * columns; i++)
			{
				Point point = new(i % columns, i / columns);
				TileType tileType = TileType.VOID;

				if(dungeon.rooms.Any(dungeonRoom => dungeonRoom.area.Contains(point)))
				{
					tileType = TileType.WALL;

					if(shrunkRooms.Any(shrunkRoom => shrunkRoom.Contains(point)) || doorPoints.Contains(point))
					{
						tileType = TileType.GROUND;
					}
				}

				SetTileType(point.X, point.Y, tileType);
			}
		}

		private Rectangle ShrinkRoom(Rectangle roomArea)
		{
			Rectangle rectangle = roomArea;
			rectangle.X += 1;
			rectangle.Width -= 2;
			rectangle.Y += 1;
			rectangle.Height -= 2;
			return rectangle;
		}
	}
}