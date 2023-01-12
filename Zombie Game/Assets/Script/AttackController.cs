using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackController : MonoBehaviour
{
    [SerializeField] Weapon currentWeapon;
    private Transform mainCamera;
    private Animator anim;
    private bool isAttacking = false;
    

    private void Awake()
    {
        mainCamera = GameObject.FindWithTag("CameraPoint").transform;
        anim = mainCamera.transform.GetChild(0).GetComponent<Animator>();

        if (currentWeapon != null)
        {
            SpawnWeapon();
        }
        
    }

    
    void Update()
    {
        Attack();
    }

    private void Attack()
    {
        if (Mouse.current.leftButton.isPressed && !isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }

    private void SpawnWeapon()
    {
        if (currentWeapon == null)
        {
            return;
        }

        currentWeapon.SpawnNewWeapon(mainCamera.transform.GetChild(0).GetChild(0),anim);   //Hande ulaþtýk iki defa çocuðuna bakarak.
    }

    public void EquipWeapon(Weapon weaponType)
    {
        if (currentWeapon != null)
        {
            currentWeapon.Drop();   //Eðer elimizde bir silah varsa weapon klasöründeki drop fonksiyonundan onu yok et.
        }

        currentWeapon = weaponType;
        SpawnWeapon();
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(currentWeapon.GetAttackRate);
        isAttacking = false;
    }

    public int GetDamage()
    {
        if (currentWeapon != null)
        {
            return currentWeapon.GetDamage;
        }
        return 0;
    }
}
