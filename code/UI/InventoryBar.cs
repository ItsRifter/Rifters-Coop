using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;
using System.Linq;


public class InventoryBar : Panel
{
	List<InventoryColumn> columns = new();
	List<WepBaseCoop> Weapons = new();

	public bool IsOpen;
	WepBaseCoop SelectedWeapon;

	public InventoryBar()
	{
		StyleSheet.Load( "UI/InventoryBar.scss" );

		for ( int i = 0; i < 6; i++ )
		{
			var icon = new InventoryColumn( i, this );
			columns.Add( icon );
		}
	}

	public override void Tick()
	{
		base.Tick();

		SetClass( "active", IsOpen);

		var player = Local.Pawn as PlayerBase;
		if ( player == null ) return;

		Weapons.Clear();
		Weapons.AddRange( player.Children.Select( x => x as WepBaseCoop ).Where( x => x.IsValid() && x.IsUsable() ) );

		foreach ( var weapon in Weapons )
		{
			columns[weapon.Bucket].UpdateWeapon( weapon );
		}
	}

	/// <summary>
	/// IClientInput implementation, calls during the client input build.
	/// You can both read and write to input, to affect what happens down the line.
	/// </summary>
	[Event.BuildInput]
	public void ProcessClientInput( InputBuilder input )
	{
		bool wantOpen = IsOpen;
		var localPlayer = Local.Pawn as PlayerBase;

		if ( localPlayer.HeldBody.IsValid() )
			return;

		// If we're not open, maybe this input has something that will 
		// make us want to start being open?
		wantOpen = wantOpen || input.MouseWheel != 0;
		wantOpen = wantOpen || input.Pressed( InputButton.Slot1 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot2 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot3 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot4 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot5 );
		wantOpen = wantOpen || input.Pressed( InputButton.Slot6 );

		if ( Weapons.Count == 0 )
		{
			IsOpen = false;
			return;
		}

		if ( IsOpen != wantOpen )
		{
			SelectedWeapon = localPlayer?.ActiveChild as WepBaseCoop;

			if ( SelectedWeapon.IsValid() && SelectedWeapon.IsReloading )
				return;

			IsOpen = true;
		}

		if ( !IsOpen ) return;

		if ( input.Down( InputButton.Attack1 ) )
		{
			input.SuppressButton( InputButton.Attack1 );
			input.ActiveChild = SelectedWeapon;
			IsOpen = false;
			//Sound.FromScreen( "dm.ui_select" );
			return;
		}

		var oldSelected = SelectedWeapon;
		int SelectedIndex = Weapons.IndexOf( SelectedWeapon );
		SelectedIndex = SlotPressInput( input, SelectedIndex );

		SelectedIndex += input.MouseWheel;
		SelectedIndex = SelectedIndex.UnsignedMod( Weapons.Count );

		SelectedWeapon = Weapons[SelectedIndex];

		for ( int i = 0; i < 6; i++ )
		{
			columns[i].TickSelection( SelectedWeapon );
		}

		input.MouseWheel = 0;

		if ( oldSelected != SelectedWeapon )
		{
			//Sound.FromScreen( "dm.ui_tap" );
		}
	}

	int SlotPressInput( InputBuilder input, int SelectedIndex )
	{
		var columninput = -1;

		if ( input.Pressed( InputButton.Slot1 ) ) columninput = 0;
		if ( input.Pressed( InputButton.Slot2 ) ) columninput = 1;
		if ( input.Pressed( InputButton.Slot3 ) ) columninput = 2;
		if ( input.Pressed( InputButton.Slot4 ) ) columninput = 3;
		if ( input.Pressed( InputButton.Slot5 ) ) columninput = 4;
		if ( input.Pressed( InputButton.Slot6 ) ) columninput = 5;

		if ( columninput == -1 ) return SelectedIndex;

		if ( SelectedWeapon.IsValid() && SelectedWeapon.Bucket == columninput )
		{
			return NextInBucket();
		}

		// Are we already selecting a weapon with this column?
		var firstOfColumn = Weapons.Where( x => x.Bucket == columninput ).OrderBy( x => x.BucketWeight ).FirstOrDefault();
		if ( firstOfColumn == null )
		{
			// DOOP sound
			return SelectedIndex;
		}

		return Weapons.IndexOf( firstOfColumn );
	}

	int NextInBucket()
	{
		Assert.NotNull( SelectedWeapon );

		WepBaseCoop first = null;
		WepBaseCoop prev = null;

		foreach ( var weapon in Weapons.Where( x => x.Bucket == SelectedWeapon.Bucket ).OrderBy( x => x.BucketWeight ) )
		{
			if ( first == null ) first = weapon;
			if ( prev == SelectedWeapon ) return Weapons.IndexOf( weapon );
			prev = weapon;
		}

		return Weapons.IndexOf( first );
	}
}
