
using System;
using Sandbox;

[Library( "rc_player_setup" )]
[Hammer.EntityTool( "Player Setup", "Rifters Co-Op", "Sets up the player for the map" )]
[Hammer.EditorSprite( "materials/editor/weapon_spawnpoint.vmat" )]

public partial class PlayerSetup : Entity
{

	[Property( "EnableFlashlight" ), Description("Is flashlight enabled or disabled on spawn")]
	public bool Enable_Flashlight { get; set; } = true;




}

