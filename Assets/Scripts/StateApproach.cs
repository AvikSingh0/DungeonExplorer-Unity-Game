using UnityEngine;
using System.Collections;

public class StateApproach : State
{
	public override void Execute(AIController character)
	{
		if (character.IsDead)
		{
			character.ChangeState(new StateDead());
		}
		else if(character.lowHealth())
		{
			character.ChangeState(new StateRunAway());
		}
		if (character.EnemySeen() && character.EnemyInRange() && character.EnemyAlive() == true)
		{
			character.ChangeState(new StateAttack());
		}
		else if (!character.EnemySeen() || character.EnemyAlive() == false)
		{
			character.ChangeState(new StateIdle());
		}

		else
		{
			character.BeApproaching();
		}
	}
}
