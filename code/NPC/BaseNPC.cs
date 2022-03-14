using System;
using System.Linq;
using System.Collections.Generic;
using Sandbox;

public partial class BaseNPC : AnimEntity
{
	public virtual int BaseHealth => 1;
	public virtual float BaseSpeed => 1;
	public virtual bool IsFriendly { get; set; } = false;

	//Attacking
	public virtual float AlertRadius => 1;
	public virtual float AttackDamage { get; set; } = 1;
	public virtual float AttackRange => 1;
	public virtual float TimeToAttack => 1.0f;
	public virtual DamageFlags AttackType => DamageFlags.Generic;
	public virtual bool isRanged => false;

	public bool IsAttacking = false;

	//Sounds
	public virtual string AlertSound { get; set; } = "";
	public virtual string PainSound { get; set; } = "";
	public virtual string IdleSound { get; set; } = "";
	public virtual string DeathSound { get; set; } = "";

	//How long until this NPC loses the enemy (or player)
	public virtual int LoseEnemyTime => 1;

	//Team type
	public enum NPCTeamEnum
	{
		Netural,
		Undead,
		Rebel,
		Scientist,
		Military,
		Alien,
	}

	public virtual NPCTeamEnum NPC_Team => NPCTeamEnum.Netural;

	private TimeSince timeIdleSound;
	private int timeTillIdle;

	//When the NPC finds a player (Only applies if hostile)
	private PlayerBase targetPlayer;
	private TimeSince timeFoundPlayer;
	private bool isInPursuit;	

	public virtual float EyeSightRange => 1f;

	public NPCDebugDraw Draw => NPCDebugDraw.Once;

	private Vector3 InputVelocity;

	private Vector3 LookDir;

	private TimeSince timeLastAttacked;

	private DamageInfo lastDamage;

	private Sound curSound;

	private bool ignorePlayers = false;

	private static bool shouldIgnorePlayers = false;

	[ConVar.Replicated]
	public static bool rpg_nav_drawpath { get; set; }

	[ConVar.Replicated]
	public static bool rc_npc_range { get; set; }

	[AdminCmd( "rc_npc_clear" )]
	public static void ClearAllNPCs()
	{
		foreach ( var npc in All.OfType<BaseNPC>().ToArray() )
			npc.Delete();
	}

	[AdminCmd("rc_ignoreplayers")]
	public static void IgnorePlayers(bool shouldEnable)
	{
		shouldIgnorePlayers = shouldEnable;
	}

	NPCNavigation Path = new NPCNavigation();
	public NPCSteering Steer;

	public override void Spawn()
	{
		Health = BaseHealth;

		EnableLagCompensation = true;

		EyePosition = Position + Vector3.Up * 64;
		CollisionGroup = CollisionGroup.Player;

		SetupPhysicsFromModel(PhysicsMotionType.Static);

		EnableHitboxes = true;
		isInPursuit = false;

		timeIdleSound = 0;
		timeTillIdle = Rand.Int( 5, 60 );
		Steer = new NPCSteerWander();
	}

