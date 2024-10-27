using Godot;
using System;

public class ClientProperties
{
  public virtual string Id { get; protected set; }
  public string Name;
  public float Speed;
  public float ShipSize;
  public Color ShipColor;
  public PortalType DesiredDestination;
  public ProductModel Product;
  public AudioStream ShipRadio;
  public AudioStream ClientVoice;

  public ClientProperties() { }

  public override string ToString()
  {
    return $"Id {Id};{Name} is a {ShipSize} ship with a speed of {Speed} and a color of {ShipColor}. They want to go to a {DesiredDestination} to buy {Product}.";
  }

  public static bool operator ==(ClientProperties a, ClientProperties b)
  {
    if (ReferenceEquals(a, b))
      return true;

    if (a is null || b is null)
      return false;

    return a.Id == b.Id &&
         a.Name == b.Name &&
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

    return Id == client.Id &&
         Name == client.Name &&
         Speed == client.Speed &&
         ShipSize == client.ShipSize &&
         ShipColor == client.ShipColor &&
         DesiredDestination == client.DesiredDestination;
  }

  public override int GetHashCode()
  {
    return HashCode.Combine(Id, Name, Speed, ShipSize, ShipColor, DesiredDestination);
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
    Id = "01";
    ClientChestPrimaryColor = Utils.GenerateColor();
    ClientChestSecondaryColor = Utils.GenerateColor();
    ClientHelmetPrimaryColor = Utils.GenerateColor();
    ClientHelmetSecondaryColor = Utils.GenerateColor();
  }

  public Color[] GenerateColorArray()
  {
    return new Color[] { ClientHelmetPrimaryColor, ClientHelmetSecondaryColor, ClientChestPrimaryColor, ClientChestSecondaryColor };
  }
}