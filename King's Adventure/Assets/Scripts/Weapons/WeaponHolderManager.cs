using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolderManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform[] weapons;

    [Header("Keys")]
    [SerializeField] KeyCode[] keys;

    [Header("Settings")]
    [SerializeField] float switchTime;

    int selectedWeapon;
    float timeSinceLastSwitch;


    // Start is called before the first frame update
    void Start()
    {
        SetWeapons();
        Select(selectedWeapon);

        timeSinceLastSwitch = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        for (int i = 0; i < keys.Length; i++)
        {
            if(Input.GetKeyDown(keys[i]) && timeSinceLastSwitch >= switchTime)
            {
                selectedWeapon = i;
            } 
        }

        if(previousSelectedWeapon != selectedWeapon)
        {
            Select(selectedWeapon);
        }

        timeSinceLastSwitch += Time.deltaTime;
    }

    void SetWeapons()
    {
        weapons = new Transform[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            weapons[i] = transform.GetChild(i);
        }

        if(keys == null)
        {
            keys = new KeyCode[weapons.Length];
        }
    }

    void Select(int weaponIndex)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == weaponIndex);
        }

        timeSinceLastSwitch = 0f;

        OnWeaponSelected();
    } 

    void OnWeaponSelected()
    {
        print("Selected new weapon...");
    }
}
