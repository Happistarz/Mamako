using System;
using Godot;
using System.Collections.Generic;

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
	public static void PrintList<T>(IEnumerable<T> values)
	{
		GD.Print("---");
		foreach (T value in values)
		{
			GD.Print(value);
		}
		GD.Print("---");
	}

	public static void PrintDictionary<K, V>(Dictionary<K, V> dictionary)
	{
		GD.Print("---");
		foreach (KeyValuePair<K, V> pair in dictionary)
		{
			GD.Print($"{pair.Key}: {pair.Value}");
		}
		GD.Print("---");
	}

	public static Color GenerateColor()
	{
		return new Color(GD.Randf(), GD.Randf(), GD.Randf());
	}
}