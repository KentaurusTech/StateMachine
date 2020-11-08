using KentaurusTech.StateMachine;
using UnityEngine;

public class StateMachineTest : MonoBehaviour
{
	private StateMachine _stateMachine;

	private void Awake()
	{
		_stateMachine = new StateMachine(new State1(), Debug.unityLogger);
	}

	private void Update()
	{
		_stateMachine.Update(Time.deltaTime);
	}
}
