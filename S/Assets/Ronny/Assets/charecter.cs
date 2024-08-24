using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class charecter : MonoBehaviour
{
    //[SerializeField]
    //bool suspected = false;
    [SerializeField]
    bool cleared = false;
    [SerializeField]
    bool chosen = false;
    Renderer rend;
    [SerializeField]
    Material defaultM;
    [SerializeField]
    Material clearedM;
    [SerializeField]
    Material chosenM;
    [SerializeField]
    Material tryAgainM;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }
    


    internal void makeCleared()
    {
        if (cleared == false)
        {
            cleared = true;
            Debug.Log("cleared");
            if (rend != null)
            {
                rend.material = clearedM;
            }
        }

        else
        {
            cleared = false;
            Debug.Log("default");
            if (rend != null)
            {
                rend.material = defaultM;
            }
        }
    }

    internal void makeSuspected()
    {
        //suspected = true;
        if (chosen == true)
        {
            if (rend != null)
            {
                rend.material = chosenM;
            }
            Debug.Log("You Win");
        }
        else
        {
            if (rend != null)
            {
                rend.material = tryAgainM;
            }
            Debug.Log("Try Again");
        }

    }

    internal void makeChosen()
    {
        chosen= true;
    }
}
