using System;
using System.Drawing;
using GXPEngine;
using GXPEngine.OpenGL;
using Mathias;

/**
 * This is the main 'game' for the Algorithms Assignment that accompanies the Algorithms course.
 * 
 * Read carefully through the assignment that you are currently working on
 * and then through the code looking for all pointers & TODO's that you have to implement.
 * 
 * The course is 6 weeks long and this is the only assignment/code that you will get,
 * split into 3 major parts (see below). This means that you have three 2 week sprints to
 * work on your assignments.
 */
class AlgorithmsAssignment : Game
{
	public static Grid Grid { get; private set; }
	public static Random Random { get; private set; }

	//Required for assignment 1
	DungeonBase _dungeon = null;

	//Required for assignment 2
	NodeGraphBase _graph = null;
	TiledView _tiledView = null;
	NodeGraphAgentBase _agent = null;

	//Required for assignment 3
	PathFinderBase _pathFinder = null;

	//common settings
	private const int SCALE = 20;				//TODO: experiment with changing this
	private const int MIN_ROOM_SIZE = 5;		//TODO: use this setting in your dungeon generator

	public AlgorithmsAssignment() : base(800, 600, false, true, -1, -1, false)
	{
		GL.ClearColor(1, 1, 1, 1);
		GL.glfwSetWindowTitle("Algorithms Game");
		Random = new Random(1);
		
		Grid grid = new (width, height, SCALE);
		Grid = grid;
		Size size = new (width / SCALE, height / SCALE);

		_dungeon = new Dungeon(size);
		if (_dungeon != null)
		{
			_dungeon.scale = SCALE;
			_dungeon.Generate(MIN_ROOM_SIZE);
		}

		

		_graph = new NodeGraph(_dungeon);
		_graph?.Generate();

		_agent = new NodeGraphAgent(_graph);
		//_agent = new SampleNodeGraphAgent(_graph);
		//_agent = new OnGraphWayPointAgent(_graph);

		////////////////////////////////////////////////////////////
		//Assignment 2.2 Good (Optional) TiledView
		//
		//TODO: Study assignment 2.2 on blackboard
		//TODO: Study the TiledView and TileType classes
		//TODO: Study the SampleTileView class and try it out below
		//TODO: Comment out the SampleTiledView again, implement the TiledDungeonView and uncomment it below

		//_tiledView = new SampleTiledView(_dungeon, TileType.GROUND);
		//_tiledView = new TiledDungeonView(_dungeon, TileType.GROUND); 
		if (_tiledView != null) _tiledView.Generate();

		////////////////////////////////////////////////////////////
		//Assignment 2.2 Good (Optional) RandomWayPointAgent
		//
		//TODO: Comment out the OnGraphWayPointAgent above, implement a RandomWayPointAgent class and uncomment it below

		//_agent = new RandomWayPointAgent(_graph);	

		//////////////////////////////////////////////////////////////
		//Assignment 2.3 Excellent (Optional) LowLevelDungeonNodeGraph
		//
		//TODO: Comment out the HighLevelDungeonNodeGraph above, and implement the LowLevelDungeonNodeGraph 

		/////////////////////////////////////////////////////////////////////////////////////////
		/// ASSIGNMENT 3 : PathFinding and PathFindingAgents
		///							
		/// SKIP THIS BLOCK UNTIL YOU'VE FINISHED ASSIGNMENT 2 AND ASKED FOR TEACHER FEEDBACK !

		//////////////////////////////////////////////////////////////////////////
		//Assignment 3.1 Sufficient (Mandatory) - Recursive Pathfinding
		//
		//TODO: Study assignment 3.1 on blackboard
		//TODO: Study the PathFinder class
		//TODO: Study the SamplePathFinder class and try it out
		//TODO: Comment out the SamplePathFinder, implement a RecursivePathFinder and uncomment it below

		//_pathFinder = new SamplePathFinder(_graph);
		//_pathFinder = new RecursivePathFinder(_graph);

		//////////////////////////////////////////////////////////////////////////
		//Assignment 3.1 Sufficient (Mandatory) - BreadthFirst Pathfinding
		//
		//TODO: Comment out the RecursivePathFinder above, implement a BreadthFirstPathFinder and uncomment it below
		//_pathFinder = new BreadthFirstPathFinder(_graph);

		//TODO: Implement a PathFindingAgent that uses one of your pathfinder implementations (should work with any pathfinder implementation)
		//_agent = new PathFindingAgent(_graph, _pathFinder);

		/////////////////////////////////////////////////
		//Assignment 3.2 Good & 3.3 Excellent (Optional)
		//
		//There are no more explicit TODO's to guide you through these last two parts.
		//You are on your own. Good luck, make the best of it. Make sure your code is testable.
		//For example for A*, you must choose a setup in which it is possible to demonstrate your 
		//algorithm works. Find the best place to add your code, and don't forget to move the
		//PathFindingAgent below the creation of your PathFinder!

		//------------------------------------------------------------------------------------------
		/// REQUIRED BLOCK OF CODE TO ADD ALL OBJECTS YOU CREATED TO THE SCREEN IN THE CORRECT ORDER
		/// LOOK BUT DON'T TOUCH :)

		if (grid != null) AddChild(grid);
		if (_dungeon != null) AddChild(_dungeon);
		if (_graph != null) AddChild(_graph);
		if (_tiledView != null) AddChild(_tiledView);
		if (_pathFinder != null) AddChild(_pathFinder);				//pathfinder on top of that
		if (_graph != null) AddChild(new NodeLabelDrawer(_graph));	//node label display on top of that
		if (_agent != null) AddChild(_agent);                       //and last but not least the agent itself

		/////////////////////////////////////////////////
		//The end!
		////
	}
}


