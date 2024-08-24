using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script to lock a 2D sprite to face camera 
/// </summary>
public class SpriteBillboard : MonoBehaviour
{
    /// <summary>
    /// Update is called once per frame, and updates the Y position of the sprite to face the camera
    /// </summary>
    void Update()
    {
        Vector3 target = Camera.main.transform.position;
        target.y = transform.position.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.position - target), 5f * Time.deltaTime);
    }
}
