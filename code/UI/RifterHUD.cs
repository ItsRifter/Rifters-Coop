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
	}
}

