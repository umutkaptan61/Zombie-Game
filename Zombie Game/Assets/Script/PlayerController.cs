using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Player Control Settings")]
    [SerializeField] private float walkSpeed = 8f;
    [SerializeField] private float runSpeed = 12f;
    [SerializeField] private float gravityModifier = 0.95f;
    [SerializeField] private float jumpPower = 0.25f;

    [SerializeField] private InputAction newMovementInput;  //Yeni �nput sistemini kullanmak i�in referans ald�k.

    [Header("Mouse Control Options")]
    [SerializeField] float mouseSensivity = 1f;
    [SerializeField] float maxWievAngle = 60f;
    [SerializeField] bool invertX;
    [SerializeField] bool invertY;

    [Header("Sound Settings")]
    [SerializeField] List<AudioClip> footStepSounds = new List<AudioClip>();
    [SerializeField] AudioClip jumpSound;
    [SerializeField] AudioClip landSound;

    private CharacterController characterController;

    private float currentSpeed = 8f;
    private float horizontalInput;
    private float verticalInput;

    private Vector3 heightMovement;

    private bool jump = false;

    private Transform mainCamera;

    private Animator anim;

    private AudioSource audioSource;

    private int lastIndex = -1;   //Sesleri oynatmak i�in bir de�i�ken

    private bool landSoundPlayed = true;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (Camera.main.GetComponent<CameraController>() == null)    //E�er ana kameram�zda CameraController scripti yoksa, scripti bulup onu ekleyecek. G�venlik �nlemi olarak koyduk.
        {
            Camera.main.gameObject.AddComponent<CameraController>();
        }

        mainCamera = GameObject.FindWithTag("CameraPoint").transform;
    }

    private void OnEnable()    //Obje aktif olunca �al���r.
    {
        newMovementInput.Enable();
    }

    private void OnDisable()   //Obje pasif olunca �al���r.
    {
        newMovementInput.Disable();
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        KeyboardInput();   //Ctrl ve nokta tu�una beraber basarsak buraya yazd���m�z fonksiyon ad�nda bir fonksiyon olu�turur.

        AnimationChanger();

        //Move();

        Rotate();

    }

    private void FixedUpdate()
    {
        Move();

        //Rotate();
    }
    
    private void Rotate()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + MouseInput().x, transform.eulerAngles.z);   //Karakteri d�nd�rme kodumuz.

        if (mainCamera != null)    //Kameray� d�nd�rme kodumuz.
        {

            if (mainCamera.eulerAngles.x > maxWievAngle && mainCamera.eulerAngles.x < 180f)
            {
                mainCamera.rotation = Quaternion.Euler(maxWievAngle,mainCamera.eulerAngles.y,mainCamera.eulerAngles.z);
            }

            else if (mainCamera.eulerAngles.x > 180f && mainCamera.eulerAngles.x < 360f - maxWievAngle)
            {
                mainCamera.rotation = Quaternion.Euler(360f - maxWievAngle, mainCamera.eulerAngles.y, mainCamera.eulerAngles.z);
            }

            else
            {
                mainCamera.rotation = Quaternion.Euler(mainCamera.rotation.eulerAngles + new Vector3(-MouseInput().y, 0f, 0f));
            }
        }
    }

    private void Move()
    {
        if (jump)
        {
            heightMovement.y = jumpPower;
            jump = false;
        }

        heightMovement.y -= gravityModifier * Time.deltaTime;   //Karaktere s�rekli bir yer�ekimi etki edecek.

        Vector3 localVerticalVector = transform.forward * verticalInput;
        Vector3 localHorizontalVector = transform.right * horizontalInput;

        Vector3 movementVector = localVerticalVector + localHorizontalVector;
        movementVector.Normalize();     //Bu kodu yazmasak karakter �ne ve arkaya yazd���m�z h�zda hareket ederken �arprazlara giderken iki vekt�r� toplay�p daha h�zl� gidiyor. Bu fonksiyon ile vekt�rlerin uzunlu�unu 1'e sabitliyoruz.
        movementVector *= currentSpeed * Time.deltaTime;

        characterController.Move(movementVector + heightMovement);

        if (characterController.isGrounded)
        {
            heightMovement.y = 0f;    //Karakter yere de�iyorsa s�rekli bir �ekim etki etmesin, �st�nde a��rl�k yapmas�n diye yerdeyken yer�ekimini s�f�rlad�k, z�play�nca gene etki edecek.

            if (!landSoundPlayed)
            {
                audioSource.PlayOneShot(landSound);
                landSoundPlayed = true;
            }
        }
    }

    private void AnimationChanger()
    {
        if (newMovementInput.ReadValue<Vector2>().magnitude > 0f  && characterController.isGrounded)
        {
            if (currentSpeed == walkSpeed)
            {
                anim.SetBool("Walk", true);
                anim.SetBool("Run", false);
            }

            else if (currentSpeed == runSpeed)
            {
                anim.SetBool("Run", true);
                anim.SetBool("Walk", false);
            }
        }
        else
        {
            anim.SetBool("Walk", false);
            anim.SetBool("Run", false);
        }
    }

    private void KeyboardInput()
    {
        horizontalInput = newMovementInput.ReadValue<Vector2>().x;   //Yeni �nput sistemi kodlar�.
        verticalInput = newMovementInput.ReadValue<Vector2>().y;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && characterController.isGrounded)   //�uan tak�l� olan keyboarda git,space tu�unu getir,bu kare esnas�nda tu�a bir kez bas�ld� m�, tek sefer �al���r.
        {
            jump = true;
            landSoundPlayed = false;
            audioSource.PlayOneShot(jumpSound);
        }

        if (Keyboard.current.leftShiftKey.isPressed)   //Shift tu�una bas�l� tuttu�umuz s�rece anlam�na gelir.
        {
            currentSpeed = runSpeed;
        }

        else
        {
            currentSpeed = walkSpeed;
        }
    }

    private void PlayFootStepSound()   //Bu yazd���m�z kodda do while d�ng�s�yle son �ald���m�z audio clip bir �ncekine e�itse farkl� ses klibi �almak i�in kod yazd�k.
    {
        int index;    //index de�erini default olarak o yapt�k.
        do
        {
            index = UnityEngine.Random.Range(0, footStepSounds.Count);   //Burada index de�erine belirli aral�kta, yani ses klibimizin say�s� kadar index numaras� atad�k.
            if (lastIndex != index)    //E�er bir �nceki indexe e�it de�ilse sesi bir kere oynat ve break ile while d�ng�s�n� bitir demektir.
            {
                if (!audioSource.isPlaying)
                {
                    audioSource.PlayOneShot(footStepSounds[index]);
                    lastIndex = index;    
                    break;
                }
            }
        } 
        while (index == lastIndex);
       
    }

    private Vector2 MouseInput()
    {
        return new Vector2(invertX ? -Mouse.current.delta.x.ReadValue() : Mouse.current.delta.x.ReadValue(), invertY ? -Mouse.current.delta.y.ReadValue() : Mouse.current.delta.y.ReadValue()) * mouseSensivity;
        //�steyen kullan�c�lar i�in kontrolleri ki�iselle�tirdik. Mouse sa�a gidince ekran sola, mouse sola gidince ekran sa�a d�nd�r�lebilir.
        //�ssteki kodda iki noktan�n sa��ndaki k�s�m invertX yanl�� ise �al���r, solundaki k�s�m invertX do�ru ise �al���r.
    }


}
