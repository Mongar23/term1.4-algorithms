using GXPEngine;

/**
 * Very simple example of a nodegraphagent that walks directly to the node you clicked on,
 * ignoring walls, connections etc.
 */
class SampleNodeGraphAgent : NodeGraphAgent
{
	//Current target to move towards
	private Node _target = null;

	public SampleNodeGraphAgent(NodeGraphBase pNodeGraphBase) : base(pNodeGraphBase)
	{
		SetOrigin(width / 2, height / 2);

		//position ourselves on a random node
		if (pNodeGraphBase.nodes.Count > 0)
		{
			jumpToNode(pNodeGraphBase.nodes[Utils.Random(0, pNodeGraphBase.nodes.Count)]);
		}

		//listen to nodeclicks
		pNodeGraphBase.OnNodeLeftClicked += onNodeClickHandler;
	}

	protected virtual void onNodeClickHandler(Node pNode)
	{
		_target = pNode;
	}

	protected override void Update()
	{
		//no target? Don't walk
		if (_target == null) return;

		//Move towards the target node, if we reached it, clear the target
		if (moveTowardsNode(_target))
		{
			_target = null;
		}
	}
}
