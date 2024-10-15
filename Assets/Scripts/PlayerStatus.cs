using UnityEngine;
using System.Collections;

public class PlayerStatus : MonoBehaviour {

	public float health = 100.0f;
	public float maxHealth = 100.0f;
	private bool dead = false;
	private Animation animation;
	private PlayerController playerController;
	private float aiDamage = 0f;

	public void AddHealth(float moreHealth) {
		if (health < maxHealth)
			health += moreHealth;
		if (health > maxHealth)
			health = maxHealth;
	}
	public string GetDamage()
	{
		if(aiDamage == 0f)
        {
			return "";
        } 
		else
			return aiDamage.ToString() + " Damage";
	}
	public float GetHealth() {
		return health;
	}

	void Start()
	{
		playerController = GetComponent<PlayerController>();
		animation = GetComponent<Animation>();
	}

	public bool isAlive() { return !dead; }

	public void ApplyDamage(float damage) {
		health -= damage;
		aiDamage = damage;
		StartCoroutine(HideDamage());
		//Debug.Log("Ouch! " + health);
		if (health <= 0) {
			health = 0;
			animation.CrossFade("die", 0.1f);
			StartCoroutine(Die());
		}
	}

	IEnumerator HideDamage()
	{
		yield return new WaitForSeconds(2f);
		aiDamage = 0f;

	}
    
	IEnumerator  Die(){
		dead = true;
		print("Dead!");
		HideCharacter();
		yield return new WaitForSeconds(10);
		print("Alive!");
		//playerController.Respawn();
		ShowCharacter();
		health = maxHealth;
		dead = false;
	}
	
	void HideCharacter(){	
	

		playerController.IsControllable = false;



	}
	
	
	
	void ShowCharacter(){


		playerController.IsControllable = true;
	}
	
}
