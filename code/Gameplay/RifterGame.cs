
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class RifterGame : Sandbox.Game
{
	public static new RifterGame Current => Sandbox.Game.Current as RifterGame;

	private RifterHUD oldHud;

	public RifterGame()
	{
		if(IsClient)
			oldHud = new RifterHUD();
	}

	[Event.Hotload]
	public void UpdateHUD()
	{
		oldHud?.Delete();

		if ( IsClient )
			oldHud = new RifterHUD();
	}

	[ClientRpc]
	public void DisplayTextUser(string text, double duration, int xPos, int yPos )
	{
		if ( oldHud == null )
			return;

		if(IsClient)
			oldHud.DisplayTextHUD( text, duration, xPos, yPos );
	}

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		var player = new PlayerBase();
		client.Pawn = player;

		player.Respawn();
	}
}
