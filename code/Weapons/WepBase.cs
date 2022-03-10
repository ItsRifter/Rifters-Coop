using System;
using System.Collections.Generic;
using Sandbox;

partial class WepBase : BaseCarriable
{
	public virtual AmmoType AmmoType => AmmoType.Pistol;
	public virtual int Damage => 1;
	public virtual int ClipSize => 20;
	public virtual float ReloadTime => 3.0f;
	public virtual float PrimaryRate => 5.0f;
	public virtual float SecondaryRate => 15.0f;
	public virtual int Bucket => 1;
	public virtual int BucketWeight => 100;
	public virtual bool IsMelee => false;
	public virtual string WorldModelPath => "";
	public virtual new string ViewModelPath { get; set; } = "";
	public virtual string PickupSound { get; set; } = "default_pickup";
	public virtual string FireSound { get; set; } = "";

	[Net, Predicted]
	public TimeSince TimeSincePrimaryAttack { get; set; }

	[Net, Predicted]
	public TimeSince TimeSinceSecondaryAttack { get; set; }

	[Net, Predicted]
	public int AmmoClip { get; set; }

	[Net, Predicted]
	public TimeSince TimeSinceReload { get; set; }

	[Net, Predicted]
	public bool IsReloading { get; set; }

	[Net, Predicted]
	public TimeSince TimeSinceDeployed { get; set; }

	public PickupTrigger PickupTrigger { get; protected set; }

	public int AvailableAmmo()
	{
		var owner = Owner as PlayerBase;
		if ( owner == null ) return 0;
		return owner.AmmoCount( AmmoType );
	}

	public override void ActiveStart( Entity ent )
	{
		base.ActiveStart( ent );

		TimeSinceDeployed = 0;

		if ( AmmoClip <= 0 )
		{
			ViewModelEntity?.SetAnimParameter( "empty", true );
		}

		IsReloading = false;
	}

	public virtual bool CanPrimaryAttack()
	{
		if ( !Owner.IsValid() || !Input.Down( InputButton.Attack1 ) ) return false;

		var rate = PrimaryRate;
		if ( rate <= 0 ) return true;

		return TimeSincePrimaryAttack > (1 / rate);
	}

	public virtual bool CanSecondaryAttack()
	{
		if ( !Owner.IsValid() || !Input.Down( InputButton.Attack2 ) ) return false;

		var rate = SecondaryRate;
		if ( rate <= 0 ) return true;

		return TimeSinceSecondaryAttack > (1 / rate);
	}

	public override void Spawn()
	{
		base.Spawn();

		SetModel( WorldModelPath );

		AmmoClip = ClipSize;

		PickupTrigger = new PickupTrigger();
		PickupTrigger.Parent = this;
		PickupTrigger.Position = Position;
	}

	public virtual void Reload()
	{
		if ( IsReloading )
			return;

		if ( AmmoClip >= ClipSize )
			return;

		TimeSinceReload = 0;

		if ( Owner is PlayerBase player )
		{
			if ( player.AmmoCount( AmmoType ) <= 0 )
				return;
		}

		IsReloading = true;

		(Owner as AnimEntity).SetAnimParameter( "b_reload", true );

		StartReloadEffects();
	}

	public override void Simulate( Client owner )
	{
		if ( TimeSinceDeployed < 0.6f )
			return;

		if ( !IsReloading )
		{
			base.Simulate( owner );
		}

		if ( Input.Pressed( InputButton.Reload ) && !IsReloading )
			Reload();

		if ( CanPrimaryAttack() )
		{
			using ( LagCompensation() )
			{
				TimeSincePrimaryAttack = 0;
				AttackPrimary();
			}
		}

		if ( !Owner.IsValid() )
			return;

		if ( CanSecondaryAttack() )
		{
			using ( LagCompensation() )
			{
				TimeSinceSecondaryAttack = 0;
				AttackSecondary();
			}
		}

		if ( IsReloading && TimeSinceReload > ReloadTime )
		{
			OnReloadFinish();
		}
	}

