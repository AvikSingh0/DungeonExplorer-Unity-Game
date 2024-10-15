using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Animator animator;
    public CharacterController controller;
    private Playerstatus1 status;
    private GameObject enemy;
    private bool isAttack = false;
    private bool canAttack = true;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        status = GetComponent<Playerstatus1>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        animator.SetFloat("Strafe", x);
        animator.SetFloat("Run_Strafe", x);
        animator.SetFloat("Forward", y);
        if (Input.GetMouseButtonDown(0))
        {
            if(canAttack == true)
            {
                Attack();
            }
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            animator.SetBool("Run_Condition", true);
        }

        else
        {
            animator.SetBool("Run_Condition", false);
        }
    }

    public void Attack()
    {
        canAttack = false;
        isAttack = true;
        animator.SetTrigger("Attack");
        StartCoroutine(AttackCooldown());

        
  


    }

    public bool checkAttack()
    {
        return isAttack;
    }


 
    IEnumerator AttackCooldown()
    {
        StartCoroutine(AttackEnding());
        yield return new WaitForSeconds(1.0f);
        canAttack = true;
    }
    IEnumerator AttackEnding()
    {
        yield return new WaitForSeconds(1.0f);
        isAttack = false;
    }





}


