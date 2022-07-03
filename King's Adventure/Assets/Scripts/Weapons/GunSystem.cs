using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GunSystem : MonoBehaviour
{
    [Header("Gun Stats")]
    [SerializeField] GunData gunData;

    [Header("Aim Settings")]
    [SerializeField] Vector3 aimDownSight;
    [SerializeField] Vector3 hipFire;
    [SerializeField] float aimSpeed;

    // Bools 
    bool shooting, readyToShoot, reloading;
    public bool aiming = false;

    [Header("Controls")]
    public KeyCode shootKey = KeyCode.Mouse0;
    public KeyCode reloadKey = KeyCode.R;
    public KeyCode aimKey = KeyCode.Mouse1;

    [Header("Graphics")]
    public GameObject muzzleFlash;
    public GameObject bulletHole;
    public TMP_Text ammoText;

    [Header("References")]
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;  
    public GunRecoil recoilScript;


    private void Start()
    {
        gunData.bulletsLeft = gunData.magazineSize;
        readyToShoot = true;
    }

    private void Update()
    {
        MyInput();

        //SetText
        ammoText.SetText(gunData.bulletsLeft + " / " + gunData.magazineSize);    
    }

    void MyInput() 
    {
        //Input for our Shoot Button
        if (gunData.allowButtonHold)
        {
            shooting = Input.GetKey(shootKey);
        }    
        else
        {
            shooting = Input.GetKeyDown(shootKey);
        }
        
        //Input when reloading
        if(Input.GetKeyDown(reloadKey) && gunData.bulletsLeft < gunData.magazineSize && !reloading)
        {
            Reload();
        }

        //When ready to shoot
        if(readyToShoot && shooting && !reloading && gunData.bulletsLeft > 0)
        {
            gunData.bulletsShot = gunData.bulletsPerTap;
            Shoot();
        }

        //When aiming
        AimDown();
            
    }

    void Shoot()
    {
        readyToShoot = false;

        //Spread
        float x = Random.Range(-gunData.spread, gunData.spread);
        float y = Random.Range(-gunData.spread, gunData.spread);

        //Calculate Direction with Spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        //RayCast
        if(Physics.Raycast(fpsCam.transform.position, direction, out rayHit, gunData.range, whatIsEnemy))
        {
            Debug.Log(rayHit.collider.name);
            
            //Deals Damage to the Target
            rayHit.collider.GetComponent<Target>().TakeDamage(gunData.damage);
            
        }

        // Graphics
        GameObject bulletHoleGraphic = Instantiate(bulletHole, rayHit.point, Quaternion.FromToRotation(Vector3.forward, rayHit.normal));
        GameObject impactMuzzle = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);
        Destroy(impactMuzzle, 0.1f);
        Destroy(bulletHoleGraphic, 0.5f);

        // Recoil
        recoilScript.RecoilFire();

        gunData.bulletsLeft--;
        gunData.bulletsShot--;

        if(!IsInvoking("ResetShot") && !readyToShoot)
        {
            Invoke("ResetShot", gunData.timeBetweenShooting);
        }

        

        //To shoot more bullets at once
        if(gunData.bulletsShot > 0 && gunData.bulletsLeft > 0)
        {
            Invoke("Shoot", gunData.timeBetweenShots);
        }
    }

    void ResetShot()
    {
        readyToShoot = true;
    }

    void Reload()
    {
        reloading = true;
        Invoke("ReloadFinished", gunData.reloadTime);
    }

    void ReloadFinished()
    {
        gunData.bulletsLeft = gunData.magazineSize;
        reloading = false;
    }

    void AimDown()
    {
        if(Input.GetKey(aimKey))
        {
            aiming = true;
            transform.localPosition = Vector3.Lerp(transform.localPosition, aimDownSight, aimSpeed * Time.deltaTime);
        }
        else
        {
            aiming = false;
            transform.localPosition = Vector3.Lerp(transform.localPosition, hipFire, aimSpeed * Time.deltaTime);
        }
    }

}
