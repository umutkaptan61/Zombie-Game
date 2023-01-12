using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    
    private Transform target;

    private void Awake()
    {
        target = GameObject.FindWithTag("CameraPoint").transform;     //target de�i�keninin Transform de�eri, CameraPoint tagli yani Playerin i�ine olu�turdu�umuz CameraPoint noktas�n�n transform de�erine denktir.
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        if (target != null)    //cameran�n pozisyonunu ve a��s�n� verdi�imiz point noktas�na e�itledik.
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
   
}
