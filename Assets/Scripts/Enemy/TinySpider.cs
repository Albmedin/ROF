using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TinySpider : Enemy
{
    enum EnemyState {Patrol, Alert, Slash, Charge, Attached};
    
    // [Header("Attack")]
    [Header("Slash")]
    [SerializeField] float slashCooldown = 1f;
    [SerializeField] float slashDelay = 1f;
    [SerializeField] float slashDistance = 3f;
    [SerializeField] GameObject quoteSwordEndquote;
    
    [Header("Charge")]
    [SerializeField] float chargeCooldown = 5f;
    [SerializeField] float chargingSpeed = 10f;
    [SerializeField] float chargeWindup = 1f;
    
    [Header("Movement")]
    [SerializeField] Vector2 patrolDistance = new Vector2(1f, 3f);
    [SerializeField] Vector2 delayBetweenPatrols = new Vector2(2f, 4f);
    [SerializeField] float walkingSpeed = 3f;
    
    [Header("Attached")]
    [SerializeField] bool damagePlayer = true;
    [SerializeField] float tickTime = 2f;
    [SerializeField] int tickDamage = 1;
    [SerializeField] float shakeDelay = 0.5f;
    
    [Header("Behavior")]
    [SerializeField] float actionDelay = 1f;
    [SerializeField] float detectionDistance = 10f;
    [SerializeField] float alertTime = 5f;
    [SerializeField] LayerMask rayCastLayers;
    [SerializeField] SpriteRenderer sprite;
    
    // Internal Variables
    EnemyState activeState = EnemyState.Patrol;
    float slashTimer = 0f;
    float patrolTimer = 0f;
    float alertTimer = 0f;
    float chargeTimer = 0f;
    float chargeSide = 0f;
    float actionTimer = 0f;
    float tickTimer = 0f;
    
    // Internal Components
    NavMeshAgent agent;
    new Collider collider;
    
    // Inherited Variables
    // Transform player;
    // Animator animator;
    
    // Start is called before the first frame update
    void Start()
    {
        AbstractStart();
        
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        collider = GetComponent<Collider>();
        
        PatrolInit();
    }

    // Update is called once per frame
    void Update()
    {
        if (slashTimer > 0){
            slashTimer -= Time.deltaTime;
        }
        
        if (Mathf.Abs(agent.velocity.x) > 0){
            sprite.transform.right = new Vector3(-agent.velocity.x, 0, 0);
        }
        
        switch (activeState){
            case EnemyState.Alert:
                AlertUpdate();
                return;
            case EnemyState.Slash:
                SlashUpdate();
                return;
            case EnemyState.Charge:
                ChargeUpdate();
                return;
            case EnemyState.Patrol:
                PatrolUpdate();
                return;
            case EnemyState.Attached:
                AttachedUpdate();
                return;
        }
    }
    
    void SwitchActiveState(EnemyState state){
        activeState = state;
        
        switch (activeState){
            case EnemyState.Alert:
                AlertInit();
                return;
            case EnemyState.Slash:
                SlashInit();
                return;
            case EnemyState.Charge:
                ChargeInit();
                return;
            case EnemyState.Patrol:
                PatrolInit();
                return;
            case EnemyState.Attached:
                AttachedInit();
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
        // patrolTimer = 0;
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
        // Check actions
        // Check if in range of player
        float playerDistance = Vector3.Distance(player.transform.position, transform.position);
        if (Vector3.Distance(player.transform.position, transform.position) < detectionDistance){
            Ray ray = new Ray(transform.position, player.transform.position-transform.position);
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
        // Else if can slash and next to player, slash
        else if (slashTimer <= 0 && playerDistance < slashDistance){
            SwitchActiveState(EnemyState.Slash);
            return;
        }
        // Otherwise
        // Move towards player
        else /* if (patrolTimer <= 0) */{
            Vector3 destination = transform.position;
            NavMeshHit hit;
            agent.FindClosestEdge(out hit);
            if (hit.distance < patrolDistance.x && hit.position.z == transform.position.z) {
                destination.x += Mathf.Sign(hit.position.x-transform.position.x)*-1*Random.Range(patrolDistance.x, patrolDistance.y);
            }
            else {
                destination.x = player.transform.position.x;
            }
            
            agent.SetDestination(destination);
            // patrolTimer = Random.Range(delayBetweenPatrols.x, delayBetweenPatrols.y);
        }
        
        // patrolTimer -= Time.deltaTime;
        alertTimer -= Time.deltaTime;
    }
    
    void SlashInit(){
        slashTimer = slashDelay;
        agent.speed = 0;
        IndicateAttack(999999f);
    }
    
    void SlashUpdate(){
        if (slashTimer <= 0){
            Instantiate(quoteSwordEndquote, transform.position+Mathf.Sign(player.transform.position.x-transform.position.x) * slashDistance * Vector3.right/2, transform.rotation);
            slashTimer = slashCooldown;
            SwitchActiveState(EnemyState.Alert);
        }
        IndicateAttack(slashTimer);
        slashTimer -= Time.deltaTime;
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
    
    void AttachedInit(){
        collider.enabled = false;
        agent.isStopped = true;
        agent.enabled = false;
        transform.position = player.transform.position; // Maybe swtich with animation?
        transform.parent = player.transform;
        player.DashPlayer += ShakeOff;
        tickTimer = tickTime;
    }
    
    void AttachedUpdate(){
        if (damagePlayer && tickTimer <= 0){
            player.Damage(tickDamage);
            tickTimer = tickTime;
        }
        
        tickTimer -= Time.deltaTime;
    }
    
    void ShakeOff(){
        StartCoroutine(ShakeOffRoutine());
    }
    
    IEnumerator ShakeOffRoutine(){
        transform.parent = null;
        transform.position = player.transform.position;
        tickTimer = 1000000f;
        yield return new WaitForSeconds(shakeDelay);
        collider.enabled = true;
        agent.enabled = true;
        agent.isStopped = false;
        agent.SetDestination(transform.position);
        SwitchActiveState(EnemyState.Alert);
    }
    
    public override void Damage(int damage){
        base.Damage(damage);
        if (activeState == EnemyState.Charge){
            SwitchActiveState(EnemyState.Alert);
        }
    }
    
    public override void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Player") {
            if (activeState == EnemyState.Charge){
                player.Damage(BodyDamage, damagePlayer ? 0 : 100, transform.position);
                SwitchActiveState(EnemyState.Attached);
            }
            else {
                player.Damage(BodyDamage, transform.position);
            }
        }
    }
    
    private void OnDestroy() {
        if (activeState == EnemyState.Attached){
            player.DashPlayer -= ShakeOff;
        }
    }
}
