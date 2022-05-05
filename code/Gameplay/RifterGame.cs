
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
		player.ApplyClothing();
	}

	[ServerCmd( "rc_impulse" )]
	public static void ImpulseCMD( int impulseCMD )
	{
		var caller = ConsoleSystem.Caller.Pawn as PlayerBase;

		if ( caller == null )
			return;

		if ( impulseCMD == 101 )
		{
			caller.Inventory.Add( new Crowbar() );
			caller.Inventory.Add( new Glock() );
			caller.Inventory.Add( new ColtMagnum() );
			caller.Inventory.Add( new SMG() );

			caller.SetAmmo( AmmoType.Pistol, 150 );
			caller.SetAmmo( AmmoType.Magnum, 12 );
			caller.SetAmmo( AmmoType.SMG, 225 );
			caller.SetAmmo( AmmoType.Buckshot, 30 );
			caller.SetAmmo( AmmoType.Crossbow, 10 );
			caller.SetAmmo( AmmoType.Grenades, 5 );
			caller.SetAmmo( AmmoType.Rockets, 3 );
			caller.SetAmmo( AmmoType.Misc, 50 );
		}
	}
}
