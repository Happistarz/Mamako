using Godot;
using System;
using System.Linq.Expressions;

public partial class SplashScene : Node2D
{
	private TextureRect _colorRect;

	public override void _Ready()
	{
		_colorRect = GetNode<TextureRect>("Canvas/TextureRect");
		Fade();
	}

	private void Fade()
	{
		var tween = CreateTween();
		tween.TweenInterval(1f);
		tween.TweenProperty(_colorRect, "modulate:a", 1, 1.5f);
		tween.TweenInterval(1.5f);
		tween.TweenProperty(_colorRect, "modulate:a", 0, 1.5f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
		tween.TweenInterval(0.5f);
		tween.Finished += ChangeScene;
	}

	private void ChangeScene()
	{
		GetTree().ChangeSceneToPacked(Refs.In.Main);
	}
}
