using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class WorldTileMap : Node2D
{
	public static Vector2I NONE = new(-1, -1);

	public enum WorldLayers
	{
		Ground = 0,
		Base = 1,
		Portal = 2,
		Mouse = 3
	}

	public WorldLayers EditModeLayer { get; set; } = WorldLayers.Ground;
	public bool EditMode { get; set; } = false;

	public Vector2I? lastTilePosition;
	public Vector2I? initialTilePosition;

	public PortalStatsGUI portalStatsGUI;
	public EditModeGUI editModeGUI;
	public WorldResourcesGUI worldResourcesGUI;

	public Portal selectedPortal;
	public Portal lastSelectedPortal;

	public Vector2 AtlasIndex = NONE;
	public int SourceId = 1;
	public HashSet<Vector2I> previousPreviewTiles = new();

	public StateMachine<WorldTileMap> stateMachine;

	public TileMapLayer GroundLayer { get; set; }
	public TileMapLayer BaseLayer { get; set; }
	public TileMapLayer PortalLayer { get; set; }
	public TileMapLayer MouseLayer { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Global.Instance.tileMap = this;

		portalStatsGUI = GetNode<PortalStatsGUI>("../GUI/PortalStatsGUI");
		editModeGUI = GetNode<EditModeGUI>("../GUI/EditModeGUI");
		worldResourcesGUI = GetNode<WorldResourcesGUI>("../GUI/WorldResourcesGUI");

		GroundLayer = GetNode<TileMapLayer>("Ground");
		BaseLayer = GetNode<TileMapLayer>("Base");
		PortalLayer = GetNode<TileMapLayer>("Portal");
		MouseLayer = GetNode<TileMapLayer>("Mouse");

		selectedPortal = null;
		lastSelectedPortal = null;

		stateMachine = new(this);
		stateMachine.ChangeState(new NormalState(this));
	}

	public Vector2I MouseToMap(TileMapLayer layer)
	{
		Vector2 mousePosition = GetGlobalMousePosition();
		return layer.LocalToMap(mousePosition);
	}

	public override void _UnhandledInput(InputEvent @event)
	{

		stateMachine.UnhandledInput(@event);

	}

	public void ClearPreviousTiles()
	{
		foreach (Vector2I tilePos in previousPreviewTiles)
		{
			MouseLayer.EraseCell(tilePos);
		}
		previousPreviewTiles.Clear();
	}

	public TileMapLayer GetLayer(WorldLayers layer)
	{
		return layer switch
		{
			WorldLayers.Ground => GroundLayer,
			WorldLayers.Base => BaseLayer,
			WorldLayers.Portal => PortalLayer,
			WorldLayers.Mouse => MouseLayer,
			_ => null,
		};
	}


	public void CopyTile()
	{
		// get the tile atlas coords of the tile we are clicking on
		Vector2I tilePos = MouseToMap(MouseLayer);
		Vector2I tileId = GetLayer(EditModeLayer).GetCellAtlasCoords(tilePos);

		// if the tile is not empty, erase it
		AtlasIndex = tileId;
	}

	public void DrawRectangle(WorldLayers _layer = 0, bool erase = false)
	{
		if (!initialTilePosition.HasValue)
			return;

		var layer = GetLayer(_layer);
		Vector2I finalTilePosition = MouseToMap(layer);

		int x1 = Math.Min(initialTilePosition.Value.X, finalTilePosition.X);
		int x2 = Math.Max(initialTilePosition.Value.X, finalTilePosition.X);
		int y1 = Math.Min(initialTilePosition.Value.Y, finalTilePosition.Y);
		int y2 = Math.Max(initialTilePosition.Value.Y, finalTilePosition.Y);

		ClearPreviousTiles();

		for (int x = x1; x <= x2; x++)
		{
			for (int y = y1; y <= y2; y++)
			{
				Vector2I tilePos = new(x, y);

				if (erase)
					layer.EraseCell(tilePos);
				else if (AtlasIndex == NONE)
					layer.SetCell(tilePos, 2, new(1, 0));
				else
					layer.SetCell(tilePos, SourceId, (Vector2I)AtlasIndex);

				if (_layer == WorldLayers.Mouse)
					previousPreviewTiles.Add(new Vector2I(x, y));
			}
		}
	}

	public void MouseMovments()
	{
		Vector2I tilePos = MouseToMap(MouseLayer);

		// if the mouse is moving, clear the last tile
		if (lastTilePosition.HasValue && lastTilePosition.Value != tilePos)
		{
			// clear last tile
			MouseLayer.EraseCell(lastTilePosition.Value);
		}

		// set the tile on layer 3 to the selected tileSetIndex
		MouseLayer.SetCell(tilePos, 2, new(0, 0));


		// in edit mode, set the tile with the selected tileSetIndex or erase it
		if (EditMode)
		{
			if (AtlasIndex == NONE)
				MouseLayer.SetCell(tilePos, 2, new(1, 0));
			else
				MouseLayer.SetCell(tilePos, SourceId, (Vector2I)AtlasIndex);
		}
		else
			MouseLayer.SetCell(tilePos, 2, new(0, 0));

		lastTilePosition = tilePos;
	}

	public void MouseClicked()
	{
		// check if the cell on layer 2 is a portal
		Vector2I tilePos = MouseToMap(PortalLayer);
		Portal portal = GetPortalAtPosition(tilePos);

		if (portal != null)
			SelectPortal(portal);
		else
			DeselectCurrentPortal();
	}

	public void RemovePortal(Portal portal)
	{
		ErasePortalTile(portal);
		portalStatsGUI.Hide();
		selectedPortal = null;
		lastSelectedPortal = null;
	}

	private void SelectPortal(Portal portal)
	{
		if (selectedPortal == portal)
		{
			return; // Already selected, no need to update.
		}

		DeselectCurrentPortal(); // Deselect the last portal if needed.

		selectedPortal = portal;
		lastSelectedPortal = portal;

		// Update the tile to show it is selected.
		SetTileAtlasIndex((Vector2I)portal.TileMapIndex, 0, 2); // Example of tile for selected.

		// Show the portal stats GUI.
		portalStatsGUI.SetPortalStats(portal);
		portalStatsGUI.Show();
	}

	public void DeselectCurrentPortal()
	{
		if (selectedPortal != null)
		{
			// Update the tile to show it is deselected.
			SetTileAtlasIndex((Vector2I)selectedPortal.TileMapIndex, 0, 1); // Example of tile for unselected.

			selectedPortal = null;
			portalStatsGUI.Hide();
		}
	}

	private void ErasePortalTile(Portal portal)
	{
		PortalLayer.EraseCell((Vector2I)portal.TileMapIndex);
	}

	public static Portal GetPortalAtPosition(Vector2I tilePos)
	{
		return Global.Instance.portals.FirstOrDefault(p => p.TileMapIndex == tilePos);
	}

	private void SetTileAtlasIndex(Vector2I tilePos, int sourceId, int atlasIndex)
	{
		PortalLayer.SetCell(tilePos, sourceId, new Vector2I(0, atlasIndex));
	}

	public bool IsSelectingPortal(Portal portal)
	{
		return selectedPortal == portal;
	}

	public void ChangeBuildLayer(int layer, int sourceId)
	{

		// change the modulate color of the selected layer
		GetLayer(EditModeLayer).Modulate = new Color(1, 1, 1, 0.7f);

		EditModeLayer = (WorldLayers)layer;
		SourceId = sourceId;
	}

	public void ResetLayerColors()
	{
		foreach (EditLayer editLayer in editModeGUI.editLayers)
			GetLayer(editLayer.LayerId).Modulate = new Color(1, 1, 1, 1);
	}
}
