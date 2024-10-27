using Godot;
using System;

public class NormalState : ObjectState<WorldTileMap>
{
  public NormalState(WorldTileMap objectState) : base(objectState)
  {
  }

  public override void UnhandledInput(InputEvent @event)
  {
    if (@event is InputEventMouseMotion)
      objectState.MouseMovments();

    if (Input.IsActionJustPressed("left_click"))
    {
      objectState.MouseClicked();
    }

    if (Input.IsActionJustPressed("edit_mode"))
    {
      objectState.stateMachine.ChangeState(new EditState(objectState));
    }
  }
}

public class EditState : ObjectState<WorldTileMap>
{
  public EditState(WorldTileMap objectState) : base(objectState)
  {
  }

  public override void Enter()
  {
    objectState.EditMode = true;
    objectState.editModeGUI.Visible = true;
    objectState.DeselectCurrentPortal();
    objectState.lastSelectedPortal = null;
  }

  private void HandleMouseMotion(InputEvent @event)
  {
    if (@event is not InputEventMouseMotion) return;

    objectState.MouseMovments();

    if (!Input.IsActionPressed("shift") || !objectState.initialTilePosition.HasValue) return;

    objectState.DrawRectangle(WorldTileMap.WorldLayers.Mouse, true); // erase the preview
    objectState.DrawRectangle(WorldTileMap.WorldLayers.Mouse);
  }

  private void HandleLeftClickInput()
  {
    if (Input.IsActionJustPressed("left_click"))
    {
      objectState.initialTilePosition = objectState.MouseToMap(objectState.GetLayer(objectState.EditModeLayer));
    }

    if (Input.IsActionJustReleased("left_click"))
    {
      if (objectState.initialTilePosition == objectState.MouseToMap(objectState.GetLayer(objectState.EditModeLayer)))
      {
        PlaceOrEraseTile();
      }

      if (Input.IsActionPressed("shift"))
      {
        HandlePreviewSubmition();
      }
      else
      {
        PlaceOrEraseTile();
        objectState.DrawRectangle(WorldTileMap.WorldLayers.Mouse, true); // erase the preview
      }

      objectState.initialTilePosition = null;
      objectState.previousPreviewTiles.Clear();
    }

    GD.Print("---");
  }

  private void HandleRightClickInput()
  {
    if (Input.IsActionJustPressed("right_click"))
    {
      if (Input.IsActionPressed("shift") && Input.IsActionPressed("left_click"))
      {
        // cancel the preview
        objectState.DrawRectangle(WorldTileMap.WorldLayers.Mouse, true);
        objectState.initialTilePosition = null;
        objectState.previousPreviewTiles.Clear();
      }
      else
        objectState.CopyTile();
    }
  }

  private void HandleEditModeInput()
  {
    if (Input.IsActionJustPressed("edit_mode"))
    {
      objectState.stateMachine.ChangeState(new NormalState(objectState));
    }
  }

  private void PlaceOrEraseTile()
  {
    if (objectState.AtlasIndex == WorldTileMap.NONE)
    {
      objectState.GetLayer(objectState.EditModeLayer).EraseCell(objectState.initialTilePosition.Value);
    }
    else
    {
      objectState.GetLayer(objectState.EditModeLayer).SetCell(objectState.initialTilePosition.Value, objectState.SourceId, (Vector2I)objectState.AtlasIndex);
    }
  }

  private void HandlePreviewSubmition()
  {
    objectState.DrawRectangle(WorldTileMap.WorldLayers.Mouse, true); // erase the preview

    if (objectState.AtlasIndex == WorldTileMap.NONE)
    {
      objectState.DrawRectangle(objectState.EditModeLayer, true); // erase the preview
    }
    else
    {
      objectState.DrawRectangle(objectState.EditModeLayer);
    }
  }

  public override void UnhandledInput(InputEvent @event)
  {
    HandleMouseMotion(@event);
    if (@event is InputEventMouseMotion) return;

    HandleLeftClickInput();
    HandleRightClickInput();
    HandleEditModeInput();
  }

  public override void Exit()
  {
    objectState.EditMode = false;
    objectState.editModeGUI.Visible = false;
    objectState.ResetLayerColors();
  }
}
