namespace KentaurusTech.StateMachine
{
	internal enum eTransitionType
	{
		None,
		First,
		Sibling,
		Inner
	}

	public class Transition
	{
		public const Transition kNone = null;

		internal State State { get; private set; }
		internal eTransitionType TransitionType { get; private set; }

		internal Transition(State state, eTransitionType transition)
		{
			State = state;
			TransitionType = transition;
		}

		public static Transition Sibling<T>() where T : State, new()
		{
			return new Transition(new T(), eTransitionType.Sibling);
		}

		public static Transition Inner<T>() where T : State, new()
		{
			return new Transition(new T(), eTransitionType.Inner);
		}
	}
}