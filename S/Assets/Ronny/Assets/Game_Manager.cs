using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
 

public class Game_Manager : MonoBehaviour
{
   
    [SerializeField]
    int zoomSpeed;
    [SerializeField]
    Camera cameraZoom;
    [SerializeField]
    LayerMask mouseLayer;
    private
    Transform selected;
    [SerializeField]
    objects_maneger objects_Maneger_ref;
    [SerializeField]
    int chosen;
    


        void awake()
    {
        
        cameraZoom = Camera.main;   
    }

    private void Start()
    {
        objects_Maneger_ref.selectchosen(chosen);
    }

    void Update()
    {
        //Camera.main.orthographicSize -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        //Select

        Ray ray = cameraZoom.ScreenPointToRay (Input.mousePosition);
        
        RaycastHit hit;
       
        if (Physics.Raycast( ray,out hit,mouseLayer ))
        {
                
            if (Input.GetMouseButtonDown(0)) 
            {
                selected = hit.transform;
                Debug.Log("suspected");
                objects_Maneger_ref.suspected(selected);
            }


            if (Input.GetMouseButtonDown(1))
            {
                selected = hit.transform;
                objects_Maneger_ref.cleared(selected);
            }

        }

    }

    
}
