using System;
using System.Linq;
using Sandbox;

partial class Inventory : BaseInventory
{

	public Inventory(PlayerBase player) : base (player)
	{

	}

	public override bool Add( Entity ent, bool makeActive = false )
	{
		var player = Owner as PlayerBase;
		var weapon = ent as WepBase;
		var notices = !player.SupressPickupNotices;
		//
		// We don't want to pick up the same weapon twice
		// But we'll take the ammo from it Winky Face
		//
		if ( weapon != null && IsCarryingType( ent.GetType() ) )
		{
			var ammo = weapon.AmmoClip;
			var ammoType = weapon.AmmoType;

			if ( ammo > 0 )
			{
				player.GiveAmmo( ammoType, ammo );

				if ( notices )
				{
					Sound.FromWorld( weapon.PickupSound, ent.Position );
					PickupFeed.OnPickup( To.Single( player ), $"+{ammo} {ammoType}" );
				}
			}

			// Despawn it
			ent.Delete();
			return false;
		}

		if ( weapon != null && notices )
		{
			Sound.FromWorld( weapon.PickupSound, ent.Position );
			PickupFeed.OnPickup( To.Single( player ), $"{ent.ClassInfo.Title}" );
		}

		return base.Add( ent, makeActive );
	}

	public bool IsCarryingType( Type t )
	{
		return List.Any( x => x.GetType() == t );
	}
}