	[Event.Tick.Server]
	public void Tick()
	{
		InputVelocity = 0;

		if ( Steer != null )
		{
			Steer.Tick( Position );

			if ( !Steer.Output.Finished )
			{
				InputVelocity = Steer.Output.Direction.Normal;
				Velocity = Velocity.AddClamped( InputVelocity * Time.Delta * 500, BaseSpeed );
			}

			if ( rpg_nav_drawpath )
			{
				Steer.DebugDrawPath();
			}
		}

		Move( Time.Delta );

		var walkVelocity = Velocity.WithZ( 0 );
		
		if ( walkVelocity.Length > 0.5f )
		{
			var turnSpeed = walkVelocity.Length.LerpInverse( 0, 100, true );
			var targetRotation = Rotation.LookAt( walkVelocity.Normal, Vector3.Up );
			Rotation = Rotation.Lerp( Rotation, targetRotation, turnSpeed * Time.Delta * 20.0f );
		}

		var animHelper = new NPCAnimationHelper( this );

		LookDir = Vector3.Lerp( LookDir, InputVelocity.WithZ( 0 ) * 1000, Time.Delta * 100.0f );
		animHelper.WithLookAt( EyePosition + LookDir );
		animHelper.WithVelocity( Velocity );
		animHelper.WithWishVelocity( InputVelocity );

		ignorePlayers = shouldIgnorePlayers;

		if ( !IsFriendly )
		{
			
			var tr = Trace.Ray(EyePosition, EyePosition + Rotation.Forward * EyeSightRange )
				.Ignore( this )
				.Radius( 36.0f )
				.Run();

			if ( timeIdleSound >= timeTillIdle )
			{
				timeTillIdle = Rand.Int( 5, 60 );
				timeIdleSound = 0;

				using ( Prediction.Off() )
				{
					curSound = PlaySound( IdleSound );
				}
			}

			if ( tr.Entity is PlayerBase player )
			{
				if ( ignorePlayers )
					return;

				OnAlert();
				Steer = new NPCSteering();
				targetPlayer = player;
			}

			if ( timeFoundPlayer >= LoseEnemyTime && isInPursuit )
			{
				targetPlayer = null;
				isInPursuit = false;
				Steer = new NPCSteerWander();
			}

			if ( targetPlayer.IsValid() )
			{
				SetAnimLookAt( "aim_head", targetPlayer.Position + Vector3.Up * 64);
				Steer.Target = targetPlayer.Position;

				if( Position.Distance(targetPlayer.Position) <= AttackRange )
				{
					Steer.Target = Position;
					if( !IsAttacking )
					{
						timeLastAttacked = 0;
						IsAttacking = true;
					}

					if ( timeLastAttacked >= TimeToAttack && IsAttacking )
					{
						DamageInfo dmgInfo = new DamageInfo();
						dmgInfo.Damage = AttackDamage;
						dmgInfo.Attacker = this;
						dmgInfo.Flags = AttackType;

						targetPlayer.TakeDamage( dmgInfo );

						timeLastAttacked = 0;
					}
				} 
				else
				{
					IsAttacking = false;
				}
			}

			if ( rc_npc_range )
			{
				DebugOverlay.TraceResult( tr );
				DebugOverlay.Sphere( Position + Vector3.Up * 32, AttackRange, Color.Red, true );
			}
		}
	}

	private Vector3 DirectionAngle(float angleDegrees)
	{
		return new Vector3( MathF.Sin( angleDegrees * MathX.DegreeToRadian( angleDegrees ) ), 0, MathF.Cos( angleDegrees * MathX.DegreeToRadian( angleDegrees ) ) );
	}

	public virtual void OnAlert()
	{
		timeFoundPlayer = 0;

		if ( isInPursuit )
			return;

		isInPursuit = true;

		using ( Prediction.Off() )
		{
			curSound.Stop();
			curSound = PlaySound( AlertSound );
		}

	}

	protected virtual void Move( float timeDelta )
	{
		var bbox = BBox.FromHeightAndRadius( 64, 4 );

		MoveHelper move = new( Position, Velocity );
		move.MaxStandableAngle = 50;
		move.Trace = move.Trace.Ignore( this ).Size( bbox );

		if ( !Velocity.IsNearlyZero( 0.001f ) )
		{
			move.TryUnstuck();
			move.TryMoveWithStep( timeDelta, 30 );
		}

		var tr = move.TraceDirection( Vector3.Down * 10.0f );

		if ( move.IsFloor( tr ) )
		{
			GroundEntity = tr.Entity;

			if ( !tr.StartedSolid )
			{
				move.Position = tr.EndPosition;
			}

			if ( InputVelocity.Length > 0 )
			{
				var movement = move.Velocity.Dot( InputVelocity.Normal );
				move.Velocity = move.Velocity - movement * InputVelocity.Normal;
				move.ApplyFriction( tr.Surface.Friction * 10.0f, timeDelta );
				move.Velocity += movement * InputVelocity.Normal;

				NPCDebugDraw.Once.Line( tr.StartPosition, tr.EndPosition );

			}
			else
			{
				move.ApplyFriction( tr.Surface.Friction * 10.0f, timeDelta );
			}

			
		}
		else
		{
			GroundEntity = null;
			move.Velocity += Vector3.Down * 900 * timeDelta;
			NPCDebugDraw.Once.WithColor( Color.Red ).Circle( Position, Vector3.Up, 10.0f );
		}

		Position = move.Position;
		Velocity = move.Velocity;
	}

