using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    [SerializeField] float existTime = 3f;
    [SerializeField] float speed = 5f;
    [SerializeField] uint damage = 1;
    public uint Damage {get{return damage;}}
    
    Rigidbody rb;
    
    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (existTime<=0){
            Destroy(gameObject);
        }
        
        existTime -= Time.deltaTime;
        
        transform.forward = rb.velocity;
    }
    
    public virtual void Fire(){
        rb.velocity = transform.forward*speed;
    }
}
