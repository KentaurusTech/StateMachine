using System.Threading.Tasks;

namespace KentaurusTech.StateMachine
{
	public abstract class State
	{
		protected State Owner { get; private set; }

		/// <summary>
		/// Init state
		/// </summary>
		/// <param name="state"></param>
		internal void Init(State state)
		{
			Owner = state;
		}

		/// <summary>
		/// Enter state
		/// </summary>
		/// <returns></returns>
		public virtual Task OnEnter()
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Exit state
		/// </summary>
		/// <returns></returns>
		public virtual Task OnExit()
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Called in the update loop to update the state machine
		/// </summary>
		/// <param name="deltaTime"></param>
		public virtual void Update(float deltaTime)
		{
			// STUB
		}

		/// <summary>
		/// Evaluate the next transition
		/// </summary>
		/// <returns></returns>
		public virtual Transition EvaluateTransition()
		{
			return Transition.kNone;
		}
	}
}