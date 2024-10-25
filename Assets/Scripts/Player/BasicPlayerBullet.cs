using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPlayerBullet : Bullet
{
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Enemy"){
            other.GetComponent<Enemy>().Damage(Damage);
        }
        Destroy(gameObject);
    }
}
