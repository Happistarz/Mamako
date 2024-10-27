using Godot;
using System;

public partial class Main : Node2D
{

	private Node2D _world;
	private Node2D _ticketMap;

	// current node2d
	public Node2D _currentNode;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		_world = GetNode<Node2D>("World");
		_ticketMap = GetNode<Node2D>("TicketOfficeWorld");

		_currentNode = _ticketMap;
		ChangeScene(_world);
	}

	public void ChangeScene(Node2D node)
	{
		Disable(_currentNode);

		_currentNode = node;
		Enable(_currentNode);
	}

	private void Enable(Node2D node)
	{
		node.Visible = true;
		node.SetProcess(true);
		node.SetPhysicsProcess(true);
		node.SetProcessInput(true);
	}

	private void Disable(Node2D node)
	{
		if (node == null) return;

		node.Visible = false;
		node.SetProcess(false);
		node.SetPhysicsProcess(false);
		node.SetProcessInput(false);
	}
}
