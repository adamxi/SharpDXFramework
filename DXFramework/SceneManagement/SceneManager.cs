using System;
using System.Collections.Generic;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace DXFramework.SceneManagement
{
	/// <summary>
	/// The SceneManager handles scenes and transitions.
	/// </summary>
	public class SceneManager : GameSystem
	{
		private static SceneManager instance;
		public static SceneManager Instance
		{
			get { return SceneManager.instance; }
		}

		private Dictionary<string, Scene> scenes;
		private Transition transition;
		private Scene next;
		private SpriteBatch spriteBatch;

		public SceneManager( Game game )
			: base( game )
		{
			instance = this;
			scenes = new Dictionary<string, Scene>();
			spriteBatch = new SpriteBatch( game.GraphicsDevice );
		}

		#region Properties
		public int ViewportWidth { get; private set; }

		public int ViewportHeight { get; private set; }

		/// <summary>
		/// Previous scene.
		/// </summary>
		public Scene Previous { get; private set; }

		/// <summary>
		/// Current scene.
		/// </summary>
		public Scene Current { get; private set; }

		/// <summary>
		/// Amount of scenes in the SceneManager.
		/// </summary>
		public int Count
		{
			get { return scenes.Count; }
		}

		public SpriteBatch SpriteBatch
		{
			get { return spriteBatch; }
		}

		public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
		#endregion

		#region Methods
		/// <summary>
		/// Adds a scene to the SceneManager.
		/// </summary>
		public void Add( Scene scene )
		{
			if( !scenes.ContainsKey( scene.Name ) )
			{
				scene.SceneManager = this;
				scenes.Add( scene.Name, scene );
			}
		}

		/// <summary>
		/// Removes a scene from the SceneManager.
		/// </summary>
		/// <param name="sceneName">Scene name.</param>
		public void Remove( string sceneName )
		{
			scenes.Remove( sceneName );
		}

		/// <summary>
		/// Gets a scene from the SceneManager.
		/// </summary>
		/// <param name="sceneName">Scene name.</param>
		public Scene Get( string sceneName )
		{
			Scene scene;
			if( scenes.TryGetValue( sceneName, out scene ) )
			{
				return scene;
			}

			return null;
		}

		/// <summary>
		/// Sets the current scene.
		/// </summary>
		/// <typeparam name="T">Scene type. If this type has not been added to the SceneManager, a new temporary instance is created.</typeparam>
		/// <param name="transition">Transition to use between current and next scene. Set to 'null' for no transition.</param>
		public void Set<T>( Transition transition = null ) where T : Scene
		{
			Scene scene;
			if( !scenes.TryGetValue( typeof( T ).Name, out scene ) )
			{
				scene = Activator.CreateInstance<T>();
			}

			Set( scene, transition );
		}

		/// <summary>
		/// Sets the current scene by name.
		/// </summary>
		/// <param name="sceneName">Name of scene to set.</param>
		/// <param name="transition">Transition to use between current and next scene. Set to 'null' for no transition.</param>
		public void Set( string sceneName, Transition transition = null )
		{
			Scene scene;
			if( scenes.TryGetValue( sceneName, out scene ) )
			{
				Set( scene, transition );
			}
			else
			{
				throw new Exception( "Scene '" + sceneName + "' has not been added to the SceneManager" );
			}
		}

		/// <summary>
		/// Sets the current scene.
		/// </summary>
		/// <param name="scene">Scene instance to set.</param>
		/// <param name="transition">Transition to use between current and next scene. Set to 'null' for no transition.</param>
		public void Set( Scene scene, Transition transition = null )
		{
			if( next != null && !next.Equals( scene ) ) // Makes sure that the next scene can only be set once.
			{
				return;
			}

			next = scene;
			if( transition != null )
			{
				this.transition = transition;
				transition.SceneManager = this;
				transition.PreLoad();
			}
			else
			{
				AdvanceScene();
			}
		}

		/// <summary>
		/// Advances the SceneManager to the next scene, and in the process handling
		/// unloading of previous scene and loading of next scene.
		/// </summary>
		public void AdvanceScene()
		{
			Previous = Current;
			Current = next;
			next = null;

			if( Current.SceneManager == null )
			{
				Current.SceneManager = this;
			}
			if( Previous != null )
			{
				Previous.PreUnload();
			}

			InputManager.ReleaseMouse();	// Prevents mouse presses from persisting through scenes.
			Current.PreLoad();

			// Check if a transition from a previous scene change is still active while advancing to the next scene.
			if( transition != null && transition.State == Transition.TransitionState.Outro )
			{
				transition.SetDone();	// If active, kill the current transition so it doesn't blend through to the next scene.
			}
		}

		internal bool IsTransitionDone()
		{
			return transition == null;
		}
		#endregion

		#region Logic
		public override void Initialize()
		{
			base.Initialize();

			GraphicsDeviceManager = Content.ServiceProvider.GetService( typeof( IGraphicsDeviceManager ) ) as GraphicsDeviceManager;
			ViewportWidth = GraphicsDeviceManager.PreferredBackBufferWidth;
			ViewportHeight = GraphicsDeviceManager.PreferredBackBufferHeight;

			foreach( Scene scene in scenes.Values )
			{
				scene.Initialize();
			}

			Enabled = true;
			Visible = true;
		}

		protected override void LoadContent()
		{
			base.LoadContent();
		}

		protected override void UnloadContent()
		{
			base.UnloadContent();
		}

		public override void Update( GameTime gameTime )
		{
			if( Current != null )
			{
				Current.Update( gameTime );
			}

			if( transition != null )
			{
				transition.Update( gameTime );

				if( transition.State == Transition.TransitionState.Done )
				{
					transition = null;
				}
			}
			base.Update( gameTime );
		}

		public override void Draw( GameTime gameTime )
		{
			base.Draw( gameTime );
			if( Current != null )
			{
				Current.Draw( gameTime );
			}

			if( transition != null )
			{
				transition.Draw( gameTime );
			}
		}
		#endregion
	}
}