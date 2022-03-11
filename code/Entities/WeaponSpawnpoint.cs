using System;
using Sandbox;

[Library("rc_weapon_point")]
[Hammer.EntityTool( "Weapon Spawnpoint", "Rifters Co-Op", "Defines an weapons spawnpoint" )]
[Hammer.EditorSprite( "materials/editor/weapon_spawnpoint.vmat" )]
public partial class WeaponSpawnpoint : Entity
{
	public enum WeaponEnum
	{
		Unspecified,
		Crowbar,
		Glock
	}

	[Property( "WeaponType" ), Description( "What type of weapon is this" )]
	public WeaponEnum Weapon_To_Spawn { get; set; } = WeaponEnum.Unspecified;
	public Output OnSpawn { get; set; }

	[Property( "SpawnOnMap" ), Description( "Should this spawn immediately on map load" )]
	public bool Spawn_Immediately { get; set; } = true;
	public override void Spawn()
	{
		base.Spawn();
	}

	[Input]
	public void SpawnWeapon()
	{
		if ( Weapon_To_Spawn == WeaponEnum.Unspecified )
			return;

		var weapon = Library.Create<WepBaseCoop>( Weapon_To_Spawn.ToString() );

		weapon.Spawn();

		weapon.Position = Position;
		weapon.Rotation = Rotation;

		OnSpawn.Fire(this);
	}
}

