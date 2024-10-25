using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] List<GameObject> enemies = new List<GameObject>();
    
    Action action;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach(GameObject e in enemies){
            if (!e){
                action += () => {enemies.Remove(e);};
            }
        }
        
        if (action!=null){
            action();
            action = null;
        }
        
        if (enemies.Count <= 0){
            Destroy(gameObject);
        }
    }
}
