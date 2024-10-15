using UnityEngine;
using System.Collections;

public class StateAttack : State
{
	public override void Execute(AIController character)
	{
		 if (character.IsDead)
		{
			character.ChangeState(new StateDead());
		}
		else if (character.lowHealth())
		{
			character.ChangeState(new StateRunAway());
		}
		else if (character.EnemySeen() && !character.EnemyInRange() && character.EnemyAlive() == true)
		{
			character.ChangeState(new StateApproach());
		}
		else if (!character.EnemySeen() || character.EnemyAlive() == false)
		{
			character.ChangeState(new StateIdle());
		}

		else
		{
			character.BeAttacking() ;
		}
	}
}
