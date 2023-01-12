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

    [SerializeField] private InputAction newMovementInput;  //Yeni ýnput sistemini kullanmak için referans aldýk.

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

    private int lastIndex = -1;   //Sesleri oynatmak için bir deðiþken

    private bool landSoundPlayed = true;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        if (Camera.main.GetComponent<CameraController>() == null)    //Eðer ana kameramýzda CameraController scripti yoksa, scripti bulup onu ekleyecek. Güvenlik önlemi olarak koyduk.
        {
            Camera.main.gameObject.AddComponent<CameraController>();
        }

        mainCamera = GameObject.FindWithTag("CameraPoint").transform;
    }

    private void OnEnable()    //Obje aktif olunca çalýþýr.
    {
        newMovementInput.Enable();
    }

    private void OnDisable()   //Obje pasif olunca çalýþýr.
    {
        newMovementInput.Disable();
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        KeyboardInput();   //Ctrl ve nokta tuþuna beraber basarsak buraya yazdýðýmýz fonksiyon adýnda bir fonksiyon oluþturur.

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
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + MouseInput().x, transform.eulerAngles.z);   //Karakteri döndürme kodumuz.

        if (mainCamera != null)    //Kamerayý döndürme kodumuz.
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

        heightMovement.y -= gravityModifier * Time.deltaTime;   //Karaktere sürekli bir yerçekimi etki edecek.

        Vector3 localVerticalVector = transform.forward * verticalInput;
        Vector3 localHorizontalVector = transform.right * horizontalInput;

        Vector3 movementVector = localVerticalVector + localHorizontalVector;
        movementVector.Normalize();     //Bu kodu yazmasak karakter öne ve arkaya yazdýðýmýz hýzda hareket ederken çarprazlara giderken iki vektörü toplayýp daha hýzlý gidiyor. Bu fonksiyon ile vektörlerin uzunluðunu 1'e sabitliyoruz.
        movementVector *= currentSpeed * Time.deltaTime;

        characterController.Move(movementVector + heightMovement);

        if (characterController.isGrounded)
        {
            heightMovement.y = 0f;    //Karakter yere deðiyorsa sürekli bir çekim etki etmesin, üstünde aðýrlýk yapmasýn diye yerdeyken yerçekimini sýfýrladýk, zýplayýnca gene etki edecek.

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
        horizontalInput = newMovementInput.ReadValue<Vector2>().x;   //Yeni ýnput sistemi kodlarý.
        verticalInput = newMovementInput.ReadValue<Vector2>().y;

        if (Keyboard.current.spaceKey.wasPressedThisFrame && characterController.isGrounded)   //Þuan takýlý olan keyboarda git,space tuþunu getir,bu kare esnasýnda tuþa bir kez basýldý mý, tek sefer çalýþýr.
        {
            jump = true;
            landSoundPlayed = false;
            audioSource.PlayOneShot(jumpSound);
        }

        if (Keyboard.current.leftShiftKey.isPressed)   //Shift tuþuna basýlý tuttuðumuz sürece anlamýna gelir.
        {
            currentSpeed = runSpeed;
        }

        else
        {
            currentSpeed = walkSpeed;
        }
    }

    private void PlayFootStepSound()   //Bu yazdýðýmýz kodda do while döngüsüyle son çaldýðýmýz audio clip bir öncekine eþitse farklý ses klibi çalmak için kod yazdýk.
    {
        int index;    //index deðerini default olarak o yaptýk.
        do
        {
            index = UnityEngine.Random.Range(0, footStepSounds.Count);   //Burada index deðerine belirli aralýkta, yani ses klibimizin sayýsý kadar index numarasý atadýk.
            if (lastIndex != index)    //Eðer bir önceki indexe eþit deðilse sesi bir kere oynat ve break ile while döngüsünü bitir demektir.
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
        //Ýsteyen kullanýcýlar için kontrolleri kiþiselleþtirdik. Mouse saða gidince ekran sola, mouse sola gidince ekran saða döndürülebilir.
        //Üssteki kodda iki noktanýn saðýndaki kýsým invertX yanlýþ ise çalýþýr, solundaki kýsým invertX doðru ise çalýþýr.
    }


}
