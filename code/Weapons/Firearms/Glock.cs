using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

partial class Glock : WepBaseCoop
{
	public override AmmoType AmmoType => AmmoType.Pistol;
	public override string ViewModelPath => "models/weapons/v_glock.vmdl";
	public override string WorldModelPath => "models/weapons/glock19.vmdl";
	public override int ClipSize => 20;
	public override float PrimaryRate => 10.0f;
	public override float SecondaryRate => 7.5f;
	public override float ReloadTime => 2.25f;
	public override int Damage => 15;
	public override int Bucket => 1;
	public override int BucketWeight => 0;
	public override string PickupSound { get; set; } = "default_pickup";
	public override string FireSound { get; set; } = "pistol_fire";
	public override float WaitFinishDeployed => 0.85f;
	public override float Recoil => 1.75f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( WorldModelPath );

		AmmoClip = ClipSize;
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
		return base.CanPrimaryAttack() && Input.Pressed( InputButton.Attack1 );
	}

	public override bool CanSecondaryAttack()
	{
		return base.CanSecondaryAttack() && Input.Down( InputButton.Attack2 );
	}

	public override bool CanReload()
	{
		if ( TimeSincePrimaryAttack < 0.5f || TimeSinceSecondaryAttack < 0.5f) return false;

		return base.CanReload();
	}

	public override void Reload()
	{
		base.Reload();
		ViewModelEntity?.SetAnimParameter( "empty", false );
	}
	public override void AttackPrimary()
	{
		base.AttackPrimary();

		if ( !TakeAmmo( 1, false ) )
		{
			DryFire();
			return;
		}

		if ( AmmoClip == 5 )
			PlaySound( "hud_ammo_warning" );

		ShootEffects();
		PlaySound( FireSound );

		ShootBullet( 0.2f, 1.5f, Damage, 3.0f );

		if ( AmmoClip <= 0 )
			ViewModelEntity?.SetAnimParameter( "empty", true );
	}

	public override void AttackSecondary()
	{
		base.AttackSecondary();

		if ( !TakeAmmo( 1, false ) )
		{
			DryFire();
			return;
		}

		if ( AmmoClip == 5 )
			PlaySound( "hud_ammo_warning" );

		ShootEffects();
		PlaySound( FireSound );

		ShootBullet( 0.30f, 1.5f, Damage, 3.0f );

		if ( AmmoClip <= 0 )
			ViewModelEntity?.SetAnimParameter( "empty", true );
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );

		if ( IsLocalPawn )
		{
			new Sandbox.ScreenShake.Perlin( 1, 1.0f, Recoil );
		}

		if(AmmoClip > 0)
		{
			if( ViewModelEntity?.GetAnimParameterInt( "rand_fire_max" ) > 0 )
				ViewModelEntity?.SetAnimParameter( "rand_fire", Rand.Int( 1, ViewModelEntity.GetAnimParameterInt( "rand_fire_max" ) ) );
		}
		else
		{
			if ( ViewModelEntity?.GetAnimParameterBool( "empty" ) != false )
				ViewModelEntity?.SetAnimParameter( "empty", true );
		}

		ViewModelEntity?.SetAnimParameter( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}
}

