using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] Transform aimObject;
    [SerializeField] Transform gunTip;
    [SerializeField] GameObject bullet;
    [Header("Ground Movement")]
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float groundSpeed = 3f;
    [SerializeField] float dashTime = 0.25f;
    [SerializeField] float dashMultiplier = 2f;
    [SerializeField] float dashCooldownTime = 0.5f;
    [SerializeField] float jumpHeight = 5f;
    [SerializeField] Transform playerBottom;
    [SerializeField] float groundDistance = 0.1f;
    [SerializeField] LayerMask groundMask;
    [Header("Flight")]
    [SerializeField] float airSpeed = 1.5f;
    [SerializeField] float liftSpeed = 3f;
    [SerializeField] float flightMaxStamina = 5f;
    public float FlightMaxStamina {get{return flightMaxStamina;}}
    [SerializeField] float flightStamina = 5f;
    public float FlightStamina {get{return flightStamina;}}
    [SerializeField] float staminaDepletion = 1f;
    [SerializeField] float staminaRegen = 1f;
    [SerializeField] float flightInputDelay = 0.25f;
    
    [Header("Test")]
    
    // Public Variables
    public Action KillPlayer;
    
    // Internal Variables
    bool isGrounded = true;
    float horizontalVelocity = 0;
    float dashTimer = 0;
    float dashCooldownTimer = 0;
    float verticalVelocity = 0;
    float flightInputDelayTime = 0;
    bool damaged = false;
    float knockbackTimeLeft = 0;
    
    // Internal Components
    CharacterController chaCon;
    PlayerInput playerInput;
    InputAction moveAction, mouseAction, shootAction, jumpAction, dashAction, interactAction;
    
    // Start is called before the first frame update
    void Start()
    {
        chaCon = GetComponent<CharacterController>();
        
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindAction("Movement");
        mouseAction = playerInput.actions.FindAction("Mouse");
        shootAction = playerInput.actions.FindAction("Shoot");
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
        isGrounded = Physics.CheckSphere(playerBottom.position, groundDistance, groundMask);
        
        // Damage knockback
        if(knockbackTimeLeft>0){
            knockbackTimeLeft -= Time.deltaTime;
        }
        else {
            // Horizontal Movement
            float adjustedSpeed = isGrounded ? groundSpeed : airSpeed;
            
            // Dash
            if (dashAction.triggered && dashCooldownTimer<=0){
                dashTimer = dashTime;
                dashCooldownTimer = dashCooldownTime;
                horizontalVelocity *= dashMultiplier;
            }
            else if (dashTimer>0){
                dashTimer -= Time.deltaTime;
            }
            else {
                horizontalVelocity = moveAction.ReadValue<float>()*adjustedSpeed;
            }
            dashCooldownTimer -= Time.deltaTime;
            
            // Jump
            if (jumpAction.triggered && isGrounded){
                verticalVelocity = Mathf.Sqrt(Mathf.Abs(jumpHeight*2f*gravity));
            }
            // Flight
            else if (jumpAction.inProgress && flightStamina > 0){
                flightInputDelayTime += Time.deltaTime;
                if (flightInputDelayTime >= flightInputDelay){
                    verticalVelocity = liftSpeed;
                    flightStamina -= Time.deltaTime*staminaDepletion;
                }
            }
            // Reset + Regen
            else if (isGrounded) {
                flightInputDelayTime = 0;
                verticalVelocity = -1f;
                flightStamina = Mathf.Min(flightStamina+staminaRegen*Time.deltaTime, flightMaxStamina);
            }
        }
        
        // Gravity
        verticalVelocity += gravity*Time.deltaTime;
        
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
        if (shootAction.inProgress){
            GameObject newBullet = Instantiate(bullet, gunTip.position, aimObject.rotation);
            newBullet.GetComponent<Bullet>().Fire();
        }
    }
    
    void Damage(Vector3 enemyPos){
        if (knockbackTime<=0){
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
    
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Enemy"){
            Damage(other.ClosestPoint(transform.position));
        }
    }
}
