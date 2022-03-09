using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

partial class Crowbar : WepBase
{
	public override string WorldModelPath => "models/weapons/crowbar.vmdl";
	
	//public override string ViewModelPath => ""

	public override float PrimaryRate => 1.5f;
	public override float SecondaryRate => 0.3f;
	public override bool IsMelee => true;
	public override int Bucket => 1;

	public override int Damage => 25;

	public int MeleeDistance = 80;

	public override void Spawn()
	{
		base.Spawn();
	}

	public override void AttackPrimary()
	{
		base.AttackPrimary();
		MeleeStrike();
	}

	public virtual void MeleeStrike()
	{
		var forward = Owner.EyeRotation.Forward;
		forward = forward.Normal;

		foreach ( var tr in TraceBullet( Owner.EyePosition, Owner.EyePosition + forward * MeleeDistance, 10f ) )
		{
			if ( !tr.Entity.IsValid() ) continue;

			tr.Surface.DoBulletImpact( tr );

			if ( !IsServer ) continue;

			using ( Prediction.Off() )
			{
				var damageInfo = DamageInfo.FromBullet( tr.EndPosition, forward * 100 * 5, Damage )
					.UsingTraceResult( tr )
					.WithAttacker( Owner )
					.WithWeapon( this );

				tr.Entity.TakeDamage( damageInfo );
			}
		}
	}
}
