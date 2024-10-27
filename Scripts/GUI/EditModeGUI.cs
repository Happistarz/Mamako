using Godot;
using System;
using System.Collections.Generic;

public struct EditLayer
{
	public Texture2D AtlasTexture;
	public Vector2I TileSetSize;
	public ItemList TileList;
	public int TileSetAtlasId;
	public WorldTileMap.WorldLayers LayerId;
}

public partial class EditModeGUI : Control
{

	[Export] public int TileSize { get; set; } = 64;

	public List<EditLayer> editLayers = new();
	private int currentLayer = 0;

	[Export] public OptionButton layerDropdown;
	[Export] public Label costLabel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		editLayers.Add(new EditLayer
		{
			AtlasTexture = Refs.In.TileSet01Texture,
			TileSetSize = new(12, 4),
			TileSetAtlasId = 1,
			LayerId = WorldTileMap.WorldLayers.Ground,
			TileList = GetNode<ItemList>("Ground")
		});
		editLayers.Add(new EditLayer
		{
			AtlasTexture = Refs.In.PropsTexture,
			TileSetSize = new(3, 1),
			TileSetAtlasId = 3,
			LayerId = WorldTileMap.WorldLayers.Base,
			TileList = GetNode<ItemList>("Props")
		});

		editLayers.ForEach(layer =>
		{
			PopulateList(layer);
		});
	}

	public void PopulateList(EditLayer editLayer)
	{
		for (int y = 0; y < editLayer.TileSetSize.Y; y++)
		{
			for (int x = 0; x < editLayer.TileSetSize.X; x++)
			{
				AtlasTexture texture = new()
				{
					Atlas = editLayer.AtlasTexture,
					Region = new Rect2(x * TileSize, y * TileSize, TileSize, TileSize)
				};
				editLayer.TileList.AddItem(null, texture);
			}
		}
	}

	private void ChangeItemList(int index)
	{
		foreach (EditLayer editLayer in editLayers)
		{
			editLayer.TileList.Visible = false;
		}

		editLayers[index].TileList.Visible = true;
	}

	public override void _GuiInput(InputEvent @event)
	{
		AcceptEvent();
	}

	public void OnItemListItemSelected(int index)
	{
		EditLayer editLayer = editLayers[currentLayer];
		Vector2 atlasTile = new(index % editLayer.TileSetSize.X, index / editLayer.TileSetSize.X);
		Global.Instance.tileMap.AtlasIndex = atlasTile;
	}

	public void OnLayerDropdownItemSelected(int index)
	{
		Global.Instance.tileMap.ChangeBuildLayer(index, layerDropdown.GetItemId(index));
		currentLayer = index;
		ChangeItemList(index);
	}
}
