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
        owner = transform.root;    //Root komutu hiyerar�ide kendi grubunda en �stteki nesneye eri�ir, yani playera.
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
        //Bir animasyon klibinin i�indeysek ve o animasyon klibinin ad� horizontal attacksa, ve animasyonun .5 ve .55 saniyeleri aras�ndaysak attacklar�m�z damage vuracak.
        if (!anim.IsInTransition(0) && anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.55f)          
        {
            ControlTheCollider(true);
            //print("Colldier a��k");
        }      

        else
        {
            ControlTheCollider(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();

        if (health != null && health.gameObject != owner.gameObject)   //�ar�t���m�z objede bir health scripti varsa ve �arpt���m�z obje biz de�ilsek �al���r.
        {
            health.GiveDamage(damage);         
        }
    }

    private void ControlTheCollider(bool open)   //Vuru� an�nda collideri etkif etmek i�in yazd�k ki, d��mana de�di�i anda collider �al��mas�n, sadece animasyon oynaraken aktif olsun.
    {
        hitCollider.enabled = open;
    }
}
