using UnityEngine;
using System.Collections;

public class StateRunAway : State
{

	public override void Execute(AIController character)
	{

		if (character.IsDead)
		{
			character.ChangeState(new StateDead());
		}
		else
        {
			character.BeRunAway();
        }


	}

}
