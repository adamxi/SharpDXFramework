using System;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;

namespace DXFramework.SceneManagement
{
	public abstract class Scene
	{
		protected Game game;
		protected SpriteBatch spriteBatch;
		protected GraphicsDevice graphicsDevice;
		protected ContentManager content;
		protected SceneManager sceneManager;
		protected DisposeCollector contentCollector;

		/// <summary>
		/// Creates a new Scene.
		/// </summary>
		/// <param name="name">Unique scene name. If 'null', the name will be set to the class name.</param>
		public Scene( string name = null )
		{
			UnloadResources = true;
			contentCollector = new DisposeCollector();
			Name = name == null ? this.GetType().Name : name;
		}

		#region Properties
		/// <summary>
		/// Gets the manager that this screen belongs to.
		/// </summary>
		public SceneManager SceneManager
		{
			get { return sceneManager; }
			internal set
			{
				sceneManager = value;
				game = value.Game;
				graphicsDevice = game.GraphicsDevice;
				content = game.Content;
				spriteBatch = value.SpriteBatch;
			}
		}

		public GraphicsDevice GraphicsDevice
		{
			get { return graphicsDevice; }
		}

		public ContentManager Content
		{
			get { return content; }
		}

		public SpriteBatch SpriteBatch
		{
			get { return spriteBatch; }
		}

		public Game Game
		{
			get { return game; }
		}

		/// <summary>
		/// True if the scene has been loaded.
		/// </summary>
		public bool Loaded { get; private set; }

		/// <summary>
		/// True if the scene will unload its resources once no longer active.
		/// </summary>
		public bool UnloadResources { get; set; }

		/// <summary>
		/// Scene name.
		/// </summary>
		public string Name { get; private set; }
		#endregion

		#region Methods
		/// <summary>
		/// Returns true if the current transition has finished.
		/// </summary>
		public bool IsTransitionDone()
		{
			return sceneManager.IsTransitionDone();
		}

		/// <summary>
		/// Adds an object to be disposed automatically when UnloadContent is called.
		/// Use this method for any content that is not loaded through the ContentManager.
		/// </summary>
		/// <typeparam name="T">Type of the object to dispose.</typeparam>
		/// <param name="disposable">The disposable object.</param>
		protected T ToDisposeContent<T>( T disposable ) where T : IDisposable
		{
			return contentCollector.Collect<T>( disposable );
		}
		#endregion

		#region Logic
		internal void PreLoad()
		{
			if( !Loaded )
			{
				LoadContent();
				Loaded = true;
			}
		}

		internal void PreUnload()
		{
			if( UnloadResources && Loaded )
			{
				UnloadContent();
				contentCollector.DisposeAndClear();
				Loaded = false;
			}
		}

		/// <summary>
		/// Called during SceneManager initialization.
		/// <see cref="Initialize"/> should not be used for loading content. Use <see cref="LoadContent"/> for this.
		/// </summary>
		public virtual void Initialize() { }

		/// <summary>
		/// Called when the scene is loaded.
		/// </summary>
		public virtual void LoadContent() { }

		/// <summary>
		/// Called when the scene is unloaded, just before next scene is loaded.
		/// </summary>
		public virtual void UnloadContent() { }

		public abstract void Update( GameTime gameTime );

		public abstract void Draw( GameTime gameTime );
		#endregion
	}
}