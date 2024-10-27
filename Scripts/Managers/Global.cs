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
}
