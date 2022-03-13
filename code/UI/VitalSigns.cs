using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class VitalSigns : Panel
{
	public Panel VitalHud;
	public Panel HealthIcon;
	public Label HealthLbl;
	public Panel ArmorIcon;
	public Label ArmorLbl;

	public VitalSigns()
	{
		StyleSheet.Load( "UI/VitalSigns.scss" );

		VitalHud = Add.Panel( "vitalsigns" );
		
		HealthIcon = VitalHud.Add.Panel( "healthIcon" );
		HealthLbl = VitalHud.Add.Label( "", "health" );

		ArmorIcon = VitalHud.Add.Panel( "armorIcon" );
		ArmorLbl = VitalHud.Add.Label( "", "armor" );
	}

	public override void Tick()
	{
		base.Tick();

		if( Local.Pawn is PlayerBase player )
		{
			VitalHud.SetClass( "active", player.Health > 0 );
			HealthLbl.SetText( $"{Math.Round(player.Health)}" );

			ArmorIcon.SetClass( "active", player.Armor > 0 );
			ArmorLbl.SetClass( "active", player.Armor > 0 );

			ArmorLbl.SetText( $"{player.Armor}" );
		}
	}
}

