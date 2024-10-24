using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float existTime = 3f;
    [SerializeField] float speed = 5f;
    
    // Rigidbody rb;
    
    // private void Awake() {
    //     rb = GetComponent<Rigidbody>();
    // }
    
    // Start is called before the first frame update
    // void Start()
    // {
        
    // }

    // Update is called once per frame
    void Update()
    {
        if (existTime<=0){
            Destroy(gameObject);
        }
        
        existTime -= Time.deltaTime;
    }
    
    public void Fire(){
        GetComponent<Rigidbody>().velocity = transform.forward*speed;
    }
    
    private void OnCollisionEnter(Collision other) {
        Destroy(gameObject);
    }
}
