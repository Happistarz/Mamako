using Godot;
using System;

public class ClientProperties
{
  public string Name;
  public float Speed;
  public float ShipSize;
  public Color ShipColor;
  public PortalType DesiredDestination;
  public AudioStream ShipRadio;
  public AudioStream ClientVoice;

  public ClientProperties() { }

  public override string ToString()
  {
    return $"{Name} is a {ShipSize} ship with a speed of {Speed} and a color of {ShipColor}. They want to go to a {DesiredDestination}.";
  }

  public static bool operator ==(ClientProperties a, ClientProperties b)
  {
    if (ReferenceEquals(a, b))
      return true;

    if (a is null || b is null)
      return false;

    return a.Name == b.Name &&
         a.Speed == b.Speed &&
         a.ShipSize == b.ShipSize &&
         a.ShipColor == b.ShipColor &&
         a.DesiredDestination == b.DesiredDestination;
  }

  public static bool operator !=(ClientProperties a, ClientProperties b)
  {
    return !(a == b);
  }

  public override bool Equals(object obj)
  {
    if (obj == null || !(obj is ClientProperties client))
      return false;

    return Name == client.Name &&
         Speed == client.Speed &&
         ShipSize == client.ShipSize &&
         ShipColor == client.ShipColor &&
         DesiredDestination == client.DesiredDestination;
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(Name, Speed, ShipSize, ShipColor, DesiredDestination);
  }
}

public interface IColorableClient
{
  public Color[] GenerateColorArray();
}

public class Pablo : ClientProperties, IColorableClient
{
  public Color ClientHelmetPrimaryColor;
  public Color ClientHelmetSecondaryColor;
  public Color ClientChestPrimaryColor;
  public Color ClientChestSecondaryColor;

  public Pablo()
  {
    ClientChestPrimaryColor = Utils.GenerateColor();
    ClientChestSecondaryColor = Utils.GenerateColor();
    ClientHelmetPrimaryColor = Utils.GenerateColor();
    ClientHelmetSecondaryColor = Utils.GenerateColor();
  }

  public override string ToString()
  {
    return $"{Name} is a {ShipSize} ship with a speed of {Speed} and a color of {ShipColor}. They want to go to a {DesiredDestination}. They have a helmet with primary color {ClientHelmetPrimaryColor} and secondary color {ClientHelmetSecondaryColor}. They have a chest with primary color {ClientChestPrimaryColor} and secondary color {ClientChestSecondaryColor}.";
  }

  public Color[] GenerateColorArray()
  {
    return new Color[] { ClientHelmetPrimaryColor, ClientHelmetSecondaryColor, ClientChestPrimaryColor, ClientChestSecondaryColor };
  }
}