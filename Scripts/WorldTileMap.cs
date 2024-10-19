using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
// TODO:
// - quand on click droit sur aucune tile, ca passe en mode erase
// - faire le rectangle de preview quand on shift click
// - corriger le bug du portal qui se remet en mode normal quand on le selectionne
// - corriger le bug du portal qui ne se remet pas en mode normal quand on le deselectionne

// do a shortcut for Vector2(-1,-1)

public partial class WorldTileMap : TileMap
{
	public static Vector2I NONE = new(-1, -1);

	public const int PREVIEW_LAYER = 3;
	public const int PORTAL_LAYER = 2;

	public int EditModeLayer { get; set; } = 0;
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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		portalStatsGUI = GetNode<PortalStatsGUI>("../GUI/PortalStatsGUI");
		editModeGUI = GetNode<EditModeGUI>("../GUI/EditModeGUI");
		worldResourcesGUI = GetNode<WorldResourcesGUI>("../GUI/WorldResourcesGUI");

		selectedPortal = null;
		lastSelectedPortal = null;

		stateMachine = new(this);
		stateMachine.ChangeState(new NormalState(this));
	}

	public Vector2I MouseToMap()
	{
		Vector2 mousePosition = GetGlobalMousePosition();
		return LocalToMap(mousePosition);
	}

	public override void _UnhandledInput(InputEvent @event)
	{

		stateMachine.UnhandledInput(@event);

	}

	public void ClearPreviousTiles()
	{
		foreach (Vector2I tilePos in previousPreviewTiles)
		{
			EraseCell(PREVIEW_LAYER, tilePos);
		}
		previousPreviewTiles.Clear();
	}


	public void CopyTile()
	{
		// get the tile atlas coords of the tile we are clicking on
		Vector2I tilePos = MouseToMap();
		Vector2I tileId = GetCellAtlasCoords(EditModeLayer, tilePos);

		// if the tile is not empty, erase it
		AtlasIndex = tileId;
	}

	public void DrawRectangle(int layer = 0, bool erase = false)
	{
		if (!initialTilePosition.HasValue)
			return;

		Vector2I finalTilePosition = MouseToMap();

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
					EraseCell(layer, tilePos);
				else if (AtlasIndex == NONE)
					SetCell(PREVIEW_LAYER, tilePos, 2, new(1, 0));
				else
					SetCell(layer, tilePos, SourceId, (Vector2I)AtlasIndex);

				if (layer == PREVIEW_LAYER)
					previousPreviewTiles.Add(new Vector2I(x, y));
			}
		}
	}

	public void MouseMovments()
	{
		Vector2I tilePos = MouseToMap();

		// if the mouse is moving, clear the last tile
		if (lastTilePosition.HasValue && lastTilePosition.Value != tilePos)
		{
			// clear last tile
			EraseCell(PREVIEW_LAYER, lastTilePosition.Value);
		}

		// set the tile on layer 3 to the selected tileSetIndex
		SetCell(PREVIEW_LAYER, tilePos, 2, new(0, 0));


		// in edit mode, set the tile with the selected tileSetIndex or erase it
		if (EditMode)
		{
			if (AtlasIndex == NONE)
				SetCell(PREVIEW_LAYER, tilePos, 2, new(1, 0));
			else
				SetCell(PREVIEW_LAYER, tilePos, SourceId, (Vector2I)AtlasIndex);
		}
		else
			SetCell(PREVIEW_LAYER, tilePos, 2, new(0, 0));

		lastTilePosition = tilePos;
	}

	public void MouseClicked()
	{
		// check if the cell on layer 2 is a portal
		Vector2I tilePos = MouseToMap();
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
		EraseCell(PORTAL_LAYER, (Vector2I)portal.TileMapIndex);
	}

	public static Portal GetPortalAtPosition(Vector2I tilePos)
	{
		return Global.Instance.portals.FirstOrDefault(p => p.TileMapIndex == tilePos);
	}

	private void SetTileAtlasIndex(Vector2I tilePos, int sourceId, int atlasIndex)
	{
		SetCell(PORTAL_LAYER, tilePos, sourceId, new Vector2I(0, atlasIndex));
	}

	public bool IsSelectingPortal(Portal portal)
	{
		return selectedPortal == portal;
	}

	public void ChangeBuildLayer(int layer, int sourceId)
	{

		// change the modulate color of the selected layer
		SetLayerModulate(EditModeLayer, new Color(1, 1, 1, 0.7f));

		EditModeLayer = layer;
		SourceId = sourceId;
	}

	public void ResetLayerColors()
	{
		foreach (EditLayer editLayer in editModeGUI.editLayers)
			SetLayerModulate(editLayer.LayerId, new Color(1, 1, 1, 1));
	}
}
