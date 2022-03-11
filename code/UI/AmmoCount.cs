using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class AmmoCount : Panel
{
	public Panel AmmoPnl;
	public Label AmmoLbl;
	public Label ReserveLbl;

	public AmmoCount()
	{
		StyleSheet.Load( "UI/AmmoCount.scss" );

		AmmoLbl = Add.Label( "" );
		ReserveLbl = Add.Label( "" );
	}

	public override void Tick()
	{
		base.Tick();

		if( Local.Pawn is PlayerBase player )
		{
			if(player.ActiveChild is WepBaseCoop weapon)
			{
				if( weapon.IsMelee )
				{
					AmmoLbl.SetText( "" );
					ReserveLbl.SetText( "" );
				} 
				else
				{
					AmmoLbl.SetText( $"{weapon.AmmoClip}" );

					var inv = weapon.AvailableAmmo();
					ReserveLbl.Text = $" / {inv}";
					SetClass( "active", inv >= 0 );
				}
			}
		}
	}
}

