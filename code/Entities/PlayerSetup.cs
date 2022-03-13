
using System;
using System.Collections.Generic;
using Sandbox;

[Library( "rc_player_setup" )]
[Hammer.EntityTool( "Player Setup", "Rifters Co-Op", "Sets up the player for the map" )]
[Hammer.EditorSprite( "materials/editor/weapon_spawnpoint.vmat" )]

public partial class PlayerSetup : Entity
{

	[Property( "EnableFlashlight" ), Description( "Is flashlight enabled or disabled on spawn" )]
	public bool Enable_Flashlight { get; set; } = true;

	[Property( "GiveCrowbar" ), Description( "Give the Crowbar on player spawn" )]
	public bool Should_Give_Crowbar { get; set; } = false;

	[Property( "GiveGlock" ), Description( "Give the Glock on player spawn" )]
	public bool Should_Give_Glock { get; set; } = false;

	[Property( "GiveRevolver" ), Description( "Give the Revolver on player spawn" )]
	public bool Should_Give_Revolver { get; set; } = false;

	private List<WepBaseCoop> weaponsToGive;

	public override void Spawn()
	{
		base.Spawn();
		weaponsToGive = new List<WepBaseCoop>();

		if ( Should_Give_Crowbar )
			weaponsToGive.Add( Library.Create<WepBaseCoop>( "Crowbar" ) );

		if ( Should_Give_Glock )
			weaponsToGive.Add( Library.Create<WepBaseCoop>( "Glock" ) );

		if ( Should_Give_Revolver )
			weaponsToGive.Add( Library.Create<WepBaseCoop>( "ColtMagnum" ) );
	}

	[Event.Tick.Server]
	public void GivePlayersInventory()
	{
		foreach ( var client in Client.All )
		{
			if (client.Pawn is PlayerBase player && player.DidRespawn )
			{
				foreach ( var weapon in weaponsToGive )
				{
					if( !player.Inventory.Contains(weapon) )
						player.Inventory.Add( Library.Create<WepBaseCoop>(weapon.GetType()) );

					
				}

				player.ActiveChild = player.Inventory.GetSlot( Rand.Int( 0, player.Inventory.Count() - 1 ) );
				player.DidRespawn = false;
			}
		}
	}

}

