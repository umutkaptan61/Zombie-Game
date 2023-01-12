using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    private Transform target;

    private void Awake()
    {
        target = GameObject.FindWithTag("CameraPoint").transform;     //target deðiþkeninin Transform deðeri, CameraPoint tagli yani Playerin içine oluþturduðumuz CameraPoint noktasýnýn transform deðerine denktir.
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        if (target != null)    //cameranýn pozisyonunu ve açýsýný verdiðimiz point noktasýna eþitledik.
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
   
}
