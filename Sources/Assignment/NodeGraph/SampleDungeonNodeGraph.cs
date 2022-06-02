using System.Diagnostics;
using System.Drawing;

/**
 * An example of a dungeon nodegraph implementation.
 * 
 * This implementation places only three nodes and only works with the SampleDungeon.
 * Your implementation has to do better :).
 * 
 * It is recommended to subclass this class instead of NodeGraph so that you already 
 * have access to the helper methods such as getRoomCenter etc.
 * 
 * TODO:
 * - Create a subclass of this class, and override the generate method, see the generate method below for an example.
 */
class SampleDungeonNodeGraph : NodeGraphBase
{
	protected DungeonBase dungeonBase;

	public SampleDungeonNodeGraph(DungeonBase pDungeonBase) : base((int)(pDungeonBase.size.Width * pDungeonBase.scale), (int)(pDungeonBase.size.Height * pDungeonBase.scale), (int)pDungeonBase.scale/3)
	{
		Debug.Assert(pDungeonBase != null, "Please pass in a dungeon.");

		dungeonBase = pDungeonBase;
	}

	protected override void generate ()
	{
		//Generate nodes, in this sample node graph we just add to nodes manually
		//of course in a REAL nodegraph (read:yours), node placement should 
		//be based on the rooms in the dungeon

		//We assume (bad programming practice 1-o-1) there are two rooms in the given dungeon.
		//The getRoomCenter is a convenience method to calculate the screen space center of a room
		nodes.Add(new Node(getRoomCenter(dungeonBase.rooms[0])));
		nodes.Add(new Node(getRoomCenter(dungeonBase.rooms[1])));
		//The getDoorCenter is a convenience method to calculate the screen space center of a door
		nodes.Add(new Node(getDoorCenter(dungeonBase.doors[0])));

		//create a connection between the two rooms and the door...
		AddConnection(nodes[0], nodes[2]);
		AddConnection(nodes[1], nodes[2]);
	}

	/**
	 * A helper method for your convenience so you don't have to meddle with coordinate transformations.
	 * @return the location of the center of the given room you can use for your nodes in this class
	 */
	protected Point getRoomCenter(Room pRoom)
	{
		float centerX = ((pRoom.area.Left + pRoom.area.Right) / 2.0f) * dungeonBase.scale;
		float centerY = ((pRoom.area.Top + pRoom.area.Bottom) / 2.0f) * dungeonBase.scale;
		return new Point((int)centerX, (int)centerY);
	}

	/**
	 * A helper method for your convenience so you don't have to meddle with coordinate transformations.
	 * @return the location of the center of the given door you can use for your nodes in this class
	 */
	protected Point getDoorCenter(Door pDoor)
	{
		return getPointCenter(pDoor.location);
	}

	/**
	 * A helper method for your convenience so you don't have to meddle with coordinate transformations.
	 * @return the location of the center of the given point you can use for your nodes in this class
	 */
	protected Point getPointCenter(Point pLocation)
	{
		float centerX = (pLocation.X + 0.5f) * dungeonBase.scale;
		float centerY = (pLocation.Y + 0.5f) * dungeonBase.scale;
		return new Point((int)centerX, (int)centerY);
	}
}
