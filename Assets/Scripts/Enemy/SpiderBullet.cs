using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderBullet : Bullet
{
    [SerializeField] int slowStacks = 1;
    
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            other.GetComponent<Player>().Damage(Damage, slowStacks, transform.position);
        }
        Destroy(gameObject);
    }
}
