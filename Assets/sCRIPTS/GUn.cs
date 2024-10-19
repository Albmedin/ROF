using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GUn : MonoBehaviour
{
    //Gun stats
    public int damage;
    public float TimeBetweenShooting, spread, range, ReloadTime, TimeBetweenShots;
    public int MagazineSize, BulletsPerTap;
    public bool AllowButtonHold;
    int BulletsLeft, BulletsShot;

    //bools
    bool shooting, ReadyToShoot, reloading;

    public Camera FpsCam;
    public Transform AttackPoint;
    public RaycastHit RayHit;
    public LayerMask WhatIsEnemy;

    private void Awake()
    {
        BulletsLeft = MagazineSize;
        ReadyToShoot = true;
    }

    private void Update()
    {
        MyInput();
    }

    private void MyInput()
    {
        if (AllowButtonHold) shooting = Input.GetKey(KeyCode.Mouse0);
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);

        if (Input.GetKeyDown(KeyCode.R) && BulletsLeft < MagazineSize && !reloading) Reload();

        //shoot
        if (ReadyToShoot && shooting && !reloading && BulletsLeft > 0)
        {
            BulletsShot = BulletsPerTap;
            Shoot();
        }
    }

    private void Shoot()
    {
        ReadyToShoot = false;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 direction = FpsCam.transform.forward + new Vector3(x, y, 0);


        if (Physics.Raycast(FpsCam.transform.position, FpsCam.transform.forward, out RayHit, range, WhatIsEnemy))
        {
            Debug.Log(RayHit.collider.name);

        
        }

        BulletsLeft--;
        BulletsLeft--;

        Invoke("ResetShot", TimeBetweenShooting);

        if(BulletsShot > 0 && BulletsLeft > 0)
        Invoke("Shoot", TimeBetweenShots);
    }

    private void ResetShot()
    {
        ReadyToShoot = true;

    }

    private void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", ReloadTime);
    }

    private void ReloadFinished()
    {
        BulletsLeft = MagazineSize;
        reloading = false;

    }
}
