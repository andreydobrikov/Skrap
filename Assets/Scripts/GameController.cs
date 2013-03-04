using UnityEngine;
using System.Collections;
using AIFramework.StateMachine;
using CorruptedSmileStudio.Spawner;

public class GameController : MonoBehaviour 
{
	private StateMachine<GameController> _stateMachine;

	public StateMachine<GameController> StateMachine
	{
		get { return _stateMachine; }
	}

	public Spawner spawnController;

	void Start()
	{
		_stateMachine = new StateMachine<GameController>(this);
		StateMachine.SetCurrentStateAndFireEnter(GameStart.Instance);
	}

	public IEnumerator CountdownToGameStart()
	{
		yield return new WaitForSeconds(5);
		StateMachine.ChangeState(GameRunning.Instance);
	}
}
