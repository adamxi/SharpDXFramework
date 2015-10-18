using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DXFramework.Util;

namespace DXFramework.UI
{
	public class UIControlCollection : IList<UIControl>, ICollection<UIControl>, IEnumerable<UIControl>, ICloneable
	{
		private UIControl owner;
		private const int defaultCapacity = 4;
		private static readonly UIControl[] emptyArray;
		private UIControl[] items;
		private int size;

		static UIControlCollection()
		{
			UIControlCollection.emptyArray = new UIControl[ 0 ];
		}

		public UIControlCollection( UIControl owner )
		{
			this.owner = owner;
			items = UIControlCollection.emptyArray;
		}

		public UIControlCollection( UIControl owner, IEnumerable<UIControl> collection )
		{
			this.owner = owner;
			if( collection == null )
			{
				throw new ArgumentNullException( "Collection" );
			}
			ICollection<UIControl> is2 = collection as ICollection<UIControl>;
			if( is2 != null )
			{
				int count = is2.Count;
				if( count == 0 )
				{
					items = UIControlCollection.emptyArray;
				}
				else
				{
					items = new UIControl[ count ];
					is2.CopyTo( items, 0 );
					size = count;
				}
			}
			else
			{
				size = 0;
				items = UIControlCollection.emptyArray;
				using( IEnumerator<UIControl> enumerator = collection.GetEnumerator() )
				{
					while( enumerator.MoveNext() )
					{
						Add( enumerator.Current );
					}
				}
			}
		}

		public UIControlCollection( UIControl owner, int capacity )
		{
			this.owner = owner;
			if( capacity < 0 )
			{
				throw new ArgumentOutOfRangeException( "Capacity" );
			}
			if( capacity == 0 )
			{
				items = UIControlCollection.emptyArray;
			}
			else
			{
				items = new UIControl[ capacity ];
			}
		}

		#region Properties
		public int Capacity
		{
			get { return items.Length; }
			set
			{
				if( value < size )
				{
					throw new ArgumentOutOfRangeException( "Capacity" );
				}
				if( value != items.Length )
				{
					if( value > 0 )
					{
						UIControl[] destinationArray = new UIControl[ value ];
						if( size > 0 )
						{
							Array.Copy( items, 0, destinationArray, 0, size );
						}
						items = destinationArray;
					}
					else
					{
						items = UIControlCollection.emptyArray;
					}
				}
			}
		}

		public bool IsFixedSize
		{
			get { return false; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public UIControl this[ int index ]
		{
			get { return items[ index ]; }
			set
			{
				items[ index ] = value;
			}
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public object SyncRoot
		{
			get { throw new NotImplementedException(); }
		}

		public int Count
		{
			get { return size; }
		}
		#endregion

		#region Methods
		public void Add( UIControl item )
		{
			if( size == items.Length )
			{
				EnsureCapacity( size + 1 );
			}
			items[ size++ ] = item;
			item.AssignParent( owner );
		}

		public void AddRange( IEnumerable<UIControl> collection )
		{
			InsertRange( size, collection );
		}

		public void Insert( int index, UIControl item )
		{
			if( index > size )
			{
				throw new ArgumentOutOfRangeException( "Index" );
			}
			if( size == items.Length )
			{
				EnsureCapacity( size + 1 );
			}
			if( index < size )
			{
				Array.Copy( items, index, items, index + 1, size - index );
			}
			items[ index ] = item;
			item.AssignParent( owner );
			size++;
		}

		public void InsertRange( int index, IEnumerable<UIControl> collection )
		{
			if( collection == null )
			{
				throw new ArgumentNullException( "Collection" );
			}
			if( index > size )
			{
				throw new ArgumentOutOfRangeException( "Index" );
			}

			ICollection<UIControl> col = collection as ICollection<UIControl>;
			if( col != null )
			{
				int count = col.Count;
				if( count > 0 )
				{
					EnsureCapacity( size + count );
					if( index < size )
					{
						Array.Copy( items, index, items, index + count, size - index );
					}
					if( this == col )
					{
						Array.Copy( items, 0, items, index, index );
						Array.Copy( items, index + count, items, index * 2, size - index );
					}
					else
					{
						UIControl[] array = new UIControl[ count ];
						col.CopyTo( array, 0 );
						array.CopyTo( items, index );
						foreach( UIControl item in col )
						{
							item.AssignParent( owner );
						}
					}
					size += count;
				}
			}
			else
			{
				using( IEnumerator<UIControl> enumerator = collection.GetEnumerator() )
				{
					while( enumerator.MoveNext() )
					{
						Insert( index++, enumerator.Current );
					}
				}
			}
		}

		public bool Remove( UIControl item )
		{
			int index = IndexOf( item );
			if( index >= 0 )
			{
				RemoveAt( index );
				return true;
			}
			return false;
		}

		public void RemoveAt( int index )
		{
			if( index >= size )
			{
				throw new ArgumentOutOfRangeException( "Index" );
			}
			size--;
			if( index < size )
			{
				Array.Copy( items, index + 1, items, index, size - index );
			}
			items[ size ] = null;
		}

		public void Clear()
		{
			if( size > 0 )
			{
				Array.Clear( items, 0, size );
				size = 0;
			}
		}

		public bool Contains( UIControl item )
		{
			for( int i = 0; i < size; i++ )
			{
				if( items[ i ].Equals( item ) )
				{
					return true;
				}
			}
			return false;
		}

		public int IndexOf( UIControl item )
		{
			return Array.IndexOf<UIControl>( items, item, 0, size );
		}

		public void CopyTo( UIControl[] array )
		{
			CopyTo( array, 0 );
		}

		public void CopyTo( UIControl[] array, int arrayIndex )
		{
			Array.Copy( items, 0, array, arrayIndex, size );
		}

		public void CopyTo( int index, UIControl[] array, int arrayIndex, int count )
		{
			Array.Copy( items, index, array, arrayIndex, count );
		}

		private void EnsureCapacity( int min )
		{
			if( items.Length < min )
			{
				int num = ( items.Length == 0 ) ? 4 : ( items.Length * 2 );
				if( num > 2146435071 )
				{
					num = 2146435071;
				}
				if( num < min )
				{
					num = min;
				}
				Capacity = num;
			}
		}

		public IEnumerator<UIControl> GetEnumerator()
		{
			for( int i = 0; i < size; i++ )
			{
				yield return items[ i ];
			}

			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public ReadOnlyCollection<UIControl> AsReadOnly()
		{
			return new ReadOnlyCollection<UIControl>( this );
		}

		public object Clone()
		{
			return Cloner.DeepClone( this );
		}
		#endregion
	}
}