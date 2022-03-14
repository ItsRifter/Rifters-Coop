using System;
using System.Collections.Generic;
using Sandbox;

partial class Nade : ModelEntity
{
	float Speed = 250.0f;

	public override void Spawn()
	{
		base.Spawn();

		SetModel( "models/weapons/grenade.vmdl" );
		SetupPhysicsFromModel( PhysicsMotionType.Dynamic );
	}


	[Event.Tick.Server]
	public virtual void Tick()
	{
		if ( !IsServer )
			return;
	}
}
