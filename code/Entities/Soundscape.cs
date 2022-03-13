using Hammer;
using System;
using Sandbox;
using Sandbox.Internal;

[Library( "rc_soundscape" )]
[Hammer.EntityTool( "Soundscape", "Rifters Co-Op", "Loops a sound in an area until the next soundscape is triggered" )]
//[Hammer.EditorSprite( "materials/editor/soundscape.vmat" )]
//[Sphere( "sound_radius", byte.MaxValue, byte.MaxValue, 0, false )]
[Hammer.VisGroup( Hammer.VisGroup.Trigger )]

public partial class Soundscape : BaseTrigger
{
	//[Property, Sandbox.Description("The trigger radius")]
	//[DefaultValue( 512 )]
	//public float sound_radius { get; protected set; }
	
	[Property, Sandbox.Description("The looping sound to play upon trigger, Leaving it empty clears soundscapes from being heard"), FGDType("sound")]
	public string Sound_Path { get; protected set; }

	private PlayerBase listener;

	public override void Spawn()
	{
		base.Spawn();
	}

	public override void StartTouch( Entity other )
	{
		if ( other is PlayerBase player )
			listener = player;


		if ( listener != null && listener.GetSoundScapePath() != Sound_Path )
		{
			listener.PlaySoundScape( Sound_Path );
		}

		base.StartTouch( other );
	}

}

