﻿
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

public class InventoryColumn : Panel
{
	public int Column;
	public bool IsSelected;
	public Label Header;
	public int SelectedIndex;

	internal List<InventoryIcon> Icons = new();

	public InventoryColumn( int i, Panel parent )
	{
		StyleSheet.Load( "UI/inventoryBar.scss" );
		Parent = parent;
		Column = i;
		Header = Add.Label( $"{i+1}", "slot-number" );
	}

	internal void UpdateWeapon( WepBaseCoop weapon )
	{
		var icon = ChildrenOfType<InventoryIcon>().FirstOrDefault( x => x.Weapon == weapon );
		if ( icon == null )
		{
			icon = new InventoryIcon( weapon );
			icon.Parent = this;
			Icons.Add( icon );
		}
	}

	internal void TickSelection( WepBaseCoop selectedWeapon )
	{
		SetClass( "active", selectedWeapon?.Bucket == Column );

		for ( int i=0; i< Icons.Count; i++ )
		{
			Icons[i].TickSelection( selectedWeapon );
		}

	}
}
