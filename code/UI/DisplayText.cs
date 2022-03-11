using System;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public partial class DisplayText : Panel
{
	public TimeSince timeUntilFade;
	public Panel textPanel;
	public Label textLbl;

	public DisplayText(string text, double duration, int xPos, int yPos)
	{
		StyleSheet.Load("UI/DisplayText.scss");

		textLbl = Add.Label();

		timeUntilFade = (float)-duration;
		textLbl.SetText( text );
		Style.Left = Length.Percent( xPos / 2 );
		Style.Top = Length.Percent( yPos );

	}

	public override void Tick()
	{
		base.Tick();

		SetClass( "active", timeUntilFade <= 0 );

		if( timeUntilFade >= 1.0f )
			Delete();
	}
}
