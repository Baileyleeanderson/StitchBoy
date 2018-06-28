using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IEnemyState {

	private Enemy enemy;

	private float patrolTimer;

	private float patrolDuration;
	
	public void Enter(Enemy enemy)
	{
		patrolDuration = UnityEngine.Random.Range(5,10);
		this.enemy = enemy;
	}

	public void Execute()
	{
		Patrol();

		enemy.Move();

		if (enemy.Target != null && enemy.InShootRange) /*video called it InThrowRange */
		{
			enemy.ChangeState(new RangedState());
		}
	}
	
	public void Exit()
	{
	
	}
	public void OnTriggerEnter(Collider2D other)
	{
		if (other.tag == "Edge")
		{
			enemy.ChangeDirection();
			
		}
	}
	private void Patrol()
	{

		patrolTimer += Time.deltaTime;

		if (patrolTimer >= patrolDuration)
		{
			enemy.ChangeState(new IdleState());
		}
	}
}
