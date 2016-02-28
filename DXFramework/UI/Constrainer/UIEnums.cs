using System;

namespace DXFramework.UI
{
	[Flags]
	public enum Edge
	{
		None = 0,
		Left = 1,
		Right = 2,
		Top = 4,
		Bottom = 8,
		CenterX = 16,
		CenterY = 32,
		TopLeft = Top | Left,
		TopCenter = Top | CenterX,
		TopRight = Top | Right,
		BottomLeft = Bottom | Left,
		BottomCenter = Bottom | CenterX,
		BottomRight = Bottom | Right,
		LeftCenter = Left | CenterY,
		RightCenter = Right | CenterY,
		CenterXY = CenterX | CenterY,
		Horizontal = Left | Right,
		Vertical = Top | Bottom,
		Dock = Left | Right | Top | Bottom
	}

	[Flags]
	public enum ConstraintCategory
	{
		/// <summary>
		/// This constraint is by default enforced during layout updates.
		/// </summary>
		Update = 1,

		/// <summary>
		/// This constraint is by default enforced during layout initialization.
		/// </summary>
		Initialization = 2,

		/// <summary>
		/// This constraint is by default enforced on all layout events.
		/// </summary>
		All = Update | Initialization
	}

	public enum ScrollRestriction
	{
		/// <summary>
		/// Restrict scrolling to the horizontal axis.
		/// </summary>
		Horizontal = 0,

		/// <summary>
		/// Restrict scrolling to the vertical axis.
		/// </summary>
		Vertical = 1,

		/// <summary>
		/// Unrestricted scrolling on both horizontal and vertical axis.
		/// </summary>
		Unrestricted = 2
	}
}