	public virtual void OnReloadFinish()
	{
		IsReloading = false;

		if ( Owner is PlayerBase player )
		{
			var ammo = player.TakeAmmo( AmmoType, ClipSize - AmmoClip );
			if ( ammo == 0 )
				return;

			AmmoClip += ammo;
		}

		if( ViewModelEntity?.GetAnimParameterBool( "empty" ) != false )
			ViewModelEntity?.SetAnimParameter( "empty", false );
	}

	[ClientRpc]
	public virtual void StartReloadEffects()
	{
		ViewModelEntity?.SetAnimParameter( "reload", true );
	}

	public virtual void AttackPrimary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;
	}

	public virtual void AttackSecondary()
	{
		TimeSincePrimaryAttack = 0;
		TimeSinceSecondaryAttack = 0;
	}

	[ClientRpc]
	protected virtual void ShootEffects()
	{
		Host.AssertClient();

		Particles.Create( "particles/pistol_muzzleflash.vpcf", EffectEntity, "muzzle" );

		if ( IsLocalPawn )
		{
			new Sandbox.ScreenShake.Perlin();
		}

		ViewModelEntity?.SetAnimParameter( "fire", true );
		CrosshairPanel?.CreateEvent( "fire" );
	}

	public IEnumerable<TraceResult> BulletTrace( Vector3 start, Vector3 end, float radius = 2.0f )
	{
		bool InWater = Map.Physics.IsPointWater( start );

		var tr = Trace.Ray( start, end )
				.UseHitboxes()
				.HitLayer( CollisionLayer.Water, !InWater )
				.HitLayer( CollisionLayer.Debris )
				.Ignore( Owner )
				.Ignore( this )
				.Size( radius )
				.Run();

		yield return tr;
	}

	public virtual void ShootBullet( float spread, float force, float damage, float bulletSize, int bulletCount = 1 )
	{

		Rand.SetSeed( Time.Tick );

		for ( int i = 0; i < bulletCount; i++ )
		{
			var forward = Owner.EyeRotation.Forward;
			forward += (Vector3.Random + Vector3.Random + Vector3.Random + Vector3.Random) * spread * 0.25f;
			forward = forward.Normal;

			foreach ( var tr in BulletTrace( Owner.EyePosition, Owner.EyePosition + forward * 5000, bulletSize ) )
			{
				tr.Surface.DoBulletImpact( tr );

				if ( !IsServer ) continue;
				if ( !tr.Entity.IsValid() ) continue;

				var damageInfo = DamageInfo.FromBullet( tr.EndPosition, forward * 100 * force, damage )
					.UsingTraceResult( tr )
					.WithAttacker( Owner )
					.WithWeapon( this );

				tr.Entity.TakeDamage( damageInfo );
			}
		}
	}

	public bool TakeAmmo( int amount )
	{
		if ( AmmoClip < amount )
			return false;

		AmmoClip -= amount;
		return true;
	}

	[ClientRpc]
	public virtual void DryFire()
	{
		// CLICK
	}

	public override void CreateViewModel()
	{
		Host.AssertClient();
		
		ViewModelEntity = new RCViewmodel();

		ViewModelEntity.SetModel(ViewModelPath);

		ViewModelEntity.Position = Position;
		ViewModelEntity.Owner = Owner;
		ViewModelEntity.EnableViewmodelRendering = true;
	}

	public override void CreateHudElements()
	{
		if ( Local.Hud == null ) return;

		//CrosshairPanel = new Crosshair();
		//CrosshairPanel.Parent = Local.Hud;
		//CrosshairPanel.AddClass( ClassInfo.Name );
	}

	public bool IsUsable()
	{
		if ( AmmoClip > 0 ) return true;
		return AvailableAmmo() > 0;
	}

	public override void OnCarryStart( Entity carrier )
	{
		base.OnCarryStart( carrier );

		if ( PickupTrigger.IsValid() )
		{
			PickupTrigger.EnableTouch = false;
		}
	}

	public override void OnCarryDrop( Entity dropper )
	{
		base.OnCarryDrop( dropper );

		if ( PickupTrigger.IsValid() )
		{
			PickupTrigger.EnableTouch = true;
		}
	}

}
