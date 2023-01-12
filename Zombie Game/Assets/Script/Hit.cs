using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]

public class Hit : MonoBehaviour
{
    private Transform owner;
    private int damage;
    private Collider hitCollider;
    private Rigidbody rb;

    private Animator anim;

    private void Awake()
    {
        owner = transform.root;    //Root komutu hiyerarþide kendi grubunda en üstteki nesneye eriþir, yani playera.
        hitCollider = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        hitCollider.isTrigger = true;
        rb.useGravity = false;
        rb.isKinematic = true;
        hitCollider.enabled = false;
    }
    void Start()
    {
        if (owner.tag == "Player")
        {
           damage = owner.GetComponent<AttackController>().GetDamage();
           anim = GetComponentInParent<Transform>().GetComponentInParent<Animator>();
        }

        else if (owner.tag == "Enemy")
        {
           damage = owner.GetComponent<EnemyController>().GetDamage();
           anim = GetComponentInParent<Animator>();
        }

        else
        {
            enabled = false;
        }
    }

    private void Update()
    {
        //Bir animasyon klibinin içindeysek ve o animasyon klibinin adý horizontal attacksa, ve animasyonun .5 ve .55 saniyeleri arasýndaysak attacklarýmýz damage vuracak.
        if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.55f)          
        {
            ControlTheCollider(true);
            //print("Colldier açýk");
        }      

        else
        {
            ControlTheCollider(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();

        if (health != null && health.gameObject != owner.gameObject)   //Çarðtýðýmýz objede bir health scripti varsa ve çarptýðýmýz obje biz deðilsek çalýþýr.
        {
            health.GiveDamage(damage);         
        }
    }

    private void ControlTheCollider(bool open)   //Vuruþ anýnda collideri etkif etmek için yazdýk ki, düþmana deðdiði anda collider çalýþmasýn, sadece animasyon oynaraken aktif olsun.
    {
        hitCollider.enabled = open;
    }
}
