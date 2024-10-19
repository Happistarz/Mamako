using Godot;
using System;
using System.Collections.Generic;

public partial class ClientFactory
{
	// private static List<Color> GenerateClientColors()
	// {
	// 	List<Color> colors = new()
	// 	{
	// 		Utils.GenerateColor(),
	// 		Utils.GenerateColor(),
	// 		Utils.GenerateColor(),
	// 	};
	// 	return colors;
	// }

	public static T GenerateClientProperties<T>() where T : ClientProperties, new()
	{
		Type type = typeof(T) == typeof(ClientProperties) ? typeof(ClientProperties) : typeof(T);
		ClientProperties properties = (ClientProperties)Activator.CreateInstance(type);

		properties.Name = GenerateName();
		properties.Speed = (float)GD.RandRange(1f, 5.0f);
		properties.DesiredDestination = (PortalType)GD.RandRange(0, Enum.GetValues(typeof(PortalType)).Length - 1);
		properties.ShipSize = (float)GD.RandRange(0.9f, 1.3f);
		properties.ShipColor = Utils.GenerateColor();

		return (T)properties;
	}

	public static Client CreateClient(Node parent)
	{
		Client client = Refs.In.Client.Instantiate<Client>();
		client.Properties = GenerateClientProperties<ClientProperties>();

		parent.AddChild(client);

		return client;
	}

	private static string GenerateName()
	{
		string[] names = new string[]
		{
			"John",
			"Jane",
			"Bob",
			"Alice",
			"Charlie",
			"David",
			"Eve",
			"Frank",
		};
		return names[GD.RandRange(0, names.Length - 1)];
	}
}
