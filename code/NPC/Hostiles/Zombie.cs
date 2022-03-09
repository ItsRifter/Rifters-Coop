using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Zombie : BaseNPC
{
	public override int BaseHealth => 50;
	public override float BaseSpeed => 30;
	public override string BaseModel => "models/citizen/citizen.vmdl";
	public override float AlertRadius => 92;
	public override float AttackCooldown => 4;
	public override float BaseAttackDMG => 25;
	public override float AttackRange => 62;
	public override DamageFlags AttackType => DamageFlags.Slash;
	public override bool isRanged => false;
	public override string AlertSound => "zombie_alert";
	public override string PainSound => "zombie_pain";
	public override int minRndLevel => 3;
	public override int maxRndLevel => 7;
	public override int minXP => 5;
	public override int maxXP => 25;

	public override void Spawn()
	{
		base.Spawn();
		RenderColor = Color.Green;
	}
	public override void TakeDamage( DamageInfo info )
	{
		base.TakeDamage( info );

		PlaySound( PainSound );
	}

	public override void OnKilled()
	{
		base.OnKilled();
	}
}
