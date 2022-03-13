using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Zombie : BaseNPC
{
	public override int BaseHealth => 50;
	public override float BaseSpeed => 30;
	public override float AlertRadius => 162;
	public override float TimeToAttack => 1.5f;
	public override float AttackDamage => 25;
	public override float AttackRange => 48;
	public override float EyeSightRange => 325.0f;
	public override DamageFlags AttackType => DamageFlags.Slash;
	public override bool isRanged => false;
	public override string AlertSound { get; set; } = "zombie_alert";
	public override string PainSound { get; set; } = "zombie_pain";
	public override string IdleSound { get; set; } = "zombie_idle";
	public override string DeathSound { get; set; } = "zombie_die";
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
