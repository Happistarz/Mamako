using System.Collections.Generic;
using Godot;

public partial class Refs : Node
{
  public static Refs In;

  [ExportGroup("Colors")]
  [Export] public Color BackgroundUI { get; private set; }
  [Export] public Color TextColor { get; private set; }
  [Export] public Color ButtonColor { get; private set; }
  [Export] public Color HeaderColor { get; private set; }


  [ExportGroup("Scenes")]
  [Export] public PackedScene World { get; private set; }
  [Export] public PackedScene TicketOffice { get; private set; }
  [Export] public PackedScene Player { get; private set; }
  [Export] public PackedScene Main { get; private set; }
  [Export] public PackedScene Client { get; private set; }

  [ExportGroup("Textures")]
  [Export] public Texture2D ShipTexture { get; private set; }
  [Export] public Texture2D TicketOfficeTexture { get; private set; }
  [Export] public Texture2D CursorTexture { get; private set; }
  [Export] public Texture2D BorderTexture { get; private set; }
  [Export] public Texture2D TileSet01Texture { get; private set; }
  [Export] public Texture2D PortalTexture { get; private set; }
  [Export] public Texture2D PropsTexture { get; private set; }
  [Export] public Texture2D WorldTexture { get; private set; }

  [ExportGroup("GUI")]
  [Export] public PackedScene PortalStatsGUI { get; private set; }
  [Export] public PackedScene EditModeGUI { get; private set; }

  [ExportGroup("SFX")]
  [Export] public AudioStream ClickSFX { get; private set; }
  [Export] public AudioStream HoverSFX { get; private set; }
  [Export] public AudioStream PlaceSFX { get; private set; }
  [Export] public AudioStream RemoveSFX { get; private set; }
  [Export] public AudioStream ErrorSFX { get; private set; }
  [Export] public AudioStream PortalSFX { get; private set; }
  [Export] public AudioStream ShipSFX { get; private set; }
  [Export] public AudioStream TicketOfficeAmbienceSFX { get; private set; }
  [Export] public AudioStream WorldAmbienceSFX { get; private set; }


  [ExportGroup("Music")]
  [Export] public AudioStream MainTheme { get; private set; }


  [ExportGroup("DataPaths")]
  [Export] public string PortalsDataPath { get; private set; } = "res://Resources/";
  [Export] public string ClientsTexturePath { get; private set; } = "res://Assets/Textures/";
  [Export] public string DialogsJsonPath { get; private set; } = "res://Resources/";

  public override void _Ready()
  {
    In = this;
  }
}