using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] int maxHealth = 10;
    public int MaxHealth {get{return maxHealth;}}
    [SerializeField] int currHealth = 10;
    public int CurrentHealth {get{return currHealth;}}
    [SerializeField] Image healthBar;
    [Header("Attack")]
    [SerializeField] int bodyDamage = 1;
    public int BodyDamage {get{return bodyDamage;}}
    [SerializeField] float warningTime = 1f;
    
    // Internal Variables
    bool indicatedAttack = false;
    public Player player {get; private set;}
    public Animator animator {get; private set;}
    
    public void AbstractStart()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        animator = GetComponent<Animator>();
    }
    
    public virtual void Damage(int damage){
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
            animator.SetFloat("Warn Time", 1/warningTime);
            indicatedAttack = true;
        }
        else if (timer >= warningTime){
            indicatedAttack = false;
        }
    }
    
    public virtual void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Player") {
            player.Damage(bodyDamage, transform.position);
        }
    }
}
