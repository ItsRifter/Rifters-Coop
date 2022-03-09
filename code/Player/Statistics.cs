using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
public partial class PlayerBase
{
	[Net] public int XP { get; private set; }

	public void GiveXP(int amt)
	{
		XP += amt;
	}
}

