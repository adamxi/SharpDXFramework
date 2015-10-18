using System;
using System.Collections.Generic;
using SharpDX;
using System.Collections.ObjectModel;

namespace DXFramework.UI
{
	public static class UILayoutEngine
	{
		//internal static void UpdateSize( UIControl control, Vector2 delta )
		//{
		//	if( control.SuspendLayout )
		//	{
		//		return;
		//	}

		//	_UpdateSize( control, delta );

		//	if( control is UIContainer )
		//	{
		//		UIContainer container = control as UIContainer;
		//		foreach( UIControl child in container.Controls )
		//		{
		//			UpdateSize( child, delta );
		//		}
		//	}
		//	if( control.HasDecorations )
		//	{
		//		control.Decorations.ForEach( c => UpdateSize( c, delta ) );
		//	}
		//}

		//internal static void _UpdateSize( UIControl control, Vector2 delta )
		//{
		//	Edge anchors = control.Anchor;

		//	if( anchors.ContainsFlag( Edge.Vertical ) )
		//	{
		//		control.size.Y += delta.Y;
		//	}
		//	else if( anchors.ContainsFlag( Edge.Bottom ) )
		//	{
		//		control.drawPosition.Y += delta.Y;
		//	}

		//	if( anchors.ContainsFlag( Edge.Horizontal ) )
		//	{
		//		control.size.X += delta.X;
		//	}
		//	else if( anchors.ContainsFlag( Edge.Right ) )
		//	{
		//		control.drawPosition.X += delta.X;
		//	}

		//	if( anchors.ContainsFlag( Edge.CenterX ) )
		//	{
		//		control.size.X += delta.X;
		//		control.drawPosition.X -= delta.X / 2;
		//	}
		//	if( anchors.ContainsFlag( Edge.CenterY ) )
		//	{
		//		control.size.Y += delta.Y;
		//		control.drawPosition.Y -= delta.Y / 2;
		//	}
		//}



		//public static void Align( this UIControl control, Edge controlEdge, UIControl anchor, Edge anchorEdge, bool updateChildren = true )
		//{
		//	if( control.SuspendLayout )
		//	{
		//		return;
		//	}

		//	Vector2 pos = Vector2.Zero;
		//	foreach( Edge edgeType in edgeTypes )
		//	{
		//		if( anchorEdge.HasFlag( edgeType ) )
		//		{
		//			switch( edgeType )
		//			{
		//				case Edge.Right:
		//					pos.X += anchor.Width;
		//					break;
		//				case Edge.Bottom:
		//					pos.Y += anchor.Height;
		//					break;
		//				case Edge.CenterX:
		//					pos.X += anchor.HalfWidth;
		//					break;
		//				case Edge.CenterY:
		//					pos.Y += anchor.HalfHeight;
		//					break;
		//			}
		//		}
		//		if( controlEdge.HasFlag( edgeType ) )
		//		{
		//			switch( edgeType )
		//			{
		//				case Edge.Left:
		//					//pos.X += control.AbsOrigin.X;
		//					break;
		//				case Edge.Top:
		//					//pos.Y += control.AbsOrigin.Y;
		//					break;
		//				case Edge.Right:
		//					pos.X -= control.Width;
		//					break;
		//				case Edge.Bottom:
		//					pos.Y -= control.Height;
		//					break;
		//				case Edge.CenterX:
		//					pos.X -= control.HalfWidth;
		//					break;
		//				case Edge.CenterY:
		//					pos.Y -= control.HalfHeight;
		//					break;
		//			}
		//		}
		//	}

		//	if( anchor.Parent == control.Parent )			// Are controls in the same child collection? 
		//	{
		//		control.location = anchor.location + pos;	// Then position relativ to child anchor.
		//	}
		//	else
		//	{
		//		control.location = pos;
		//	}

		//	_UpdateDrawLocation( control );

		//	if( updateChildren && control is UIContainer )
		//	{
		//		UIContainer container = control as UIContainer;
		//		foreach( UIControl child in container.Controls )
		//		{
		//			_UpdateDrawLocation( child );
		//		}
		//	}
		//}

		




	}
}