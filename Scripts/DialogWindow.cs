using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

struct Dialog
{
	public string Speaker { get; set; }
	public string Text { get; set; }
}
public partial class DialogWindow : CanvasLayer
{
	private List<Dialog> dialogues;

	[Export]
	public string DialogNumber { get; set; } = "01";

	private Label speakerLabel;
	private Label textLabel;
	private ProgressBar progressBar;
	private Timer charTimer;
	private TextureRect clientTexture;

	private int currentDialog = 0;
	private string targetText = "";

	private float currentProgress = 0;

	private int currentLetter = 0;

	private float charSpeed = 0.03f; // seconds per character

	private Tween tween;

	private enum DialogState
	{
		Idle,
		Writing,
		Finished
	}

	private DialogState dialogState = DialogState.Idle;

	ClientProperties clientProperties;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		speakerLabel = GetNode<Label>("Panel/Speaker");
		textLabel = GetNode<Label>("Panel/Text");
		progressBar = GetNode<ProgressBar>("Panel/Progress");
		charTimer = GetNode<Timer>("CharTimer");
		clientTexture = GetNode<TextureRect>("ClientTexture");

		clientProperties = ClientFactory.GenerateClientProperties<Pablo>();
		SetClientColors();
	}

	public override void _Process(double delta)
	{
		if (dialogState == DialogState.Writing)
		{
			if (currentLetter >= targetText.Length)
			{
				dialogState = DialogState.Idle;
				charTimer.Stop();
			}
		}
		else if (dialogState == DialogState.Finished)
		{
			QueueFree();
		}
	}

	#region Dialogues
	public void LoadDialog()
	{
		dialogues = new List<Dialog>();

		string path = ProjectSettings.GlobalizePath("res://Resources//Dialogs.json");
		string json = File.ReadAllText(path);
		Json jsonDialog = new();
		Error error = jsonDialog.Parse(json);
		if (error != Error.Ok)
		{
			GD.PrintErr("Error parsing JSON");
			return;
		}
		Dictionary parsed = (Dictionary)jsonDialog.Data;

		foreach (string nb in parsed.Keys.Select(v => (string)v))
		{
			Dictionary dialog = (Dictionary)parsed[nb];
			dialogues.Add(new Dialog()
			{
				Speaker = clientProperties.Name.ToUpper(),
				Text = (string)dialog["Message"]
			});
		}

		currentDialog = 0;
		currentLetter = 0;
		currentProgress = 0;
		targetText = dialogues[currentDialog].Text;
		speakerLabel.Text = dialogues[currentDialog].Speaker;
		dialogState = DialogState.Writing;
		UpdateProgressBar();
		charTimer.Start();
	}


	private void UpdateDialog()
	{
		textLabel.Text = targetText[..currentLetter];
	}

	private void UpdateProgressBar()
	{
		currentProgress = ((float)currentDialog + 1) / dialogues.Count * 100;

		if (currentDialog == dialogues.Count - 1)
		{
			StyleBoxFlat box = progressBar.GetThemeStylebox("fill").Duplicate() as StyleBoxFlat;
			box.BgColor = new Color(0.875f, 0.361f, 0.196f);
			progressBar.AddThemeStyleboxOverride("fill", box);
		}

		tween = CreateTween();
		tween.TweenProperty(progressBar, "value", currentProgress, 1f).SetEase(Tween.EaseType.Out);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey eventKey && eventKey.Pressed && !eventKey.Echo)
		{
			if (eventKey.IsActionPressed("dialog"))
			{
				if (dialogState == DialogState.Writing)
				{
					currentLetter = targetText.Length;
					UpdateDialog();
					dialogState = DialogState.Idle;
				}
				else if (dialogState == DialogState.Idle)
				{
					currentDialog++;
					if (currentDialog < dialogues.Count)
					{
						targetText = dialogues[currentDialog].Text;
						speakerLabel.Text = dialogues[currentDialog].Speaker;
						dialogState = DialogState.Writing;
						currentLetter = 0;
						UpdateProgressBar();
						charTimer.Start();
					}
					else
					{
						dialogState = DialogState.Finished;
					}
				}
			}

			if (eventKey.IsActionPressed("start"))
			{
				LoadDialog();
				Fade(true);
			}
		}
	}

	private void OnCharTimerTimeout()
	{
		if (currentLetter < targetText.Length)
		{
			currentLetter++;
			UpdateDialog();

			char currentChar = targetText[currentLetter - 1];
			if (".,!?".Contains(currentChar))
			{
				charTimer.WaitTime = charSpeed * 2.5;
			}
			else
			{
				charTimer.WaitTime = charSpeed;
			}
		}
		else
		{
			charTimer.Stop();
		}
	}

	private void Fade(bool _in)
	{
		tween = CreateTween();
		tween.TweenProperty(GetNode("Panel"), "modulate:a", _in ? 1f : 0, 1f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
		tween.TweenProperty(clientTexture, "modulate:a", _in ? 1f : 0, 1f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut).SetDelay(0.5f);
		tween.Parallel();
	}
	#endregion

	#region Client
	public void SetClientProperties(ClientProperties properties)
	{
		clientProperties = properties;

		SetClientColors();
	}

	private void SetClientColors()
	{
		if (clientProperties is not IColorableClient colorableClient) return;

		Color[] colors = colorableClient.GenerateColorArray();
		ShaderMaterial shader = (ShaderMaterial)clientTexture.Material.Duplicate();
		shader.SetShaderParameter("color_to", colors);
		clientTexture.Material = shader;
	}
	#endregion
}