	public override void Simulate( Client cl )
	{
		base.Simulate( cl );

		Rotation = Input.Rotation;
		EyeRotation = Rotation;

		var maxSpeed = 500;

		Velocity += Input.Rotation * new Vector3( Input.Forward, Input.Left, Input.Up ) * maxSpeed * 5 * Time.Delta;
		if ( Velocity.Length > maxSpeed ) Velocity = Velocity.Normal * maxSpeed;

		Velocity = Velocity.Approach( 0, Time.Delta * maxSpeed * 3 );

		Position += Velocity * Time.Delta;

		EyeRotation = Rotation;
		EyePosition = Position;
	}

	public override void TakeDamage( DamageInfo info )
	{
		if ( IsFriendly )
			return;

		if ( !ignorePlayers )
			OnAlert();

		if(info.Attacker is PlayerBase player)
			targetPlayer = player;

		lastDamage = info;

		Health -= info.Damage;

		if ( Health > 0 )
		{
			using ( Prediction.Off() )
			{
				if(curSound.Finished)
					curSound = PlaySound( PainSound );
			}
		}

		if ( Health <= 0 )
			OnKilled();
	}

	public override void OnKilled()
	{
		EnableLagCompensation = false;
		SpawnNPCRagdoll( lastDamage.Force, lastDamage.HitboxIndex);

		using ( Prediction.Off() )
		{
			curSound.Stop();
			curSound = PlaySound( DeathSound );
		}

		base.OnKilled();
	}

	void SpawnNPCRagdoll( Vector3 force, int forceBone )
	{
		var ent = new ModelEntity();
		ent.Position = Position;
		ent.Rotation = Rotation;
		ent.MoveType = MoveType.Physics;
		ent.RenderColor = RenderColor;
		ent.UsePhysicsCollision = true;
		ent.SetInteractsAs( CollisionLayer.Debris );
		ent.SetInteractsWith( CollisionLayer.WORLD_GEOMETRY );
		ent.SetInteractsExclude( CollisionLayer.Player | CollisionLayer.Debris );

		ent.SetModel( GetModelName() );
		ent.CopyBonesFrom( this );
		ent.TakeDecalsFrom( this );
		ent.SetRagdollVelocityFrom( this );
		ent.DeleteAsync( 20.0f );

		// Copy the clothes over
		foreach ( var child in Children )
		{
			if ( child is ModelEntity e )
			{
				var clothing = new ModelEntity();
				clothing.Model = e.Model;
				clothing.SetParent( ent, true );
			}
		}

		ent.PhysicsGroup.AddVelocity( force );

		if ( forceBone >= 0 )
		{
			var body = ent.GetBonePhysicsBody( forceBone );
			if ( body != null )
			{
				body.ApplyForce( force * 1000 );
			}
			else
			{
				ent.PhysicsGroup.AddVelocity( force );
			}
		}
	}

	public override void FrameSimulate( Client cl )
	{
		base.FrameSimulate( cl );

		Rotation = Input.Rotation;
		EyeRotation = Rotation;
		Position += Velocity * Time.Delta;
	}
}
