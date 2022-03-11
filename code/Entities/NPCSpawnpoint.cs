using System;
using Sandbox;

[Library( "rc_npc_spawnpoint" )]
[Hammer.Model]
[Hammer.RenderFields()]
[Hammer.EntityTool( "NPC Spawnpoint", "Rifters Co-Op", "Defines an NPC's spawnpoint with any uniqueness" )]
public partial class NPCSpawnpoint : ModelEntity
{
	[Property( "IsFriendly" ), Description( "Is this a friendly npc (will be friendly to other teams that aren't hostile)" )]
	public bool Is_Friendly_NPC { get; set; } = false;

	[Property( "SpawnOnMap" ), Description( "Should this spawn immediately on map load" )]
	public bool Spawn_Immediately { get; set; } = true;
	public enum NPCSpawnEnum
	{
		Unspecified,
		Zombie,
	}

	[Property( "NPCTeam" ), Description( "Spawns the specific NPC" )]
	public NPCSpawnEnum NPC_To_Spawn { get; set; } = NPCSpawnEnum.Unspecified;

	[Property( "UniqueSoundAlert" ), Description( "Play a unique Alert Sound (LEAVE BLANK TO USE DEFAULT)" )]
	public string Unique_Alert { get; set; }

	[Property( "UniqueSoundIdle" ), Description( "Play a unique Idle Sound (LEAVE BLANK TO USE DEFAULT)" )]
	public string Unique_Idle { get; set; }

	[Property( "UniqueSoundPain" ), Description( "Play a unique Pain Sound (LEAVE BLANK TO USE DEFAULT)" )]
	public string Unique_Pain { get; set; }

	[Property( "UniqueSoundDeath" ), Description( "Play a unique Death Sound (LEAVE BLANK TO USE DEFAULT)" )]
	public string Unique_Death { get; set; }

	[Property( "UniqueHealth" ), Description( "Sets the health (LEAVE AS 0 TO USE DEFAULT)" )]
	public int Unique_Health { get; set; } = 0;

	public Output OnSpawn { get; set; }

	private string curModel;

	public override void Spawn()
	{
		if ( Spawn_Immediately )
			SpawnNPC();

		curModel = GetModelName();

		SetModel( "" );
	}

	[Input]
	public void SpawnNPC()
	{
		if ( NPC_To_Spawn == NPCSpawnEnum.Unspecified )
			return;

		var npc = Library.Create<BaseNPC>( NPC_To_Spawn.ToString() );

		npc.Spawn();

		npc.SetModel( curModel );

		if ( !string.IsNullOrEmpty( Unique_Idle ) )
			npc.IdleSound = Unique_Idle;

		if ( !string.IsNullOrEmpty( Unique_Alert ) )
			npc.AlertSound = Unique_Alert;

		if ( !string.IsNullOrEmpty( Unique_Pain ) )
			npc.PainSound = Unique_Pain;

		if ( !string.IsNullOrEmpty( Unique_Death ) )
			npc.DeathSound = Unique_Death;

		if ( Unique_Health != 0 )
			npc.Health = Unique_Health;

		npc.Position = Position;
		npc.Rotation = Rotation;

		npc.IsFriendly = Is_Friendly_NPC;
		
		OnSpawn.Fire(this);
	}
}

