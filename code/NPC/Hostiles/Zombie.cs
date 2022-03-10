using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Zombie : BaseNPC
{
	public override int BaseHealth => 50;
	public override float BaseSpeed => 30;
	public override float AlertRadius => 162;
	public override float AttackCooldown => 4;
	public override float BaseAttackDMG => 25;
	public override float AttackRange => 42;
	public override DamageFlags AttackType => DamageFlags.Slash;
	public override bool isRanged => false;
	public override string AlertSound { get; set; } = "zombie_alert";
	public override string PainSound { get; set; } = "zombie_pain";
	public override string IdleSound { get; set; } = "zombie_idle";
	public override string DeathSound { get; set; } = "zombie_die";
	public override int minRndLevel => 3;
	public override int maxRndLevel => 7;
	public override int minXP => 5;
	public override int maxXP => 25;
	public override int LoseEnemyTime => 15;
	public override NPCTeamEnum NPC_Team => NPCTeamEnum.Undead;

	public override void Spawn()
	{
		base.Spawn();
	}
	public override void OnKilled()
	{
		base.OnKilled();
	}
}
