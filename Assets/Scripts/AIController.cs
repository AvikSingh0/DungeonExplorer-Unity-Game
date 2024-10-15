using UnityEngine;
using System.Collections;

public class AIController : MonoBehaviour {

	//public float			attackDistance = 2.0f;
	public float			sightRadius = 12f;
	public float			sightAngle = 120f;
	public float			walkRadius = 4f;
	public float			walkSpeed = 2f;
	public float			runSpeed = 3.3f;

	private float waitTime = 1.0f;
	private float timer = 0.0f;

	private float			attackRange = 1.3f;
	private float 			attackSpeed = 4.0f;
	private float 			gravity = 50.0f;
	private float			attackValue = 5.0f;
	//private float			attackTime = 1.166668f;
	private bool			attacked = false;

	private CharacterController controller;
	private AIStatus		aiStatus;
	private Playerstatus1	playerStatus;
	private Transform		target;
	private Vector3			moveDirection = new Vector3(0,0,0);
	private State			currentState;
	Animator animator;
	private bool canAttack = true;



	private bool			isControllable = true;
	private bool			isDead = false;

	//This is a hack for legacy animation - we will do this properly later
	private bool			deathStarted = false;

	//enum StateType { state_ApproachRun, state_ApproachWalk, state_Idle, state_Attack };
	//private StateType currentState = StateType.state_Idle;
	public bool 	IsControllable {
		get {return isControllable;}
		set {isControllable = value;}
	}

	public bool IsDead
	{
		get { return isDead; }
		set { isDead = value; }
	}
	
	// Use this for initialization
	void Start () {
		controller = GetComponent< CharacterController>();
		animator = GetComponent<Animator>();
		aiStatus = GetComponent<AIStatus>();
		GameObject tmp = GameObject.FindWithTag("Player");
		Debug.Log(tmp.name);
		if (tmp != null){
			target=tmp.transform;
			playerStatus = tmp.GetComponent<Playerstatus1>();
		}

		

		ChangeState(new StateIdle());
	}
	
	public void ChangeState(State newState){
		currentState = newState;
	}
	
	public void BeDead(){
		//This is a hack for legacy animation - we will do this properly later
		if (!deathStarted)
		{
			animator.SetTrigger("Die");
			deathStarted = true;
			CharacterController controller = GetComponent<CharacterController>();
			controller.enabled = false;
			timer += Time.deltaTime;
			if (timer >= waitTime)
			{
				// Reset the timer
				timer = 0.0f;

				gameObject.SetActive(false);
				this.IsControllable = false;
			}

	
			

		}
		moveDirection = new Vector3(0,0,0);

	
	}
	
	public void BeIdle(){
		animator.SetTrigger("idle");
		moveDirection = new Vector3(0,0,0);
	}

	public void BeAttacking()
    {
		if(canAttack == true)
        {
			animator.SetTrigger("Attack");
			Vector3 direction = target.position - transform.position;
			Quaternion targetRotation = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
			Debug.Log("got Hit");

			playerStatus.ApplyDamage(aiStatus.damageVal);
		}

		canAttack = false;

		StartCoroutine(AttackCooldown2());


	}

	public void BeApproaching()
    {
		Vector3 direction = target.position - transform.position;
		Quaternion targetRotation = Quaternion.LookRotation(direction);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

		Vector3 distanceFromPlayer = target.position - transform.position;
		if (distanceFromPlayer.magnitude <= walkRadius) 
        {

			animator.Play("Walk");


			transform.position = Vector3.MoveTowards(transform.position, target.position, walkSpeed * Time.deltaTime);

		}
        else
        {
			animator.Play("Walk");
			//animator.SetTrigger("walk"); ;
			transform.position = Vector3.MoveTowards(transform.position, target.position, runSpeed * Time.deltaTime);

		}

	}

	public void BeRunAway()
    {
		Vector3 direction = target.position - transform.position;


		if (direction.magnitude <= sightRadius)
		{
			Quaternion targetRotation = Quaternion.LookRotation(-direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
			//anim["Run"].speed = 1f;
			//anim.CrossFade("Run", 0.1f);
			Vector3 oppositeDir = transform.position - target.position;
			transform.position = Vector3.MoveTowards(transform.position, transform.position + oppositeDir, runSpeed * 0.8f * Time.deltaTime);
			Vector3 newPosition = new Vector3(transform.position.x, 0, transform.position.z);
			transform.position = newPosition;
		}
		else
        {
			Quaternion targetRotation = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
			//anim.CrossFade("Idle", 0.1f);
		}
	}

	public bool EnemySeen()
    {
		Vector3 distanceFromPlayer = target.position - transform.position;
		float angleToPlayer = Vector3.Angle(transform.forward, distanceFromPlayer);
		if (distanceFromPlayer.magnitude <= sightRadius  && angleToPlayer <= (sightAngle / 2f))
		{
			return true;
		}
		else
		{
			return false;
		}
	}
	public bool EnemyInRange()
    {
		Vector3 distanceFromPlayer = target.position - transform.position;
		if (distanceFromPlayer.magnitude <= attackRange)
        {
			return true;
        }
		else
        {
			return false;
        }

	}

	public bool EnemyAlive()
    {
		return playerStatus.isAlive();
    }

	public bool lowHealth()
    {
		if(aiStatus.health < aiStatus.maxHealth * 0.1)
        {
			return true;
        } else
        {
			return false;
        }
    }

	public void addPlayerHealth(float health)
    {
		playerStatus.AddHealth(health);
	}

	void Update () {
		
		if (!isControllable)
			return;
		if (IsDead)
		{
			BeDead();
		}
		else if (lowHealth())
		{
			BeRunAway();
		}
		else if (EnemySeen() && EnemyInRange() && EnemyAlive() == true)
		{
			BeAttacking();
			
		}
		else if (EnemySeen() && !EnemyInRange() && EnemyAlive() == true)
		{
			BeApproaching();
		}

		else if(!EnemySeen() && !EnemyInRange() && EnemyAlive() == true)
		{
			
			BeIdle();
		}
		StartCoroutine(AttackCooldown());
		//currentState.Execute(this);	
		//moveDirection.y -= gravity*Time.deltaTime;
		//controller.Move(moveDirection * Time.deltaTime);
	}

	void OnDisable()
	{
		/*
		 * If you uncomment this, you need to somehow tell the PlayerController to update
		 * the enemies array by calling GameObject.FindGameObjectsWithTag("Enemy").
		 * Otherwise the reference to a desgtroyed GameObejct will still be in the enemies 
		 * array and you will get null pointer exceptions if you try to access it
		 */

		//Destroy(gameObject);

	}
	IEnumerator AttackCooldown()
	{
		yield return new WaitForSeconds(1.0f);
	}


IEnumerator AttackCooldown2()
	{
		yield return new WaitForSeconds(1.0f);
		canAttack = true;

	}
}