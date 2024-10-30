using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class Player : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] uint maxHealth = 10;
    public uint MaxHealth {get{return maxHealth;}}
    [SerializeField] uint currHealth = 10;
    public uint CurrentHealth {get{return currHealth;}}
    [SerializeField] float knockbackTime = 0.25f;
    [SerializeField] float knockbackForce = 3f;
    [Header("Weapon")]
    [SerializeField] float shotDelay = 0.4f;
    [SerializeField] float chargeTimeMax = 0.75f;
    [SerializeField] Transform aimObject;
    [SerializeField] Transform gunTip;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject bullet2;
    [Header("Ground Movement")]
    [SerializeField] float speedMultiplier = 1f;
    [SerializeField] float gravity = -30f;
    [SerializeField] float terminalVelocity = -20f;
    [SerializeField] float groundSpeed = 3f;
    [SerializeField] float dashTime = 0.25f;
    [SerializeField] float dashMultiplier = 2f;
    [SerializeField] float dashCooldownTime = 0.5f;
    [SerializeField] float jumpHeight = 0.5f;
    [Header("Flight")]
    [SerializeField] float airSpeed = 4.5f;
    [SerializeField] float liftSpeed = 3.5f;
    [SerializeField] float flightMaxStamina = 5f;
    public float FlightMaxStamina {get{return flightMaxStamina;}}
    [SerializeField] float flightStamina = 5f;
    public float FlightStamina {get{return flightStamina;}}
    [SerializeField] float staminaDepletion = 1f;
    [SerializeField] float staminaRegen = 3.75f;
    [SerializeField] float flightInputDelay = 0.14f;
    
    [Header("Test")]
    
    // Public Variables
    public Action KillPlayer;
    
    // Internal Variables
    bool isGrounded = true;
    bool inFlight = false;
    bool inLift = false;
    float horizontalVelocity = 0;
    float dashTimer = 0;
    float dashCooldownTimer = 0;
    float verticalVelocity = 0;
    float flightInputDelayTime = 0;
    bool damaged = false;
    float knockbackTimeLeft = 0;
    float shotDelayTimer = 0;
    
    float chargeTime = 0;
    
    // Internal Components
    SpriteRenderer playerSprite;
    CharacterController chaCon;
    PlayerInput playerInput;
    InputAction moveAction, mouseAction, shootAction, specialAction, jumpAction, dashAction, interactAction;
    CinemachineImpulseSource impulseSource;
    
    // Start is called before the first frame update
    void Start()
    {
        playerSprite = GetComponentInChildren<SpriteRenderer>();
        
        chaCon = GetComponent<CharacterController>();
        
        impulseSource = GetComponent<CinemachineImpulseSource>();

        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Movement");
        mouseAction = playerInput.actions.FindAction("Mouse");
        shootAction = playerInput.actions.FindAction("Shoot");
        specialAction = playerInput.actions.FindAction("Special");
        jumpAction = playerInput.actions.FindAction("Jump");
        dashAction = playerInput.actions.FindAction("Dash");
        interactAction = playerInput.actions.FindAction("Interact");
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        GunManager();
    }
    
    void Movement(){
        isGrounded = chaCon.isGrounded;
        
        // Damage knockback
        if(knockbackTimeLeft>0){
            knockbackTimeLeft -= Time.deltaTime;
        }
        else {
            // Horizontal Movement
            float adjustedSpeed = !inFlight ? groundSpeed : airSpeed;
            
            // Dash
            if (horizontalVelocity != 0 && dashAction.triggered && dashCooldownTimer<=0){
                dashTimer = dashTime;
                dashCooldownTimer = dashCooldownTime;
                horizontalVelocity *= dashMultiplier;
            }
            else if (dashTimer>0){
                dashTimer -= Time.deltaTime;
            }
            else {
                horizontalVelocity = moveAction.ReadValue<float>()*adjustedSpeed;
                dashCooldownTimer -= Time.deltaTime;
            }
            
            // Flip sprite
            if (horizontalVelocity < 0) {
                playerSprite.flipX = true;
            }
            else if (horizontalVelocity > 0) {
                playerSprite.flipX = false;
            }
            
            // Jump
            if (jumpAction.triggered && isGrounded){
                verticalVelocity = Mathf.Sqrt(Mathf.Abs(jumpHeight*2f*gravity));
            }
            // Flight
            else if (jumpAction.inProgress && flightStamina > 0 && !isGrounded){
                flightInputDelayTime += Time.deltaTime;
                if (flightInputDelayTime >= flightInputDelay) {
                    inFlight = true;
                    inLift = true;
                    verticalVelocity = liftSpeed;
                    flightStamina -= Time.deltaTime*staminaDepletion;
                }
            }
            // Flight but not lifting
            else if (!isGrounded) {
                // Reset vertical velocity immediately after stopping lift
                if (inLift) {
                    verticalVelocity = 0;
                }
                inLift = false;
            }
            // Reset + Regen
            else if (isGrounded && verticalVelocity <=0) {
                flightInputDelayTime = 0;
                verticalVelocity = -1f;
                flightStamina = Mathf.Min(flightStamina+staminaRegen*Time.deltaTime, flightMaxStamina);
                inFlight = false;
                inLift = false;
            }
        }
        
        // Gravity
        verticalVelocity += gravity * Time.deltaTime;
        verticalVelocity = Math.Max(verticalVelocity, terminalVelocity);

        chaCon.Move(new Vector3(horizontalVelocity, verticalVelocity, 0)*Time.deltaTime);
    }
    
    void GunManager(){
        // Aim Gun
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(mouseAction.ReadValue<Vector2>());
        
        if (Physics.Raycast(ray, out hit)){
            aimObject.LookAt(new Vector3(hit.point.x, hit.point.y, transform.position.z));
        }
        
        // Shoot
        if (shootAction.inProgress && shotDelayTimer<=0){
            GameObject newBullet = Instantiate(bullet, gunTip.position, aimObject.rotation);
            newBullet.GetComponent<Bullet>().Fire();
            shotDelayTimer = shotDelay;
            chargeTime = 0;
        }
        else {
            shotDelayTimer -= Time.deltaTime;
        }
        
        // Special
        if (specialAction.inProgress) {
            chargeTime += Time.deltaTime;
        }
        else {
            if (chargeTime >= chargeTimeMax) {
                GameObject newBullet = Instantiate(bullet2, gunTip.position, aimObject.rotation);
                newBullet.GetComponent<Bullet>().Fire();
            }
            chargeTime = 0;
        }
    }
    
    void Damage(Vector3 enemyPos){
        if (knockbackTimeLeft<=0){
            Debug.Log("ScreenShakedmg");
            impulseSource.GenerateImpulse();
            currHealth--;
            if (currHealth <= 0 && KillPlayer!=null){
                KillPlayer();
            }
            knockbackTimeLeft = knockbackTime;
            Vector2 temp = (transform.position-enemyPos).normalized;
            horizontalVelocity = temp.x*knockbackForce;
            verticalVelocity = temp.y*knockbackForce;
        }
    }
    
    public void Damage(uint damage, Vector3 enemyPos){
        if (knockbackTimeLeft<=0){
            Debug.Log("ScreenShakedmg");
            impulseSource.GenerateImpulse();
            currHealth -= damage;
            if (currHealth <= 0 && KillPlayer!=null){
                KillPlayer();
            }
            knockbackTimeLeft = knockbackTime;
            Vector2 temp = (transform.position-enemyPos).normalized;
            horizontalVelocity = temp.x*knockbackForce;
            verticalVelocity = temp.y*knockbackForce;
        }
    }
    
    // private void OnTriggerEnter(Collider other) {
    //     if (other.tag == "Enemy"){
    //         Damage(other.ClosestPoint(transform.position));
    //     }
    // }
}