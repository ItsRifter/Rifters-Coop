using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

partial class Crowbar : WepBaseCoop
{
	public override AmmoType AmmoType => AmmoType.Unspecified;
	public override string WorldModelPath => "models/weapons/crowbar.vmdl";
	public override string ViewModelPath => "models/weapons/v_crowbar.vmdl";
	public override float PrimaryRate => 2.0f;
	public override float SecondaryRate => 0.0f;
	public override bool IsMelee => true;
	public override int Bucket => 0;
	public override int BucketWeight => 0;
	public override int ClipSize => 0;
	public override int Damage => 25;
	public override float WaitFinishDeployed => 1.25f;

	public int MeleeDistance = 60;

	public override void Spawn()
	{
		base.Spawn();
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Down(InputButton.Attack1);
	}

	public override void AttackPrimary()
	{
		base.AttackPrimary();
		MeleeStrike();
	}

	public virtual void MeleeStrike()
	{
		var tr = Trace.Ray( Owner.EyePosition, Owner.EyePosition + Owner.EyeRotation.Forward * MeleeDistance )
			.Ignore( Owner )
			.Run();

		if ( !tr.Entity.IsValid() || !tr.Hit )
		{
			ViewModelEntity?.SetAnimParameter( "b_miss", true );
			ViewModelEntity?.SetAnimParameter( "miss_num", Rand.Int( 1, ViewModelEntity.GetAnimParameterInt( "miss_num_max" ) ) );
			return;
		}

		ViewModelEntity?.SetAnimParameter( "b_hit", true );
		ViewModelEntity?.SetAnimParameter( "hit_num", Rand.Int( 1, ViewModelEntity.GetAnimParameterInt( "hit_num_max" ) ) );

		if ( !IsServer ) return;

		tr.Surface.DoBulletImpact( tr );

		using ( Prediction.Off() )
		{
			ShootBullet( 0.0f, 1.0f, Damage, 1.0f );
		}
	}
}
