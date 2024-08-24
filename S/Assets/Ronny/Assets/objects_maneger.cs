using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class objects_maneger : MonoBehaviour
{
    [SerializeField]
    List<charecter> charecters_list;
    [SerializeField]
    int numberOfCells;
    charecter charecter_ref;
    charecter charecterChosen_ref;




    internal void suspected(Transform selected)
    {
        findTheSelected(selected);
        suspectedSelected(charecter_ref);
    }

    internal void cleared(Transform selected)
    {
        findTheSelected(selected);
        clearedSelected(charecter_ref);

    }

    internal void findTheSelected(Transform selected)
    {
        Vector3 location = selected.position;

        for (int i = 0; i < numberOfCells; i++)
        {
            if (charecters_list[i].transform.position == location)
            {
                charecter_ref = charecters_list[i];
                              
            }
        }
    }

    private void suspectedSelected(charecter charecter_ref)
    {
        charecter_ref.makeSuspected();
        
    }

    private void clearedSelected(charecter charecter_ref)
    {
        charecter_ref.makeCleared();
        
    }

    internal  void selectchosen(int n)
    {
        charecterChosen_ref = charecters_list[n];
        charecterChosen_ref.makeChosen();
    }
}
