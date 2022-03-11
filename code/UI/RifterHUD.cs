using System;
using Sandbox;
using Sandbox.UI;

public partial class RifterHUD : Sandbox.HudEntity<RootPanel>
{
	public RifterHUD()
	{
		RootPanel.AddChild<ChatBox>();
		RootPanel.AddChild<Scoreboard<ScoreboardEntry>>();
		RootPanel.AddChild<NameTags>();

		RootPanel.AddChild<VitalSigns>();
		RootPanel.AddChild<AmmoCount>();
		RootPanel.AddChild<InventoryBar>();
	}


	public void DisplayTextHUD(string text, double duration, int xPos, int yPos )
	{
		var displayHUD = new DisplayText( text, duration, xPos, yPos );

		RootPanel.AddChild( displayHUD );
	}
}

