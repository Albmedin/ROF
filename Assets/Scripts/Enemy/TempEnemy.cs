using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempEnemy : Enemy
{
    // [Header("Attack")]
    [SerializeField] float shootDelay = 2;
    [SerializeField] GameObject bullet;
    
    // Internal Variables
    float shootTimer = 0;
    // Inherited Variables
    // Transform player;
    // Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        AbstractStart();
    }

    // Update is called once per frame
    void Update()
    {
        // transform.LookAt(new Vector3(player.transform.position.x, player.transform.position.y, 0));
        
        if (shootTimer <= 0){
            Vector3 bulletAim = (player.transform.position-transform.position).normalized;
            GameObject newBullet = Instantiate(bullet, transform.position+bulletAim, transform.rotation);
            newBullet.transform.forward = bulletAim;
            newBullet.GetComponent<EnemyBullet>().Fire();
            
            shootTimer = shootDelay;
        }
        IndicateAttack(shootTimer);
        shootTimer -= Time.deltaTime;
    }
}
