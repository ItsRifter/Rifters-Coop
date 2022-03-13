using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Zombie : BaseNPC
{
	public override string NPCModel => "models/npc/zombie/zombie.vmdl";
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

	private string[] randomTop = new string[] { 
		"models/citizen_clothes/jacket/jacket.red.vmdl_c", 
		"models/citizen_clothes/jacket/jacket_heavy.vmdl_c",
		"models/citizen_clothes/shirt/longsleeve_shirt/models/longsleeve_shirt.vmdl_c",

	};

	private string[] randomBottom = new string[] {
		"models/citizen_clothes/jacket/longsleeve/models/jeans.vmdl_c",
		"models/citizen_clothes/trousers/trousers_tracksuitblue.vmdl_c",
	};

	public override void Spawn()
	{
		base.Spawn();

		var top = new ModelEntity();
		top.SetModel(randomTop[Rand.Int( 0, randomTop.Count() - 1 ) ] );
		top.SetParent( this, true );

		var bottom = new ModelEntity();
		bottom.SetModel( randomBottom[Rand.Int( 0, randomBottom.Count() - 1 )] );
		bottom.SetParent( this, true );

	}
	public override void OnKilled()
	{
		base.OnKilled();
	}
}
