using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Gun", fileName = "Gun")]
public class GunData : ScriptableObject
{
    [Header("Gun Stats")]
    public int damage;
    [Space]
    public float timeBetweenShooting;
    public float spread;
    public float range;
    public float reloadTime;
    public float timeBetweenShots;
    [Space]
    public int magazineSize;
    public int bulletsPerTap;
    [Space]
    public int bulletsLeft;
    public bool allowButtonHold;
    [HideInInspector]
    public int bulletsShot;
}
