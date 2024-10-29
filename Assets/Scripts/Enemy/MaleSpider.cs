using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class MaleSpider : Enemy
{
    enum EnemyState {Patrol, Alert, Shoot, Charge};
    
    // [Header("Attack")]
    [SerializeField] float chargeCooldown = 5f;
    [Header("Shoot")]
    [SerializeField] int shotCount = 3;
    [SerializeField] float delayBetweenShots = 1f;
    [SerializeField] float delayBeforeShooting = 1f;
    [SerializeField] float shootCooldown = 5f;
    [SerializeField] Bullet bullet;
    [SerializeField] Transform shotOrigin;
    [SerializeField] SpriteRenderer sprite;
    
    [Header("Movement")]
    [SerializeField] Vector2 patrolDistance = new Vector2(1f, 3f);
    [SerializeField] Vector2 delayBetweenPatrols = new Vector2(2f, 4f);
    [SerializeField] float walkingSpeed = 5f;
    [SerializeField] float chargingSpeed = 10f;
    
    [Header("Behavior")]
    [SerializeField] float actionDelay = 1f;
    [SerializeField] float detectionDistance = 10f;
    [SerializeField] float alertTime = 5f;
    [SerializeField] float chargeWindup = 1f;
    [SerializeField] LayerMask rayCastLayers;
    
    // Internal Variables
    EnemyState activeState = EnemyState.Patrol;
    float shootTimer = 0f;
    int shotsLeft = 0;
    float patrolTimer = 0f;
    float alertTimer = 0f;
    float chargeTimer = 0f;
    float chargeSide = 0f;
    float actionTimer = 0f;
    
    // Internal Components
    NavMeshAgent agent;
    
    // Inherited Variables
    // Transform player;
    // Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        AbstractStart();
        
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        
        PatrolInit();
    }

    // Update is called once per frame
    void Update()
    {
        if (shootTimer > 0){
            shootTimer -= Time.deltaTime;
        }
        
        if (chargeTimer > 0){
            chargeTimer -= Time.deltaTime;
        }
        
        if (Mathf.Abs(agent.velocity.x) > 0){
            sprite.transform.right = new Vector3(-agent.velocity.x, 0, 0);
        }
        
        switch (activeState){
            case EnemyState.Alert:
                AlertUpdate();
                return;
            case EnemyState.Charge:
                ChargeUpdate();
                return;
            case EnemyState.Shoot:
                ShootUpdate();
                return;
            case EnemyState.Patrol:
                PatrolUpdate();
                return;
        }
    }
    
    void SwitchActiveState(EnemyState state){
        activeState = state;
        
        switch (activeState){
            case EnemyState.Alert:
                AlertInit();
                return;
            case EnemyState.Charge:
                ChargeInit();
                return;
            case EnemyState.Shoot:
                ShootInit();
                return;
            case EnemyState.Patrol:
                PatrolInit();
                return;
        }
    }
    
    void PatrolInit(){
        agent.speed = walkingSpeed;
        patrolTimer = 0;
    }
    
    void PatrolUpdate(){
        // Patrol Movement
        if (patrolTimer <= 0){
            Vector3 destination = transform.position;
            NavMeshHit hit;
            agent.FindClosestEdge(out hit);
            if (hit.distance < patrolDistance.x && hit.position.z == transform.position.z) {
                destination.x += Mathf.Sign(hit.position.x-transform.position.x)*-1*Random.Range(patrolDistance.x, patrolDistance.y);
            }
            else {
                destination.x += Random.Range(patrolDistance.x, patrolDistance.y)*(Random.Range(0,2)*2-1);
            }
            
            agent.SetDestination(destination);
            patrolTimer = Random.Range(delayBetweenPatrols.x, delayBetweenPatrols.y);
        }
        
        // If stopped wait to move
        if (agent.velocity.magnitude < 0.1f){
            patrolTimer -= Time.deltaTime;
        }
        
        // Check if in range of player
        if (Vector3.Distance(player.transform.position, transform.position) < detectionDistance){
            Ray ray = new Ray(transform.position, player.transform.position-transform.position);
            RaycastHit hitData;
            if (Physics.Raycast(ray, out hitData, detectionDistance, rayCastLayers) && hitData.transform.tag == "Player"){
                SwitchActiveState(EnemyState.Alert);
            }
        }
    }
    
    void AlertInit(){
        agent.speed = walkingSpeed;
        patrolTimer = 0;
        alertTimer = alertTime;
        actionTimer = actionDelay;
    }
    
    void AlertUpdate(){
        // Return to normal if time over
        if (alertTimer <= 0){
            SwitchActiveState(EnemyState.Patrol);
            return;
        }
        if (actionTimer > 0){
            actionTimer -= Time.deltaTime;
            return;
        }
        // If stopped check actions
        if (agent.velocity.magnitude < 0.1f){
            // Check if in range of player
            if (Vector3.Distance(player.transform.position, transform.position) < detectionDistance){
                Ray ray = new Ray(player.transform.position, transform.position);
                RaycastHit hitData;
                if (Physics.Raycast(ray, out hitData, detectionDistance, rayCastLayers) && hitData.transform.tag == "Player"){
                    alertTimer = alertTime;
                }
            }
            // If can reach player, charge
            if (chargeTimer <= 0 && agent.CalculatePath(player.transform.position, new NavMeshPath())){
                SwitchActiveState(EnemyState.Charge);
                return;
            }
            // Else if can shoot, shoot
            else if (shootTimer <= 0){
                SwitchActiveState(EnemyState.Shoot);
                return;
            }
            // Otherwise
            // Alert Movement
            else if (patrolTimer <= 0){
                Vector3 destination = transform.position;
                NavMeshHit hit;
                agent.FindClosestEdge(out hit);
                if (hit.distance < patrolDistance.x && hit.position.z == transform.position.z) {
                    destination.x += Mathf.Sign(hit.position.x-transform.position.x)*-1*Random.Range(patrolDistance.x, patrolDistance.y);
                }
                else {
                    destination.x += Random.Range(patrolDistance.x, patrolDistance.y)*Mathf.Abs(player.transform.position.x-transform.position.x);
                }
                
                agent.SetDestination(destination);
                patrolTimer = Random.Range(delayBetweenPatrols.x, delayBetweenPatrols.y);
            }
            patrolTimer -= Time.deltaTime;
        }
        alertTimer -= Time.deltaTime;
    }
    
    void ShootInit(){
        shootTimer = delayBeforeShooting;
        shotsLeft = shotCount;
        StartCoroutine(DelayBeforeShooting());
    }
    
    void ShootUpdate(){
        if (shotsLeft<=0){
            shootTimer = shootCooldown;
            SwitchActiveState(EnemyState.Alert);
            return;
        }
        if (shootTimer <= 0){
            // Shoot
            Shoot();
            shootTimer = delayBetweenShots;
            shotsLeft--;
        }
        
        IndicateAttack(shootTimer);
    }
    
    IEnumerator DelayBeforeShooting(){
        yield return new WaitForSeconds(delayBeforeShooting);
        // sprite.flipX = player.transform.position.x-transform.position.x < 0;
        sprite.transform.right = new Vector3(player.transform.position.x-transform.position.x, 0, 0);
    }
    
    void Shoot(){
        // Shoot three shots
        sprite.transform.right = new Vector3(player.transform.position.x-transform.position.x, 0, 0);
        Vector3 dir = player.transform.position - shotOrigin.position; 
        float dist = new Vector3(dir.x, 0, 0).magnitude;
        float g = Physics.gravity.magnitude;
        float speed = bullet.Speed;
        
        if (dist > Mathf.Pow(speed,2)*Mathf.Sin(2*45*Mathf.Deg2Rad)/g){
            dir.y = dist;
        }
        else if (Mathf.Asin(dir.normalized.y)*Mathf.Rad2Deg < 45){
            float v = speed;
            float x = dir.x;
            float y = dir.y;
            
            float left = Mathf.Pow(v,2);
            float right = Mathf.Sqrt(Mathf.Pow(v, 4)-g*(g*Mathf.Pow(x,2)+2*y*Mathf.Pow(v,2)));
            
            float upper = Mathf.Atan((left+right)/(g*x));
            float lower = Mathf.Atan((left-right)/(g*x));
            
            if (!float.IsNaN(upper)||!float.IsNaN(lower)){
                float angle = !float.IsNaN(upper) ? upper : lower;
                angle = Mathf.Abs(angle);
                dir = new Vector3(Mathf.Cos(angle)*Mathf.Sign(dir.x), Mathf.Sin(angle), 0);
            }
        }
        
        GameObject newBullet = Instantiate(bullet.gameObject, shotOrigin.position, shotOrigin.rotation);
        newBullet.transform.forward = dir.normalized;
        newBullet.GetComponent<EnemyBullet>().Fire();
    }
    
    void ChargeInit(){
        agent.speed = chargingSpeed;
        chargeSide = Mathf.Sign(player.transform.position.x-transform.position.x);
        IndicateAttack(999999f);
        StartCoroutine(ChargeStart());
    }
    
    void ChargeUpdate(){
        if (Mathf.Sign(agent.destination.x-transform.position.x) != chargeSide && agent.velocity.magnitude < 0.1f){
            agent.autoBraking = true;
            agent.SetDestination(transform.position);
            chargeTimer = chargeCooldown;
            SwitchActiveState(EnemyState.Alert);
            return;
        }
    }
    
    IEnumerator ChargeStart(){
        chargeTimer = chargeWindup;
        while (chargeTimer > 0){
            chargeTimer -= Time.deltaTime;
            IndicateAttack(chargeTimer);
            yield return null;
        }
        agent.autoBraking = false;
        agent.SetDestination(player.transform.position);
    }
    
    public void SetDestination(){
        agent.SetDestination(shotOrigin.position);
    }
}