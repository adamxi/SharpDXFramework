using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DXFramework.UI
{
	public class UIProgressBar : UIPanel
	{
		//private float progress;
		
		public UIProgressBar()
		{

		}

		private GrowthDirection Growth { get; set; }

		public enum GrowthDirection
		{
			LeftToRight,
			RightToLeft
		}
	}
}