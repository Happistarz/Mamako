using Godot;
using System;

public partial class PortalStatsGUI : Control
{

	[Export] public Label PortalName;
	[Export] public Label PortalPrice;
	[Export] public Label PortalType;

	[Export] public Button RemoveButton;

	private Portal portal;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void SetPortalStats(Portal portal)
	{
		this.portal = portal;
		PortalName.Text = portal.Name;
		PortalPrice.Text = $"{Mathf.Round(portal.Price)}€";
		PortalType.Text = portal.Type.ToString();
	}

	public void OnRemoveButtonPressed()
	{
		Global.Instance.RemovePortal(portal);
		Global.Instance.Money += Mathf.RoundToInt(portal.Price);
		Global.Instance.tileMap.worldResourcesGUI.UpdateMoney(Global.Instance.Money);
	}
}
