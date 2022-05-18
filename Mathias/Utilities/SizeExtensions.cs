using System.Drawing;

namespace Mathias.Utilities
{
	public static class SizeExtensions
	{
		public static int Area(this Size source) { return source.Width * source.Height; }
	}
}