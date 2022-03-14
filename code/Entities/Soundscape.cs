using Hammer;
using System;
using Sandbox;
using Sandbox.Internal;

[Library( "rc_soundscape" )]
[Hammer.EntityTool( "Soundscape", "Rifters Co-Op", "Loops a sound in an area until the next soundscape is triggered" )]
[Hammer.EditorSprite( "materials/editor/soundscape.vmat" )]
[Sphere( "sound_radius", byte.MaxValue, byte.MaxValue, 0, false )]
[Hammer.VisGroup( Hammer.VisGroup.Sound )]

public partial class Soundscape : Entity
{
	[Property, Sandbox.Description("The trigger radius")]
	[DefaultValue( 512 )]
	public float sound_radius { get; protected set; }
	
	[Property, Sandbox.Description("The looping sound to play upon trigger, Leaving it empty clears soundscapes from being heard"), FGDType("sound")]
	public string Sound_Path { get; protected set; }

	public override void Spawn()
	{
		base.Spawn();	
	}

	[Event.Tick.Server]
	public void TickSoundScape()
	{
		var other = FindInSphere( Position, sound_radius );

		foreach ( var client in other )
		{
			if ( client is PlayerBase player )
			{
				if ( player.GetSoundScapePath() == Sound_Path )
					return;

				player.PlaySoundScape( Sound_Path );
			}
		}
		
	}


}

