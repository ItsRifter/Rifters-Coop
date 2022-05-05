using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

partial class SMG : WepBaseCoop
{
	public override AmmoType AmmoType => AmmoType.SMG;
	public override string ViewModelPath => "models/weapons/v_smg.vmdl";
	public override string WorldModelPath => "models/weapons/smg.vmdl";
	public override int ClipSize => 45;
	public override int SecondaryClipSize => 3;
	public override float PrimaryRate => 15.0f;
	public override float SecondaryRate => 0.75f;
	public override float ReloadTime => 2.25f;
	public override int Damage => 10;
	public override int Bucket => 2;
	public override int BucketWeight => 0;
	public override string PickupSound { get; set; } = "default_pickup";
	public override string FireSound { get; set; } = "smg1_fire";
	public override float WaitFinishDeployed => 0.85f;
	public override float Recoil => 2.25f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( WorldModelPath );

		AmmoClip = ClipSize;
		SecondaryAmmoClip = 99;
	}

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart(ent);

		if ( AmmoClip <= 0 && !IsMelee )
		{
			ViewModelEntity?.SetAnimParameter( "empty", true );
		}
	}

	public override void CreateViewModel()
	{
		Host.AssertClient();

		ViewModelEntity = new RCViewmodel();

		ViewModelEntity.SetModel( ViewModelPath );

		ViewModelEntity.Position = Position;
		ViewModelEntity.Owner = Owner;
		ViewModelEntity.EnableViewmodelRendering = true;
	}

	public override bool CanPrimaryAttack()
	{
		return base.CanPrimaryAttack() && Input.Down( InputButton.Attack1 );
	}

	public override bool CanSecondaryAttack()
	{
		return base.CanSecondaryAttack() && Input.Pressed( InputButton.Attack2 );
	}

	public override bool CanReload()
	{
		if ( TimeSincePrimaryAttack < 0.5f || TimeSinceSecondaryAttack < 0.5f) return false;

		return base.CanReload();
	}

	public override void Reload()
	{
		base.Reload();
	}
	public override void AttackPrimary()
	{
		base.AttackPrimary();

		if ( !TakeAmmo( 1, false ) )
		{
			DryFire();
			return;
		}

		if ( AmmoClip == 15 )
			PlaySound( "hud_ammo_warning" );

		ShootEffects();
		PlaySound( FireSound );

		ShootBullet( 0.2f, 1.5f, Damage, 3.0f );
	}

	public override void AttackSecondary()
	{
		base.AttackSecondary();

		if ( !TakeAmmo( 1, true ) )
		{
			DryFire();
			return;
		}

		PlaySound( FireSound );
		ViewModelEntity?.SetAnimParameter( "altfire", true );
		//ShootExplosive( 0.0f, 2.5f, Damage * 2);
	}

	public override void ShootExplosive( float spread, float force, float damage )
	{
		base.ShootExplosive( spread, force, damage );

		using ( Prediction.Off() )
		{
			var nade = new Nade();
			nade.Position = Owner.EyePosition;
			nade.Rotation = Owner.EyeRotation;
			nade.Owner = Owner;
			nade.Velocity = Owner.EyeRotation.Forward * 100;
		}
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );

		if ( IsLocalPawn )
		{
			new Sandbox.ScreenShake.Perlin( 1.0f, 1.0f, Recoil );
		}
	
		if( ViewModelEntity?.GetAnimParameterInt( "rand_fire_max" ) > 0 )
			ViewModelEntity?.SetAnimParameter( "rand_fire", Rand.Int( 1, ViewModelEntity.GetAnimParameterInt( "rand_fire_max" ) ) );
	
		ViewModelEntity?.SetAnimParameter( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}

	public override void SimulateAnimator( PawnAnimator anim )
	{
		anim.SetAnimParameter( "holdtype", 2 );
	}
}

