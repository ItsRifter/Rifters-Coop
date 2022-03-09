using System;
using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
public partial class PickupFeed : Panel
{
	public static PickupFeed Current;

	public PickupFeed()
	{
		Current = this;
	}

	[ClientRpc]
	public static void OnPickup( string text )
	{
		Current?.AddEntry( $"\n{text}" );
	}

	private async Task AddEntry( string text )
	{
		var panel = Current.Add.Label( text );
		await Task.Delay( 500 );
		panel.Delete();
	}
}
