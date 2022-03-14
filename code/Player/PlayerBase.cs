using Sandbox;
using System;
using System.Linq;


partial class PlayerBase : Player
{
	[Net] public int Armor { get; private set; }
	public bool SupressPickupNotices { get; private set; }

	private DamageInfo lastDMGInfo;

	public bool DidRespawn = false;

	//SoundScapes
	Sound soundScapePlaying;
	TimeSince timeSinceStep;
	string ssPath;

	Vector3 fallingVelocity;
	float fallDamage = 0;

	[ConVar.Replicated( "rc_debug" )]
	public static bool DebugMode { get; set; } = false;

	public PlayerBase()
	{
		Inventory = new Inventory(this);
	}

	public override void Respawn()
	{
		DidRespawn = true;
		base.Respawn();

		CameraMode = new FirstPersonCamera();
		Animator = new StandardPlayerAnimator();
		Controller = new WalkController();

		SetModel( "models/citizen/citizen.vmdl" );

		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		SupressPickupNotices = true;

		Inventory.DeleteContents();

		ApplyClothing();

		//Inventory.Add( new Glock(), true );
		//GiveAmmo( AmmoType.Pistol, 999 );

		holdBody = new PhysicsBody( Map.Physics )
		{
			BodyType = PhysicsBodyType.Keyframed
		};

		SupressPickupNotices = false;
	}

	public void ApplyClothing()
	{
		var hat = new ModelEntity();
		hat.SetModel( "models/citizen_clothes/hat/hat_woolly.vmdl_c" );
		hat.SetParent( this, true );
		
		var vest = new ModelEntity();
		vest.SetModel( "models/citizen_clothes/vest/Tactical_Vest/Models/tactical_vest.vmdl_c" );
		vest.SetParent( this, true );

		var shirt = new ModelEntity();
		shirt.SetModel( "models/citizen_clothes/jacket/jacket.red.vmdl_c" );
		shirt.SetParent( this, true );

		var trousers = new ModelEntity();
		trousers.SetModel( "models/citizen_clothes/jacket/longsleeve/models/jeans.vmdl_c" );
		trousers.SetParent( this, true );

		var boots = new ModelEntity();
		boots.SetModel( "models/citizen_clothes/shoes/shoes.workboots.vmdl_c" );
		boots.SetParent( this, true );

		hat.Tags.Add( "clothes" );
		vest.Tags.Add( "clothes" );
		shirt.Tags.Add( "clothes" );
		trousers.Tags.Add( "clothes" );
		boots.Tags.Add( "clothes" );

		hat.EnableHideInFirstPerson = true;
		vest.EnableHideInFirstPerson = true;
		shirt.EnableHideInFirstPerson = true;
		trousers.EnableHideInFirstPerson = true;
		boots.EnableHideInFirstPerson = true;
	}

	[AdminCmd("rc_impulse")]
	public static void ImpulseCMD(int impulseCMD)
	{
		Event.Run( "rc_evnt_impulse", impulseCMD);
	}

	[Event("rc_evnt_impulse")]
	public void Impulse(int impulse)
	{
		if ( impulse == 101 )
		{
			Inventory.Add( new Crowbar() );
			Inventory.Add( new Glock() );
			Inventory.Add( new ColtMagnum() );
			Inventory.Add( new SMG() );

			SetAmmo( AmmoType.Pistol, 150 );
			SetAmmo( AmmoType.Magnum, 12 );
			SetAmmo( AmmoType.SMG, 225 );
			SetAmmo( AmmoType.Buckshot, 30 );
			SetAmmo( AmmoType.Crossbow, 10 );
			SetAmmo( AmmoType.Grenades, 5 );
			SetAmmo( AmmoType.Rockets, 3 );
			SetAmmo( AmmoType.Misc, 50 );
		}
	}

	public override void TakeDamage( DamageInfo info )
	{
		if ( info.Attacker is PlayerBase )
			return;

		lastDMGInfo = info;

		base.TakeDamage( info );
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );
		SimulateActiveChild( cl, ActiveChild );

		TickPlayerUse();
		SimulateGrabbing();

		if ( Input.ActiveChild != null && !HeldBody.IsValid() )
		{
			ActiveChild = Input.ActiveChild;
		}

