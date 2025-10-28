using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    public int selectedWeapon = 0;
    private PlayerController playerController;

    void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
    }

    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (selectedWeapon >= transform.childCount)
                selectedWeapon = 0;
            else
                selectedWeapon++;
            if (selectedWeapon > 1)
            {
                selectedWeapon = 1;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0f || Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (selectedWeapon <= 0)
                selectedWeapon = transform.childCount - 1;
            else
                selectedWeapon--;
        }


        if (selectedWeapon == 0)
        {
            playerController.currentWeaopn = PlayerController.Weapon.snowBall;
        }
        else if (selectedWeapon == 1)
        {
            playerController.currentWeaopn = PlayerController.Weapon.iceBall;
        }
    }

    
}
