using System;
using System.Threading.Tasks;
using UnityEngine;

namespace KentaurusTech.StateMachine
{
	public class StateMachine
	{
		private enum eTransition
		{
			None,
			StartNewState,
			EndingOldState,
			CurrentState
		}

		private const int kMaxStates = 100;
		private ILogger _logger = null;
		private eTransition _eTransitionState = eTransition.None;
		private State[] _states = new State[kMaxStates];
		private int _currentDepth = -1;

		/// <summary>
		/// Constructor of the state machine
		/// </summary>
		/// <param name="firstState">First state</param>
		/// <param name="logger">Basic logger</param>
		public StateMachine(State firstState, ILogger logger = null)
		{
			_logger = logger;
			DoTransition(new Transition(firstState, eTransitionType.First));
		}

		/// <summary>
		/// Called in the update loop to update the state machine
		/// </summary>
		/// <param name="deltaTime">Delta time pased from the game</param>
		public void Update(float deltaTime)
		{
			if (_currentDepth > kMaxStates)
			{
				throw new Exception("Infinite loop detected");
			}

			if (_currentDepth == -1 ||
				_eTransitionState != eTransition.CurrentState)
			{
				return;
			}

			for (int i = _currentDepth; i >= 0; --i)
			{
				var state = _states[i];
				var transition = state.EvaluateTransition();

				if (transition == null)
				{
					if (i == _currentDepth)
					{
						state.Update(deltaTime);
					}
				}
				else
				{
					DoTransition(transition, i);
				}
			}

		}

		/// <summary>
		/// Perform the transition
		/// </summary>
		/// <param name="transition"></param>
		/// <param name="targetDepth"></param>
		private void DoTransition(Transition transition, int targetDepth = 0)
		{
			try
			{
				_ = Transition(transition, targetDepth);
			}
			catch (Exception e)
			{
				LogError(e);
			}
		}

		/// <summary>
		/// Switch function to determine if we transition to a sibling or child state
		/// </summary>
		/// <param name="transition">Transition information to which state we want to make the state</param>
		/// <param name="depth">At which depth of the stack we want to set the state</param>
		/// <returns>Nothing</returns>
		private async Task Transition(Transition transition, int depth)
		{
			switch (transition.TransitionType)
			{
				case eTransitionType.First: await FirstTransition(transition.State); break;
				case eTransitionType.Sibling: await SiblingTransition(transition.State, depth); break;
				case eTransitionType.Inner: await InnerTransition(transition.State); break;
				default: break;
			}
		}

		/// <summary>
		/// First transition when the state machine starts
		/// </summary>
		/// <param name="state">State of the which it starts with</param>
		/// <returns></returns>
		private async Task FirstTransition(State state)
		{
			_eTransitionState = eTransition.StartNewState;
			Log($"StateMachine {_eTransitionState} in state {state.GetType().Name}");
			await state.OnEnter();
			state.Init(null);
			_currentDepth = 0;
			_states[_currentDepth] = state;
			_eTransitionState = eTransition.CurrentState;
			Log($"StateMachine {_eTransitionState} in state {state.GetType().Name}");
		}

		/// <summary>
		/// Transitions into a sibiling node of the tree
		/// </summary>
		/// <param name="state"></param>
		/// <param name="depth"></param>
		/// <returns></returns>
		private async Task SiblingTransition(State state, int depth)
		{
			_eTransitionState = eTransition.StartNewState;
			Log($"StateMachine {_eTransitionState} in state {state.GetType().Name}");
			await state.OnEnter();

			if (_currentDepth > -1)
			{
				_eTransitionState = eTransition.EndingOldState;

				for (int i = _currentDepth; i >= depth; --i)
				{
					var oldState = _states[i];
					Log($"StateMachine {_eTransitionState} in state {oldState.GetType().Name}");
					await oldState.OnExit();
					_states[i] = null;
				}
			}

			var ownerIndex = depth - 1;
			state.Init(ownerIndex > 0 ? _states[ownerIndex] : null);
			_states[depth] = state;
			_currentDepth = depth;
			_eTransitionState = eTransition.CurrentState;
			Log($"StateMachine {_eTransitionState} in state {state.GetType().Name}");
		}

		/// <summary>
		/// This transitions into the child stack
		/// </summary>
		/// <param name="state">State you are going to</param>
		/// <returns>Returns nothing</returns>
		private async Task InnerTransition(State state)
		{
			_eTransitionState = eTransition.StartNewState;
			state.Init(_states[_currentDepth]);
			await state.OnEnter();
			_currentDepth += 1;
			_states[_currentDepth] = state;
			_eTransitionState = eTransition.CurrentState;
		}

		private void Log(object message)
		{
			_logger?.Log(message);
		}

		private void LogError(object message)
		{
			_logger?.LogError(nameof(StateMachine), message);
		}
	}
}