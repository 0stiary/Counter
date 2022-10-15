using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test.Configuration
{
	public struct Thickness
	{
		public Thickness(double uniformLength)
		{
			Bottom = Left = Right = Top = uniformLength;
		}
		public Thickness(double left, double top, double right, double bottom)
		{
			Right = right;
			Top = top;
			Left = left;
			Bottom = bottom;
		}
		public double Bottom { get; set; }
		public double Left { get; set; }
		public double Right { get; set; }
		public double Top { get; set; }

	}
}
