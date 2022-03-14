using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sandbox;

[Library( "rc_breakable", Description = "Similar to func_breakable, a mesh with health that can break" )]
[Hammer.Model]
[Hammer.SupportsSolid]
[Hammer.RenderFields]
partial class BreakableWall : AnimEntity
{
	[Property( "HealthBreak"), Description( "Health until break, -1 makes it unbreakable to users" )]
	public float HealthUntilBreak { get; set; } = 1;

	[Property( "MinDamage" ), Description( "Minimal required to damage this mesh" )]
	public float MinDamage { get; set; } = 0;

	[Property( "DamagedSound" ), Description( "Sound to play upon damage" ), FGDType( "sound" )]
	public string DamageSound { get; set; }

	[Property( "DestroyedSound" ), Description("Sound to play upon desctruction"), FGDType( "sound" )]
	public string DestroyedSound { get; set; }

	protected Output OnBroken { get; set; }

	public override void Spawn()
	{
		base.Spawn();
		SetupPhysicsFromModel( PhysicsMotionType.Keyframed );
	}

	[Input]
	public void Break()
	{
		Sound.FromEntity( DestroyedSound, this );
		_ = OnBroken.Fire( this );
		OnKilled();
	}

	public override void TakeDamage( DamageInfo info )
	{
		if ( HealthUntilBreak == -1 )
			return;

		if ( MinDamage != 0 && info.Damage < MinDamage )
			info.Damage = 0;

		HealthUntilBreak -= info.Damage;

		if ( HealthUntilBreak <= 0 )
		{
			Sound.FromEntity( DestroyedSound, this );
			_ = OnBroken.Fire( this );
			OnKilled();
		} 
		else
		{
			Sound.FromEntity( DamageSound, this );
		}
	}
}
