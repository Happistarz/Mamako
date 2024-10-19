using System;
using Godot;
using System.Collections.Generic;

public enum PortalType
{
	SpaceMarket,
	AlienTechStore,
	IntergalacticBazaar,
	StarshipPartsDepot,
	AlienCafe,
	QuantumGadgetry,
	SpaceDiner,
	ZeroGravityGym,
	StellarLibrary,
	AlienMuseum,
	SpaceObservatory,
	InterstellarPark,
	BlackHoleCafe,
	Spaceport,
	AlienZoo,
	StarshipRepairStation,
	SpaceCasino,
	Planetarium,
	SpaceOperaHouse,
	IntergalacticEmbassy
}

// categorise PortalType into groups
public enum PortalGroup
{
	Market,
	Store,
	Entertainment,
	Recreation,
	Education,
	Transport,
	Culture,
	Science,
	Government
}

// link PortalType to PortalGroup
public static class PortalGroupMap
{
	public static PortalGroup GetGroup(PortalType type)
	{
		return type switch
		{
			PortalType.SpaceMarket => PortalGroup.Market,
			PortalType.AlienTechStore or PortalType.StarshipPartsDepot => PortalGroup.Store,
			PortalType.AlienCafe or PortalType.SpaceDiner or PortalType.BlackHoleCafe or PortalType.SpaceCasino => PortalGroup.Entertainment,
			PortalType.ZeroGravityGym or PortalType.InterstellarPark => PortalGroup.Recreation,
			PortalType.StellarLibrary or PortalType.AlienMuseum or PortalType.AlienZoo or PortalType.Planetarium or PortalType.SpaceOperaHouse => PortalGroup.Education,
			PortalType.QuantumGadgetry or PortalType.StarshipRepairStation => PortalGroup.Transport,
			PortalType.SpaceObservatory => PortalGroup.Science,
			PortalType.IntergalacticEmbassy => PortalGroup.Government,
			_ => PortalGroup.Market,
		};
	}
}

public class Portal
{
	public string Name;
	public PortalType Type;
	public Vector2 TileMapIndex;

	public Vector2 Position;

	public float Price;

	public Portal() { }

	public override string ToString()
	{
		return $"{Type} at {TileMapIndex} with average price of {Price}";
	}

	public static bool operator ==(Portal a, Portal b)
	{
		if (ReferenceEquals(a, b))
			return true;

		if (a is null || b is null)
			return false;

		return a.TileMapIndex == b.TileMapIndex && a.Type == b.Type && a.Price == b.Price;
	}

	public static bool operator !=(Portal a, Portal b)
	{
		return !(a == b);
	}

	public override bool Equals(object obj)
	{
		if (obj == null || !(obj is Portal portal))
			return false;

		return TileMapIndex == portal.TileMapIndex &&
				 Type == portal.Type &&
				 Price == portal.Price;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(TileMapIndex, Type, Price);
	}
}

public class Utils
{
	public static void Print<T>(IEnumerable<T> values)
	{
		GD.Print("---");
		foreach (T value in values)
		{
			GD.Print(value);
		}
		GD.Print("---");
	}

	public static Color GenerateColor()
	{
		return new Color(GD.Randf(), GD.Randf(), GD.Randf());
	}
}