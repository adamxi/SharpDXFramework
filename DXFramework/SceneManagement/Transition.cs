namespace DXFramework.SceneManagement
{
	/// <summary>
	/// Abstract class for defining transitions used between scenes.
	/// </summary>
	public abstract class Transition : Scene
	{
		public Transition() { }

		public TransitionState State { get; protected set; }

		public override void LoadContent()
		{
			State = TransitionState.Intro;
		}

		public override void UnloadContent()
		{
			SetDone();
		}

		public void SetDone()
		{
			State = TransitionState.Done;
		}

		public enum TransitionState
		{
			Intro,
			Halfway,
			Outro,
			Done,
		}
	}
}