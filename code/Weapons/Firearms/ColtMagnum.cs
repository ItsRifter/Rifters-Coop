using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

partial class ColtMagnum : WepBaseCoop
{
	public override AmmoType AmmoType => AmmoType.Magnum;
	public override string ViewModelPath => "models/weapons/v_357.vmdl";
	public override string WorldModelPath => "models/weapons/357.vmdl";
	public override int ClipSize => 6;
	public override float PrimaryRate => 1.25f;
	public override float SecondaryRate => 0.0f;
	public override float ReloadTime => 3.25f;
	public override int Damage => 40;
	public override int Bucket => 1;
	public override int BucketWeight => 1;
	public override string PickupSound { get; set; } = "default_pickup";
	public override string FireSound { get; set; } = "357_fire";
	public override float WaitFinishDeployed => 0.95f;
	public override float Recoil => 3.5f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( WorldModelPath );

		AmmoClip = ClipSize;
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

	public override bool CanSecondaryAttack()
	{
		return false;
	}

	public override bool CanReload()
	{
		if ( TimeSincePrimaryAttack < 0.95f ) return false;

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

		if ( AmmoClip == 2 )
			Sound.FromScreen( "hud_ammo_warning" );

		ShootEffects();
		PlaySound( FireSound );

		ShootBullet( 0.05f, 1.5f, Damage, 3.0f );
	}

	[ClientRpc]
	protected override void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );

		if ( IsLocalPawn )
		{
			new Sandbox.ScreenShake.Perlin(1, 0.6f, Recoil);
		}

		ViewModelEntity?.SetAnimParameter( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}
}

