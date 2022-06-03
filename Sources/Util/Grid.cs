using GXPEngine;
using System.Drawing;

/**
 * A grid helper class.
 */
class Grid : Canvas
{
	public int Rows { get; }
	public int Columns { get; }

	public Grid(int pWidth, int pHeight, int pGridSize) : base(pWidth, pHeight)
	{
		int columns = width / pGridSize;
		int rows = height / pGridSize;

		Rows = rows;
		Columns = columns;

		for (int i = 0; i <= columns; i++)
		{
			graphics.DrawLine(Pens.Black, i * pGridSize, 0, i * pGridSize, height);
		}

		for (int j = 0; j <= rows; j++)
		{
			graphics.DrawLine(Pens.Black, 0, j * pGridSize, width, j * pGridSize);
		}

		alpha = 0.1f;
	}
}

