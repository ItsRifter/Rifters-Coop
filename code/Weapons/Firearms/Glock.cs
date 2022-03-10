using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

partial class Glock : WepBase
{
	public override string ViewModelPath => "models/weapons/v_glock.vmdl";
	public override string WorldModelPath => "models/weapons/glock19.vmdl";
	public override int ClipSize => 20;
	public override float PrimaryRate => 15.0f;
	public override float SecondaryRate => 1.0f;
	public override float ReloadTime => 2.25f;
	public override int Damage => 20;
	public override int Bucket => 1;
	public override string PickupSound { get; set; } = "default_pickup";
	public override string FireSound { get; set; } = "pistol_fire";

	public override void Spawn()
	{
		base.Spawn();


		SetModel( WorldModelPath );

	}

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart(ent);		
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

	public override void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;

		if ( !TakeAmmo( 1 ) )
		{
			DryFire();
			return;
		}

		ShootEffects();
		PlaySound( FireSound );

		ShootBullet( 0.2f, 1.5f, 9.0f, 3.0f );

	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );

		if ( IsLocalPawn )
		{
			new Sandbox.ScreenShake.Perlin();
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

