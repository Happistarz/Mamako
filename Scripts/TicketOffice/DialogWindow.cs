using Godot;
using Godot.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

struct Dialog
{
	public string Speaker { get; set; }
	public string Text { get; set; }

	public override string ToString()
	{
		return $"{Speaker}: {Text}";
	}
}
public partial class DialogWindow : CanvasLayer
{
	private List<Dialog> dialogues;

	[Export]
	public string DialogNumber { get; set; } = "01";

	private Label speakerLabel;
	private RichTextLabel textLabel;
	private ProgressBar progressBar;
	private TextureRect clientTexture;
	private AnimationPlayer animPlayer;
	private ColorRect arrow;

	private int currentDialog = 0;
	private string targetText = "";

	private float currentProgress = 0;

	private int currentLetter = 0;

	private const float charactersPerSecond = 5f;
	private const float minTypingLength = 1f;
	private const float maxTypingLength = 5f;

	private Tween tween;

	private enum DialogState
	{
		Idle,
		Writing,
		Finished
	}

	private DialogState dialogState = DialogState.Idle;

	private ClientProperties clientProperties;

	private PortalModel portalModel;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		speakerLabel = GetNode<Label>("Panel/Speaker");
		textLabel = GetNode<RichTextLabel>("Panel/Text");
		progressBar = GetNode<ProgressBar>("Panel/Progress");
		clientTexture = GetNode<TextureRect>("ClientTexture");

		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		arrow = GetNode<ColorRect>("Panel/Arrow");
		arrow.Visible = false;

		SetClientProperties(ClientFactory.GenerateClientProperties<Pablo>());
	}

	#region Dialogues
	public void LoadDialog()
	{
		dialogues = new List<Dialog>();

		string path = ProjectSettings.GlobalizePath($"{Refs.In.DialogsJsonPath}Dialogs{clientProperties.Id}.json");
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
			string message = (string)dialog["Message"];

			message = ReplacePlaceholders(message);

			dialogues.Add(new Dialog()
			{
				Speaker = clientProperties.Name.ToUpper(),
				Text = message
			});
		}

		currentDialog = 0;
		InitializeDialog();
	}

	private void InitializeDialog()
	{
		currentProgress = 0;
		UpdateProgressBar();

		if (dialogues.Count > 0)
		{
			DisplayDialog();
		}
		else
		{
			GD.PrintErr("No dialogues found");
		}
	}

	private void DisplayDialog()
	{
		targetText = dialogues[currentDialog].Text;
		speakerLabel.Text = dialogues[currentDialog].Speaker;

		StartTyping();
	}

	private void StartTyping()
	{
		dialogState = DialogState.Writing;

		float animationLength = GetVisibleCharacterCount(targetText) / charactersPerSecond;

		animPlayer.GetAnimation("typing").Length = Mathf.Clamp(animationLength, minTypingLength, maxTypingLength);

		textLabel.VisibleCharacters = 0;
		textLabel.Text = targetText;

		animPlayer.Play("typing");
		arrow.Visible = false;
	}

	private string ReplacePlaceholders(string message)
	{
		message = message.Replace("%WHAT%", clientProperties.Product.Name);
		message = message.Replace("%PORTAL%", portalModel.Name);
		message = message.Replace("%PREFIX%", portalModel.Prefix);
		message = message.Replace("%WHAT_PREFIX%", clientProperties.Product.Prefix);
		message = message.Replace("%WHAT_ICON%", clientProperties.Product.Icon);
		message = message.Replace("%PORTAL_ICON%", GameDataManager.Instance.GetCategory(portalModel.Category).Icon);

		return ReplaceUnicodeTags(message);
	}

	private static string ReplaceUnicodeTags(string text)
	{
		string pattern = @"\[char=(.*?)\]";
		return Regex.Replace(text, pattern, match =>
		{
			string unicode = match.Groups[1].Value;
			return char.ConvertFromUtf32(int.Parse(unicode, System.Globalization.NumberStyles.HexNumber));
		});
	}

	private static int GetVisibleCharacterCount(string text)
	{
		var visibleText = Regex.Replace(text, @"\[(.*?)\]", ""); // Retirer toutes les balises BBCode
		return visibleText.Length;
	}

	private void UpdateProgressBar()
	{
		currentProgress = ((float)currentDialog + 1) / dialogues.Count * 100;

		if (currentDialog == dialogues.Count - 1)
		{
			StyleBoxFlat box = progressBar.GetThemeStylebox("fill").Duplicate() as StyleBoxFlat;
			box.BgColor = Refs.In.ButtonColor;
			progressBar.AddThemeStyleboxOverride("fill", box);
		}

		progressBar.Value = currentProgress;
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey eventKey && eventKey.IsActionPressed("dialog") && !eventKey.Echo)
		{

			if (dialogState == DialogState.Writing)
			{
				animPlayer.Stop();
				textLabel.VisibleCharacters = targetText.Length;
				dialogState = DialogState.Idle;

				arrow.Visible = true;
				animPlayer.Play("arrow");
			}
			else if (dialogState == DialogState.Idle)
			{
				currentDialog++;
				if (currentDialog < dialogues.Count)
				{
					DisplayDialog();
					UpdateProgressBar();

					arrow.Visible = false;
				}
				else
				{
					dialogState = DialogState.Finished;
					Fade(false);
				}
			}
			else if (dialogState == DialogState.Finished)
			{
				Fade(false);
			}
		}
	}

	private void Fade(bool _in)
	{
		tween = CreateTween();
		tween.TweenProperty(GetNode("Panel"), "modulate:a", _in ? 1f : 0, 1f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);
		tween.Parallel().TweenProperty(clientTexture, "modulate:a", _in ? 1f : 0, 1f).SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.InOut);

		if (!_in)
			tween.TweenCallback(new Callable(this, nameof(OnFadeOutFinished)));
	}

	private void OnFadeOutFinished()
	{
		QueueFree();
	}

	public void OnAnimationPlayerAnimationFinished(string animName)
	{
		if (animName == "typing")
		{

			dialogState = currentDialog == dialogues.Count - 1 ? DialogState.Finished : DialogState.Idle;

			arrow.Visible = true;
			animPlayer.Play("arrow");
		}
	}

	#endregion

	#region Client
	public void SetClientProperties(ClientProperties properties)
	{
		clientProperties = properties;
		portalModel = GameDataManager.Instance.GetPortal(clientProperties.DesiredDestination);

		clientTexture.Texture = GD.Load<Texture2D>($"{Refs.In.ClientsTexturePath}Client{clientProperties.Id}.png");

		SetClientColors();

		LoadDialog();

		Fade(true);
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
