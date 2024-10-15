using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float attackDistance = 2.0f;
    public CharacterController controller;
    private PlayerStatus status;

    private float gravity = -9.81f;
    public float moveSpeed = 2;
    public float rotateSpeed = 10;
    private float jumpHeight = 1;
    Vector3 playerVelocity;
    Vector3 rotateDirection;
    float yVelocity = 0;
    private Animation animation;
    private bool isControllable = true;
    private GameObject[] enemies;
    private bool attacked = false;
    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        status = GetComponent<PlayerStatus>();
        animation = GetComponent<Animation>();

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log(enemies.Length);
    }

    public bool IsControllable
    {
        get { return isControllable; }
        set { isControllable = value; }
    }

    int FindClosest()
    {
        Transform target;
        float minDistance = 20000;
        int closest = -1;
        for (int i = 0; i < enemies.Length; i++)
        {
            AIStatus enemyStatus = enemies[i].GetComponent(typeof(AIStatus)) as AIStatus;
            if (!enemyStatus.isAlive())
                continue;
            target = enemies[i].transform;
            Vector3 toPlayer = target.position - transform.position;

            float dist = toPlayer.magnitude;

            toPlayer.y = 0;
            toPlayer = Vector3.Normalize(toPlayer);

            //Forward in world space
            Vector3 forward = transform.TransformDirection(new Vector3(0, 0, 1));
            forward.y = 0;
            forward = Vector3.Normalize(forward);

            if (dist <= attackDistance)
            {
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = i;
                }
            }

        }

        return closest;
    }

    void OnGUI()
    {
        GUI.Box(new Rect(Screen.width - 100, 0, 100, 50), " " + status.GetHealth().ToString()+"\n "+ status.GetDamage());
    }

    /* Update is called once per frame */
    void Update()
    {
        if (!isControllable)
        {
            return;
        }
        //Z is 1 if moving forward, -1 otherwise
        if (Input.GetAxis("Fire1") != 0)
        {
            animation.CrossFade("attack", 0.1f);
            AnimationState animState = animation["attack"];
            int closestEnemyInd = FindClosest();
            if (closestEnemyInd >= 0)
            {
                AIStatus status = enemies[closestEnemyInd].GetComponent<AIStatus>();
                if (animState.normalizedTime >= 0.3f && animState.normalizedTime <= 0.5f && attacked == false)
                {
                    Debug.Log("Damage: " + 5);
                    status.ApplyDamage(5);    
                    attacked = true;
                }


                if (animState.normalizedTime >= 0.5f)
                {
                    attacked = false;

                }
                
            }
            return;
        }

        playerVelocity = new Vector3(0, 0, Input.GetAxis("Vertical"));
        if (playerVelocity.magnitude == 0)
        {
            animation.CrossFade("idle", 0.1f);
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            animation.CrossFade("run",0.1f);
            playerVelocity = transform.TransformDirection(playerVelocity);

            //Scale by speed
            playerVelocity = playerVelocity * moveSpeed * 2;

        }
        else
        {
            animation.CrossFade("walk", 0.1f);
            //Transforms from local to world space
            playerVelocity = transform.TransformDirection(playerVelocity);

            //Scale by speed
            playerVelocity *= moveSpeed;
        }

        //Transforms from local to world space


        // Changes the height position of the player..
        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            Debug.Log("Jump");
            yVelocity = Mathf.Sqrt(jumpHeight * -2f * (gravity));
        }
        //Apply  gravity
        yVelocity += gravity * Time.deltaTime;

        playerVelocity.y = yVelocity;

        float moveHorz = Input.GetAxis("Horizontal");
        if (moveHorz > 0) //right turn - rotate clockwise, or about +Y
            rotateDirection = new Vector3(0, 1, 0);
        else if (moveHorz < 0) //left turn – rotate counter-clockwise, or about -Y
            rotateDirection = new Vector3(0, -1, 0);
        else
            rotateDirection = new Vector3(0, 0, 0);

        controller.transform.Rotate(rotateDirection, rotateSpeed * Time.deltaTime);
        CollisionFlags flags = controller.Move(playerVelocity * Time.deltaTime);
    }
}