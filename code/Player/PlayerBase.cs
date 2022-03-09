using Sandbox;
using System;
using System.Linq;


partial class PlayerBase : Player
{
	[Net] public int Armor { get; private set; }
	public bool SupressPickupNotices { get; private set; }

	private DamageInfo lastDMGInfo;

	public PlayerBase()
	{
		Inventory = new Inventory(this);
	}

	public override void Respawn()
	{
		base.Respawn();

		CameraMode = new FirstPersonCamera();
		Animator = new StandardPlayerAnimator();
		Controller = new WalkController();

		SetModel( "models/citizen/citizen.vmdl" );

		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		SupressPickupNotices = true;

		Inventory.Add( new Glock19(), true );
		GiveAmmo( AmmoType.Pistol, 999 );

		SupressPickupNotices = false;
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
	}

	public override void OnKilled()
	{
		base.OnKilled();

		BecomeRagdollOnClient( lastDMGInfo.Force, lastDMGInfo.HitboxIndex );
	}
}
