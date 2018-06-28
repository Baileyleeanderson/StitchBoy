using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedState : IEnemyState {

	private Enemy enemy;

	private float throwTimer;
	private float throwCoolDown = 1f;
	private bool canThrow = true; 

	public void Enter(Enemy enemy)
	{
		this.enemy = enemy;
	}

	public void Execute() 
	{
		throwTimer += Time.deltaTime;
		if (throwTimer >= throwCoolDown)
		{
			AcidSpit(); /*the link called this ThrowKnife() */
			
		}
		if (enemy.InMeleeRange)
		{
			enemy.ChangeState(new MeleeState());	
		}
		else if (enemy.Target != null)
		{
			enemy.Move();
		}
		else 
		{
			enemy.ChangeState(new IdleState());
		}
	}
	
	public void Exit()
	{
	
	}
	public void OnTriggerEnter(Collider2D other)
	{
		
	}

	private void AcidSpit()
	{
		
		throwTimer += Time.deltaTime;
		if (throwTimer >= throwCoolDown)
		{
			canThrow = true;
			throwTimer = 0;
		}
		if (canThrow)
		{
			canThrow = false;
			enemy.MyAnimator.SetTrigger("attack"); /*links throw in animator is attack on mine */
			this.enemy.laser.Play();
			this.enemy.meleeWhoosh.Play();
		}
	}
}
