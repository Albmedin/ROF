using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] uint maxHealth = 10;
    public uint MaxHealth {get{return maxHealth;}}
    [SerializeField] uint currHealth = 10;
    public uint CurrentHealth {get{return currHealth;}}
    [SerializeField] Image healthBar;
    [Header("Attack")]
    [SerializeField] uint bodyDamage = 1;
    [SerializeField] int warningTime = 1;
    
    // Internal Variables
    bool indicatedAttack = false;
    public Player player {get; private set;}
    public Animator animator {get; private set;}
    
    public void AbstractStart()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        animator = GetComponent<Animator>();
    }
    
    public void Damage(uint damage){
        currHealth -= damage;
        healthBar.fillAmount = (float)currHealth/maxHealth;
        animator.SetTrigger("Hit");
        if (currHealth <= 0){
            Destroy(gameObject);
        }
    }
    
    public void IndicateAttack(float timer){
        if (timer <= warningTime && !indicatedAttack){
            animator.SetTrigger("Warn");
            animator.SetFloat("Warn Time", warningTime);
            indicatedAttack = true;
        }
        else if (timer > warningTime){
            indicatedAttack = false;
        }
    }
    
    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Player") {
            player.Damage(bodyDamage, transform.position);
        }
    }
}
