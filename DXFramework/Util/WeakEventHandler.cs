using System.Reflection;

namespace System
{
	public delegate void UnregisterCallback<E>( EventHandler<E> eventHandler ) where E : EventArgs;

	public interface IWeakEventHandler<E> where E : EventArgs
	{
		EventHandler<E> Handler { get; }
		WeakReference Target { get; }
	}

	public class WeakEventHandler<T, E> : IWeakEventHandler<E>
		where T : class
		where E : EventArgs
	{
		private delegate void OpenEventHandler( T @this, object sender, E e );
		private WeakReference m_TargetRef;
		private OpenEventHandler m_OpenHandler;
		private EventHandler<E> m_Handler;
		private UnregisterCallback<E> m_Unregister;

		public WeakEventHandler( EventHandler<E> eventHandler, UnregisterCallback<E> unregister )
		{
			m_TargetRef = new WeakReference( eventHandler.Target );
			m_OpenHandler = Delegate.CreateDelegate( typeof( OpenEventHandler ), null, eventHandler.Method ) as OpenEventHandler;
			m_Handler = Invoke;
			m_Unregister = unregister;
		}

		public void Invoke( object sender, E e )
		{
			T target = (T)m_TargetRef.Target;

			if( target != null )
			{
				m_OpenHandler.Invoke( target, sender, e );
			}
			else if( m_Unregister != null )
			{
				m_Unregister( m_Handler );
				m_Unregister = null;
			}
		}

		public EventHandler<E> Handler
		{
			get { return m_Handler; }
		}

		public WeakReference Target
		{
			get { return m_TargetRef; }
		}

		public static implicit operator EventHandler<E>( WeakEventHandler<T, E> weh )
		{
			return weh.m_Handler;
		}
	}

	public static class EventHandlerUtils
	{
		public static EventHandler<E> MakeWeak<E>( this EventHandler<E> eventHandler, UnregisterCallback<E> unregister ) where E : EventArgs
		{
			if( eventHandler == null )
			{
				throw new ArgumentNullException( "eventHandler" );
			}
			if( eventHandler.Method.IsStatic || eventHandler.Target == null )
			{
				throw new ArgumentException( "Only instance methods are supported.", "eventHandler" );
			}
			// check to see if we're already weak
			if( eventHandler.Method.DeclaringType.IsGenericType && eventHandler.Method.DeclaringType.GetGenericTypeDefinition() == typeof( WeakEventHandler<,> ) )
			{
				return eventHandler;
			}

			Type wehType = typeof( WeakEventHandler<,> ).MakeGenericType( eventHandler.Method.DeclaringType, typeof( E ) );
			ConstructorInfo wehConstructor = wehType.GetConstructor( new Type[] { typeof( EventHandler<E> ), typeof( UnregisterCallback<E> ) } );
			IWeakEventHandler<E> weh = wehConstructor.Invoke( new object[] { eventHandler, unregister } ) as IWeakEventHandler<E>;

			return weh.Handler;
		}

		public static EventHandler<E> Unregister<E>( this EventHandler<E> sourceHandler, EventHandler<E> value ) where E : EventArgs
		{
			if( value == null )
			{
				throw new ArgumentNullException( "value" );
			}
			if( value.Method.IsStatic || value.Target == null )
			{
				throw new ArgumentException( "Only instance methods are supported.", "value" );
			}

			if( sourceHandler != null )
			{
				// look for the weak event handler in the invocation list
				foreach( EventHandler<E> evt in sourceHandler.GetInvocationList() )
				{
					IWeakEventHandler<E> weh = evt.Target as IWeakEventHandler<E>;
					if( weh != null )
					{
						object target = weh.Target.Target;
						if( target != null && ReferenceEquals( target, value.Target ) )
						{
							return weh.Handler;
						}
					}
				}
			}

			// return the input as the default if we don't find a wrapped event handler
			return value;
		}
	}
}