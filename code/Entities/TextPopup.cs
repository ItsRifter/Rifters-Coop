
using System;
using Sandbox;

[Library( "rc_displaytext" )]
[Hammer.EntityTool( "Text Popup", "Rifters Co-Op", "Similar to game_text, displays a text to players HUD" )]
[Hammer.EditorSprite( "materials/editor/display_text.vmat" )]

public partial class TextPopup : Entity
{

	[Property( "DisplayText" ), Description("Message to display to user")]
	public string Text_To_Display { get; set; } = "";

	[Property( "DisplayToAll" ), Description( "Display to all users (will use activator of a trigger if disabled)" )]
	public bool Display_All { get; set; } = false;

	[Property( "TimeDuration" ), Description( "How long does this text last before fade" )]
	public double Time_Duration { get; set; } = 1.0f;

	[Property("TextXPos"), Description("Where this will display in X screen position")]
	public int Screen_X_Pos { get; set; } = 50;

	[Property( "TextYPos" ), Description( "Where this will display in Y screen position" )]
	public int Screen_Y_Pos { get; set; } = 50;

	[Input]
	public void DisplayText()
	{
		if ( Display_All )
		{
			RifterGame.Current.DisplayTextUser( To.Everyone, Text_To_Display, Time_Duration, Screen_X_Pos, Screen_Y_Pos );
		} 
		/*else if (!Display_All && activator != null)
		{
			foreach ( var client in Client.All )
			{
				if(activator == client && client.Pawn is PlayerBase)
					RifterGame.Current.DisplayTextUser( To.Single(client), Text_To_Display, Time_Duration, Screen_X_Pos, Screen_Y_Pos );
			}
		}*/
	}

}

