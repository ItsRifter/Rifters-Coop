using System;
using Sandbox;

[Library( "rc_weapon_override" )]
[Hammer.EntityTool( "Weapon Override", "Rifters Co-Op", "Overrides the weapon for custom maps" )]
[Hammer.EditorSprite( "materials/editor/weapon_spawnpoint.vmat" )]
public partial class WeaponOverride : Entity
{

	[Property( "UniqueWorldModelPathPistol" ), Description( "Uses a unique pistol world model path"), ]
	public string Unique_World_Model_Pistol { get; set; }

	[Property( "UniqueViewModelPathPistol" ), Description( "Uses a unique pistol view model path (LEAVE BLANK TO USE DEFAULT)" ), ]
	public string Unique_View_Model_Pistol { get; set; }

	[Property( "UniqueSoundFire" ), Description( "Play a unique fire Sound (LEAVE BLANK TO USE DEFAULT)" )]
	public string Unique_Fire_Pistol { get; set; }

	public override void Spawn()
	{
		base.Spawn();
	}
}
