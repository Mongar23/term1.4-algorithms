using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Mathias.Utilities;

namespace Mathias
{
	public class TiledDungeonView : TiledViewBase
	{
		private readonly DungeonBase dungeon;
		private Point[] doorPoints;

		public TiledDungeonView(DungeonBase dungeon, TileType pDefaultTileType) : base(dungeon.size.Width,
			dungeon.size.Height,
			(int)dungeon.scale,
			pDefaultTileType)
		{
			this.dungeon = dungeon;
		}

		protected override void generate()
		{
			System.Diagnostics.Stopwatch stopwatch = new();
			stopwatch.Start();

			doorPoints = dungeon.doors.Select(dungeonDoor => dungeonDoor.location).ToArray();

			for (int i = 0; i < rows * columns; i++)
			{
				Point point = new(i % columns, i / columns);
				TileType tileType = TileType.VOID;

				if(dungeon.rooms.Any(dungeonRoom => dungeonRoom.area.Contains(point)))
				{
					tileType = TileType.WALL;

					if(dungeon.rooms.Any(room => room.InnerArea.Contains(point)) || doorPoints.Contains(point))
					{
						tileType = TileType.GROUND;
					}
				}

				SetTileType(point.X, point.Y, tileType);
			}

			Debug.Initialized(this, stopwatch.ElapsedMilliseconds);
			stopwatch.Stop();
		}
	}
}