		if ( GroundEntity == null )
		{
			SimulateFalling();
		}
		else
		{
			if( fallDamage > 0 )
			{
				DamageInfo dmgInfo = new DamageInfo();
				dmgInfo.Damage = fallDamage;
				dmgInfo.Flags = DamageFlags.Fall;

				fallDamage = 0;

				if ( Health - fallDamage > 0 )
					PlaySound( "fall_pain" );
				else if (Health - fallDamage <= 0 )
					PlaySound( "fall_death" );

				TakeDamage( dmgInfo );
			}

			SimulateFootsteps();
		}
	}

	//Handles footstep sounds
	private void SimulateFootsteps()
	{
		var controlSpeed = Controller as WalkController;

		if ( controlSpeed.Duck.IsActive )
			return;

		//Standing still
		if ( controlSpeed.Velocity == new Vector3( 0, 0, 0 ) )
			return;

		if ( timeSinceStep < 1.0f )
			return;

		var footTR = Trace.Ray( EyePosition, EyePosition + Vector3.Down * 100 )
			.Ignore( this )
			.Run();

		if ( footTR.Surface.Name.Contains( "concrete" ) )
			PlaySound( "concrete_step" );
		
		if ( footTR.Surface.Name.Contains( "dirt" ) )
			PlaySound( "grass_step" );

		if ( footTR.Surface.Name.Contains( "wood" ) )
			PlaySound( "wood_step" );

		if ( footTR.Surface.Name.Contains( "metal" ) )
			PlaySound( "metal_step" );

		if ( footTR.Surface.Name.Contains( "plastic" ) )
			PlaySound( "tile_step" );

		if ( controlSpeed.GetWishSpeed() == controlSpeed.SprintSpeed )
			timeSinceStep = 0.75f;
		else if ( controlSpeed.GetWishSpeed() == controlSpeed.DefaultSpeed )
			timeSinceStep = 0.60f;
		else if ( controlSpeed.GetWishSpeed() == controlSpeed.WalkSpeed )
			timeSinceStep = 0.35f;
	}

	//Handles fall damage
	private void SimulateFalling()
	{
		fallingVelocity.z = Velocity.z;

		if(fallingVelocity.z < -650.0f)
		{
			fallDamage = fallingVelocity.z / 22.5f;
			fallDamage = -fallDamage;
		}
	}

	public override void StartTouch( Entity other )
	{
		//Checks if over the weapon ammo limit
		if(other is WepBaseCoop weapon )
		{
			if ( !weapon.IsMelee && AmmoCount( weapon.AmmoType ) >= AmmoLimit[(int)weapon.AmmoType] )
				return;
		}

		base.StartTouch( other );
	}

	public Sound GetSoundScapePlaying()
	{
		return soundScapePlaying;
	}

	public string GetSoundScapePath()
	{
		return ssPath;
	}

	[ClientRpc]
	public void SoundScapeClient( string sound )
	{
		if ( ssPath == sound )
			return;

		ssPath = sound;

		soundScapePlaying.Stop();
		soundScapePlaying = Sound.FromScreen( ssPath );
		soundScapePlaying.SetVolume( 0.25f );

		if ( DebugMode )
			Log.Info( "Playing Soundscape " + ssPath );
	}

	public void PlaySoundScape(string sound)
	{
		if ( IsClient )
			return;

		if ( sound == ssPath )
			return;

		ssPath = sound;

		soundScapePlaying.Stop();
		SoundScapeClient( To.Single( this ), ssPath );
	}

	public void SwitchToBestWeapon()
	{
		var best = Children.Select( x => x as WepBaseCoop )
			.Where( x => x.IsValid() && x.IsUsable() )
			.OrderByDescending( x => x.Bucket )
			.FirstOrDefault();

		if ( best == null ) return;

		ActiveChild = best;
	}

	float walkBob = 0;
	float lean = 0;
	float fov = 0;

	private void AddCameraEffects( ref CameraSetup setup )
	{
		var speed = Velocity.Length.LerpInverse( 0, 320 );
		var forwardspeed = Velocity.Normal.Dot( setup.Rotation.Forward );

		var left = setup.Rotation.Left;
		var up = setup.Rotation.Up;

		if ( GroundEntity != null )
		{
			walkBob += Time.Delta * 25.0f * speed;
		}

		setup.Position += up * MathF.Sin( walkBob ) * speed * 2;
		setup.Position += left * MathF.Sin( walkBob * 0.6f ) * speed * 1;

		// Camera lean
		lean = lean.LerpTo( Velocity.Dot( setup.Rotation.Right ) * 0.03f, Time.Delta * 15.0f );

		var appliedLean = lean;
		appliedLean += MathF.Sin( walkBob ) * speed * 0.2f;
		setup.Rotation *= Rotation.From( 0, 0, appliedLean );

		speed = (speed - 0.7f).Clamp( 0, 1 ) * 3.0f;

		fov = fov.LerpTo( speed * 20 * MathF.Abs( forwardspeed ), Time.Delta * 2.0f );

		setup.FieldOfView += fov;
	}
	public override void OnKilled()
	{
		base.OnKilled();

		GrabEnd();

		ActiveChild = null;
		EnableDrawing = false;

		BecomeRagdollOnClient( lastDMGInfo.Force, lastDMGInfo.HitboxIndex );
	}
}
