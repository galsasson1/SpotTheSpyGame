using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonInteractions : MonoBehaviour
{
    /// <summary>
    /// Enum to indicate what sort of marking this person currently has
    /// </summary>
    private enum Status
    {
        UNMARKED,
        CLEARED,
        SUSPECT
    }

    #region Hidden Variables
    /// <summary>
    /// The current marking of the person
    /// </summary>
    private Status _status = Status.UNMARKED;

    /// <summary>
    /// Reference to the game manager
    /// </summary>
    private GameManager _gm;

    /// <summary>
    /// Reference to the person's quick outline script
    /// </summary>
    private Outline _outline;

    /// <summary>
    /// A reference array to all of the person's sprites to allow quick material changing
    /// </summary>
    private SpriteRenderer[] _personGraphics;
    #endregion

    /// <summary>
    /// On start, get all necessery references
    /// </summary>
    private void Start()
    {
        _gm = FindAnyObjectByType<GameManager>();
        _outline = GetComponent<Outline>();
        _personGraphics = GetComponentsInChildren<SpriteRenderer>();
    }

    /// <summary>
    /// When called, applies the correct material to all the person's sprites, and updates the suspect count UI to reflect the changed amount
    /// </summary>
    public void MarkCleared()
    {
        _gm.PlayTargetOtherPlayerSound();

        //If the target was already cleared, unmark them, assign the regular outline material and increase the suspect count
        if (_status == Status.CLEARED)
        {
            _status = Status.UNMARKED;
            SetMaterials(_gm.outlineMat);
            _gm.UpdateSuspectsTotal(1);
            return;
        }

        //If the target was a suspect, clear out both the related outline graphics and the running reference in the game manager
        if(_status == Status.SUSPECT)
        {
            _gm.currentSuspect = null;
            _outline.enabled = false;
            _gm.ToggleGuessButton();
        }

        //Mark the target as cleared, set the "blacked out" material to all of the person's sprite, and reduce the suspect count by 1
        _status = Status.CLEARED;
        SetMaterials(_gm.blackedOutOutlineMat);
        _gm.UpdateSuspectsTotal(-1);
    }

    /// <summary>
    /// When calls, applies the appropriate outline graphic and updates the suspect reference in the game manager
    /// </summary>
    public void MarkSuspect()
    {
        _gm.PlayEliminatePlayerSound();

        //If the target was a suspect, unmark them, clear out both the related outline graphics and the running reference in the game manager
        if (_status == Status.SUSPECT)
        {
            _status = Status.UNMARKED;
            _outline.enabled = false;
            _gm.currentSuspect = null;
            _gm.ToggleGuessButton();
            return;
        }

        //If the target was cleared, assign the regular outline material and increase the suspect count
        if (_status == Status.CLEARED)
        {
            SetMaterials(_gm.outlineMat);
            _gm.UpdateSuspectsTotal(1);
        }

        //If there is another reference to another suspect in the game manager, unmark them
        if(_gm.currentSuspect != null && _gm.currentSuspect != this)
        {
            _gm.currentSuspect.MarkSuspect();
        }

        //Apply the appropriate outline graphic and update the suspect reference in the game manager
        _gm.currentSuspect = this;
        _status = Status.SUSPECT;
        _outline.enabled = true;
        _gm.ToggleGuessButton();
    }

    /// <summary>
    /// For each valid sprite under the game object this script is attached to, apply the input material
    /// </summary>
    /// <param name="material"></param>
    private void SetMaterials(Material material)
    {
        foreach(SpriteRenderer sprite in _personGraphics)
        {
            if(sprite.sprite != null)
            {
                sprite.material = material;
            }
        }
    }
}
