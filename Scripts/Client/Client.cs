using Godot;
using Godot.NativeInterop;
using System;

public partial class Client : Node2D
{

	public PathFollow2D ParentPath;

	public ClientProperties Properties;

	public Label NameLabel;

	// public Ship Ship;
	public AnimatedSprite2D Ship;

	private Tween tween;

	public Portal TargetPortal;
	public Timer PortalUseTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		// Properties = new()
		// {
		// 	Name = "John",
		// 	Speed = 1.0f,
		// 	DesiredDestination = PortalType.SpaceMarket,
		// 	ShipSize = 1.0f,
		// 	ShipColor = new Color(0.5f, 0.5f, 0.5f)
		// };

		NameLabel = GetNode<Label>("NameLabel");
		Ship = GetNode<AnimatedSprite2D>("AnimatedShip");
		PortalUseTimer = GetNode<Timer>("PortalUseTimer");

		ShipColorShader();

		NameLabel.Text = Properties.Name;
		Ship.Modulate = Properties.ShipColor;
		Ship.Scale = new(Properties.ShipSize, Properties.ShipSize);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (TargetPortal == null) return;

		RotateShip((float)delta);
		MoveShip((float)delta);
	}

	private void ShipColorShader()
	{
		// get current shader
		Color[] colors = new Color[4] {
			Properties.ShipColor+new Color(-0.21f, -0.17f, -0.04f),
		 	Properties.ShipColor+new Color(-0.09f, 0.08f, -0.02f),
			Properties.ShipColor,
			Properties.ShipColor+new Color(0.06f, 0.05f, 0.01f)
		};

		ShaderMaterial shaderMaterial = (ShaderMaterial)Ship.Material.Duplicate();
		shaderMaterial.SetShaderParameter("color_to", colors);

		Ship.Material = shaderMaterial;
	}

	public void MoveTo(Vector2 position)
	{
		tween = CreateTween();
		tween.TweenProperty(this, "position", position, 1.0f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
	}
	public void MoveTo(float duration)
	{
		tween = CreateTween();
		tween.TweenProperty(ParentPath, "progress_ratio", 1.0f, duration - duration / Properties.Speed).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
	}

	private void MoveShip(float delta)
	{

		// lerp to target portal
		Vector2 currentPosition = GlobalPosition;
		Vector2 targetPosition = TargetPortal.Position;
		Vector2 velocity = (targetPosition - currentPosition).Normalized() * Properties.Speed;

		if (currentPosition.DistanceTo(targetPosition) < velocity.Length())
		{
			// reached target
			GlobalPosition = targetPosition;
			PortalUseTimer.Start();

			// play teleport animation if not selecting portal
			if (!Global.Instance.tileMap.IsSelectingPortal(TargetPortal))
				Global.Instance.tileMap.GetLayer(WorldTileMap.WorldLayers.Portal).SetCell((Vector2I)TargetPortal.TileMapIndex, 0, new(0, 0));
		}
		else
		{
			// move towards target
			GlobalPosition += velocity;
		}
	}

	private void RotateShip(float delta)
	{
		// lerp the rotation
		float currentRotation = Rotation;
		float targetRotation = (TargetPortal.Position - GlobalPosition).Angle();
		float rotation = Mathf.LerpAngle(currentRotation, targetRotation, 0.1f);
		Rotation = rotation;
	}

	public void UsePortal()
	{
		Color color = Modulate;
		color.A = Mathf.Lerp(color.A, 0, 0.1f);
		Modulate = color;
	}
	public void Fade(bool fadeIn)
	{
		tween = CreateTween();
		float targetAlpha = fadeIn ? 1.0f : 0.0f;
		tween.TweenProperty(this, "modulate:a", targetAlpha, 1.0f).SetTrans(Tween.TransitionType.Expo).SetEase(Tween.EaseType.Out);

		if (!fadeIn)
		{
			tween.TweenCallback(Callable.From(() => QueueFree()));
		}
	}

	public void OnPortalUseTimeout()
	{
		PortalUseTimer.Stop();

		if (!Global.Instance.tileMap.IsSelectingPortal(TargetPortal))
			Global.Instance.tileMap.GetLayer(WorldTileMap.WorldLayers.Portal).SetCell((Vector2I)TargetPortal.TileMapIndex, 0, new(0, 1));

		TargetPortal = null;

		QueueFree();
	}
}
