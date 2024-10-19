using Godot;
using System;

public partial class PlayerController : Node2D
{

	[Export] public Rect2 GameBounds = new Rect2(0, 0, 1920, 1080);

	[ExportGroup("Camera")]
	[Export] public float cameraSpeed = 5f;
	[Export] public float cameraSensitivity = 20f;
	[Export] public float cameraMaxZoom = 2f;
	[Export] public float cameraMinZoom = 0.5f;

	private Camera2D camera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		camera = GetNode<Camera2D>("Camera");

		// Set the mouse in the middle of the screen
		Input.MouseMode = Input.MouseModeEnum.Confined;
		GetViewport().WarpMouse(GetViewportRect().Size / 2);
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		Vector2 cameraZoom = camera.Zoom;
		if (Input.IsActionJustPressed("zoom_in"))
		{
			cameraZoom.X = Mathf.Clamp(cameraZoom.X + 0.1f, cameraMinZoom, cameraMaxZoom);
			cameraZoom.Y = cameraZoom.X;
		}
		else if (Input.IsActionJustPressed("zoom_out"))
		{
			cameraZoom.X = Mathf.Clamp(cameraZoom.X - 0.1f, cameraMinZoom, cameraMaxZoom);
			cameraZoom.Y = cameraZoom.X;
		}

		camera.Zoom = cameraZoom;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (Input.IsActionJustPressed("quit"))
		{
			GetTree().Quit();
		}

		MoveCamera();
	}

	private void MoveCamera()
	{
		Vector2 mousePos = GetViewport().GetMousePosition();
		Vector2 viewportSize = GetViewportRect().Size;

		// Calculate the difference between the center of the screen and the mouse position
		float leftThreshold = cameraSensitivity;
		float rightThreshold = viewportSize.X - cameraSensitivity;
		float topThreshold = cameraSensitivity;
		float bottomThreshold = viewportSize.Y - cameraSensitivity;

		bool nearLeft = mousePos.X < leftThreshold;
		bool nearRight = mousePos.X > rightThreshold;
		bool nearTop = mousePos.Y < topThreshold;
		bool nearBottom = mousePos.Y > bottomThreshold;

		Vector2 direction = new();

		if (nearLeft)
		{
			direction.X = -1;
		}
		else if (nearRight)
		{
			direction.X = 1;
		}

		if (nearTop)
		{
			direction.Y = -1;
		}
		else if (nearBottom)
		{
			direction.Y = 1;
		}

		Vector2 newPos = Position + direction * cameraSpeed;

		// Clamp the camera position to the game bounds
		newPos.X = Mathf.Clamp(newPos.X, GameBounds.Position.X, GameBounds.End.X - viewportSize.X);
		newPos.Y = Mathf.Clamp(newPos.Y, GameBounds.Position.Y, GameBounds.End.Y - viewportSize.Y);

		Position = newPos;
	}
}
