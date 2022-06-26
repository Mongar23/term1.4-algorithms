using System;
using System.Drawing;
using GXPEngine;
using GXPEngine.OpenGL;
using Mathias;
using Mathias.Agents;
using Mathias.Utilities;

internal class AlgorithmsAssignment : Game
{
	private const int MIN_ROOM_SIZE = 5;
	private const int SCALE = 40;

	//Required for assignment 1
	private readonly DungeonBase _dungeon;
	private readonly NodeGraphAgentBase _agent;

	//Required for assignment 2
	private readonly NodeGraphBase _graph;

	//Required for assignment 3
	private readonly PathFinderBase _pathFinder;
	private readonly TiledViewBase _tiledView;

	public static AlgorithmsAssignment Instance { get; private set; }
	public bool ExtensiveLogging { get; private set; }
	public Grid Grid { get; }
	public Random Random { get; }


	public AlgorithmsAssignment(int windowWidth, int windowHeight) : base(windowWidth, windowHeight, false)
	{
		Instance = game as AlgorithmsAssignment;

		GL.ClearColor(1, 1, 1, 1);
		GL.glfwSetWindowTitle("Algorithms Game");
		Random = new Random();

		Grid = new Grid(width, height, SCALE);
		Size size = new(width / SCALE, height / SCALE);
		ExtensiveLogging = true;

		#region part 1

		_dungeon = new Dungeon(size, GradeType.Good);
		if (_dungeon != null)
		{
			_dungeon.scale = SCALE;
			_dungeon.Generate(MIN_ROOM_SIZE);
		}

		#endregion

		#region part 2

		_graph = new NodeGraph(_dungeon, NodeGraph.Level.High);
		_graph?.Generate();

		_agent = new NodeGraphAgent(_graph, GradeType.Sufficient);

		if (((NodeGraphAgent)_agent).gradeType == GradeType.Good)
		{
			_tiledView = new TiledDungeonView(_dungeon, TileType.GROUND);
			_tiledView?.Generate();
		}

		#endregion

		#region part 3

		_pathFinder = new PathFinder(_graph, PathFinder.SearchType.BFS);

		_agent = new PathFindingAgent(_graph, _pathFinder);

		#endregion

		#region Add all as children

		if(Grid != null){ AddChild(Grid);}

		if (_dungeon != null) { AddChild(_dungeon); }

		if (_tiledView != null) { AddChild(_tiledView); }

		if (_graph != null) { AddChild(_graph); }

		if (_pathFinder != null) { AddChild(_pathFinder); }

		if (_graph != null) { AddChild(new NodeLabelDrawer(_graph)); }

		if (_agent != null) { AddChild(_agent); }

		#endregion
	}
}