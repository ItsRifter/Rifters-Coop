
using Sandbox;
using Sandbox.UI.Construct;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public partial class RifterGame : Sandbox.Game
{
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

	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		var player = new PlayerBase();
		client.Pawn = player;

		player.Respawn();
	}
}
