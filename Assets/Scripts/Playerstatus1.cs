using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playerstatus1 : MonoBehaviour
{
    Animator animator;
    public float health = 100.0f;
    public float maxHealth = 100.0f;
    private bool dead = false;
    public PlayerMovement playerMov;
    private float aiDamage = 0f;
    public void AddHealth(float moreHealth)
    {
        if (health < maxHealth)
            health += moreHealth;
        if (health > maxHealth)
            health = maxHealth;
    }
    public float GetHealth()
    {
        return health;
    }
    public string GetDamage()
    {
        if (aiDamage == 0f)
        {
            return "";
        }
        else
            return aiDamage.ToString() + " Damage";
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        playerMov = GetComponent<PlayerMovement>();
    }
    public bool isAlive() { return !dead; }

    public void ApplyDamage(float damage)
    {
        health -= damage;
        aiDamage = damage;
        StartCoroutine(HideDamage());
        //Debug.Log("Ouch! " + health);
        if (health <= 0)
        {
            health = 0;
            animator.SetTrigger("Dead");
            StartCoroutine(Die());
        }
    }
    IEnumerator HideDamage()
    {
        yield return new WaitForSeconds(2f);
        aiDamage = 0f;

    }

    IEnumerator Die()
    {
        dead = true;
        print("Player Died!");
        yield return new WaitForSeconds(10);
        print("Alive!");
        //playerController.Respawn();
        health = maxHealth;
        dead = false;
    }

}

