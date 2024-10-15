using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    public PlayerMovement playerMov;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()

    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Debug.Log(other.name);
            if (playerMov.checkAttack() == true)
            {
                AIStatus enemystatus= GameObject.Find(other.name).GetComponent<AIStatus>();
                Debug.Log("HITTTTT: "+other.name);
                Debug.Log("Damage: " + 5);
                enemystatus.ApplyDamage(5);
            }
        }
    }
}

