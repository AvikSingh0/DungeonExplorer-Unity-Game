using UnityEngine;
using System.Collections;

public class AIStatus : MonoBehaviour {
	
	public float	health = 20.0f;
	public float	damageVal = 5f;

	//Uncomment if we need to apply a power-up to the AI
	public float	maxHealth = 20.0f;

	private bool dead = false;
	private AIController aiController;
	
	void Start(){
		aiController = GetComponent< AIController>();
	}
	
	public bool isAlive() {return !dead;}	
	
	public void ApplyDamage(float damage){
		//Debug.Log("Enemy NPC damage " + damage);
		health -= damage;
		if (health <= 0 && !aiController.IsDead)
		{
			dead = true;
			health = 0;
			aiController.addPlayerHealth(10f);
			print("           +10 HP            ");
			print("***********Dead!*************");
			aiController.IsDead = true;

		}
	}
}
