using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public partial class Global : Node
{
	public static Global Instance;

	public List<Portal> portals = new();

	public WorldTileMap tileMap;

	public int Money { get; set; } = 1000;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;

		tileMap = GetTree().Root.GetNode<WorldTileMap>("Main/World/TileMap");

		// SpawnClient();
	}

	public Portal SpawnPortal(Vector2 tileMapIndex, Vector2 position)
	{
		Portal portal = new()
		{
			Name = $"Portal {portals.Count + 1}",
			Type = (PortalType)GD.RandRange(0, Enum.GetValues(typeof(PortalType)).Length - 1),
			TileMapIndex = tileMapIndex,
			Position = position,
			Price = GD.Randf() * 1000
		};

		portals.Add(portal);
		return portal;
	}

	public void RemovePortal(Portal portal)
	{
		tileMap.RemovePortal(portal);
		portals.Remove(portal);
	}

	// private void SpawnShip()
	// {
	// 	Ship ship = ClientFactory.CreateShip();
	// 	GetTree().Root.GetNode("Main/World").AddChild(ship);
	// 	ship.GlobalPosition = new(32, 32);
	// 	ship.SetTargetPortal(portals[GD.RandRange(0, portals.Count - 1)]);
	// }

	// private void SpawnClient()
	// {
	// 	Client client = ClientFactory.CreateClient(GetTree().Root.GetNode("Main/World"));
	// 	GetTree().Root.GetNode("Main/World").AddChild(client);
	// 	client.GlobalPosition = new(32, 32);
	// 	client.TargetPortal = portals[GD.RandRange(0, portals.Count - 1)];
	// }
}
