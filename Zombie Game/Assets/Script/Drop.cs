using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider))]
public class Drop : MonoBehaviour
{
    [SerializeField] Weapon weaponToDrop;
    [SerializeField] Vector3 Angle = Vector3.zero;

    private BoxCollider dropBox;

    private void Awake()
    {
        dropBox = GetComponent<BoxCollider>();
        dropBox.isTrigger = true;
        dropBox.size *= 3;
    }

    private void Start()
    {
        if (weaponToDrop != null)
        {
            Instantiate(weaponToDrop.GetWeaponPrefab, transform.position, transform.rotation, transform);
        }
    }

    private void Update()
    {
        transform.Rotate(Angle, Space.World);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                if (weaponToDrop != null)
                {
                    other.GetComponent<AttackController>().EquipWeapon(weaponToDrop);
                }

                Destroy(gameObject);
            }
        }
    }
}


