using System.Collections.Generic;
using Godot;

public partial class Refs : Node
{
  public static Refs In;

  [ExportGroup("Colors")]
  [Export] public Color BackgroundUI { get; set; }
  [Export] public Color TextColor { get; set; }
  [Export] public Color ButtonColor { get; set; }
  [Export] public Color HeaderColor { get; set; }


  [ExportGroup("Scenes")]
  [Export] public PackedScene World { get; set; }
  [Export] public PackedScene TicketOffice { get; set; }
  [Export] public PackedScene Player { get; set; }
  [Export] public PackedScene Main { get; set; }
  [Export] public PackedScene Client { get; set; }

  [ExportGroup("Textures")]
  [Export] public Texture2D ShipTexture { get; set; }
  [Export] public Texture2D TicketOfficeTexture { get; set; }
  [Export] public Texture2D CursorTexture { get; set; }
  [Export] public Texture2D BorderTexture { get; set; }
  [Export] public Texture2D TileSet01Texture { get; set; }
  [Export] public Texture2D PortalTexture { get; set; }
  [Export] public Texture2D PropsTexture { get; set; }
  [Export] public Texture2D WorldTexture { get; set; }

  [ExportGroup("GUI")]
  [Export] public PackedScene PortalStatsGUI { get; set; }
  [Export] public PackedScene EditModeGUI { get; set; }

  [ExportGroup("SFX")]
  [Export] public AudioStream ClickSFX { get; set; }
  [Export] public AudioStream HoverSFX { get; set; }
  [Export] public AudioStream PlaceSFX { get; set; }
  [Export] public AudioStream RemoveSFX { get; set; }
  [Export] public AudioStream ErrorSFX { get; set; }
  [Export] public AudioStream PortalSFX { get; set; }
  [Export] public AudioStream ShipSFX { get; set; }
  [Export] public AudioStream TicketOfficeAmbienceSFX { get; set; }
  [Export] public AudioStream WorldAmbienceSFX { get; set; }


  [ExportGroup("Music")]
  [Export] public AudioStream MainTheme { get; set; }

  public override void _Ready()
  {
    In = this;
  }
}