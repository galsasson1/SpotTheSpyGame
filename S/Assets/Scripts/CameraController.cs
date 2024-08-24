using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    #region Exposed Variables

    /// <summary>
    /// The rate in which the camera is moved when dragging it around
    /// </summary>
    [Header("Movement Parameters")]
    [SerializeField] private float _dragSpeed;

    /// <summary>
    /// The speed of rotaion of the camera
    /// </summary>
    [SerializeField] private float _rotaionSpeed;

    /// <summary>
    /// Maximum movement range along the X axis
    /// </summary>
    [SerializeField] private float _horizontalMax;

    /// <summary>
    /// Maximum movement range along the Z axis
    /// </summary>
    [SerializeField] private float _depthMax;

    /// <summary>
    /// The rate in which the zoom changes when using the mousewheel
    /// </summary>
    [Header("Zoom Parameters")]
    [SerializeField] private float _zoomSpeed;

    /// <summary>
    /// Minimum FOV value (How far can the camera zoom in)
    /// </summary>
    [SerializeField] private float _zoomMinFOV;

    /// <summary>
    /// Maximum FOV value (How far can the camera zoom out)
    /// </summary>
    [SerializeField] private float _zoomMaxFOV;

    /// <summary>
    /// The layer mask for the raycast calculations to filter out only interactable objects
    /// </summary>
    [Header("Raycast Parameters")]
    [SerializeField] private LayerMask _interactablesLayer;

    /// <summary>
    /// The maximum distance to fire the raycast to check interactables
    /// </summary>
    [SerializeField] private float _raycastMaxDistance;

    /// <summary>
    /// The maximum length of time before the code assume you just wanted to drag scroll and not mark someone
    /// </summary>
    [SerializeField] private float _timeToMark;

    /// <summary>
    /// The X rotation of the camera in the start
    /// </summary>
    private float _Xrotation;

    /// <summary>
    /// Is the player currently dragging the camera?
    /// </summary>
    private bool isOnUI;
    #endregion

    #region Hidden Variables

    /// <summary>
    /// Main camera reference
    /// </summary>
    private Camera _camera;

    /// <summary>
    /// Camera's transform reference
    /// </summary>
    private Transform _cameraTransform;

    /// <summary>
    /// Starting position of the camera
    /// </summary>
    private Vector3 _startingCameraPos;

    /// <summary>
    /// The currently attempted zoom level 
    /// </summary>
    private float _newZoom;

    /// <summary>
    /// Internal variable to serve as timer for the marking check
    /// </summary>
    private float _runningTimeToMark;

    // Internal variable to serve as a timer between left-clicks
    private float _lastLeftClickTime = 0f;

    // Tracks whether the pointer (finger) is currently pressed down
    private bool isPointerDown = false;
    // Tracks whether the pointer moves during a long press
    private bool isDragging = false;
    // Store initial position before dragging mouse
    private Vector3 initialPosition;
    // Tracks how long the pointer has been pressed down
    private float pointerDownTimer = 0f;
    // Duration threshold for a long press gesture
    private float longPressDuration = 0.5f;
    // Movement threshold
    private float dragThreshold = 0.1f;

    #endregion

    /// <summary>
    /// On start, record starting position of the camera and assign camera references
    /// </summary>
    void Start()
    {
        _camera = Camera.main;
        _cameraTransform = _camera.transform;
        _startingCameraPos = _camera.transform.localPosition;
        _Xrotation = transform.rotation.eulerAngles.x;
    }

    /// <summary>
    /// Every frame, check for player inputs, and adjust the camera/fire raycasts accordingly
    /// </summary>
    void Update()
    {
        if (!UIManager.isPaused)
            Move();
    }    

    ///<summary>
    ///move the camera according to the mouse movement
    ///</summary>
    private void Move()
    {
        // Handles zooming in and out when scrolling the mousewheel
        Zoom(Input.GetAxis("Mouse ScrollWheel"));

        // Double click to mark suspect
        if (Input.GetMouseButtonDown(0))
        {
            // Check for a double click
            if ((Time.time - _lastLeftClickTime) < 0.3f)
            {
                // Handle marking suspects for double click
                MarkSuspect(true, Input.mousePosition);
            }
            else
            {
                // Single click
                _lastLeftClickTime = Time.time;
            }
        }

        //When you press on the RMB, send a raycast at the mouse's position. If that target is a person, mark him as cleared
        MarkCleared(Input.mousePosition);

        //Drag the mouse across the screen with LMB to move the camera across the map
        if (Input.GetButtonDown("Fire1"))
            isOnUI = IsPointerOverUIObject();
        DraggingControls(Input.GetButton("Fire1"), Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        //Reset camera to starting position upon pressing the mousewheel
        ResetCamera(Input.GetButtonDown("Fire3"));

        //Rotate the camera
        Rotation(Input.GetKey(KeyCode.Q), Input.GetKey(KeyCode.E));
    }

    private void Zoom(float scrollWheel)
    {

        _newZoom = _camera.fieldOfView - (scrollWheel * _zoomSpeed);
        if (_newZoom >= _zoomMinFOV && _newZoom <= _zoomMaxFOV)
        {
            _camera.fieldOfView = _newZoom;
        }
    }

    // Mark suspect with double click
    private void MarkSuspect(bool fireOnePressed, Vector3 mousePosition)
    {
        //When you first start holding down the mouse, start a timer equal to the variable _timeToMark
        if (fireOnePressed)
        {
            _runningTimeToMark = _timeToMark;
        }

        //If the timer is active, count down the timer
        if (_runningTimeToMark > 0)
        {
            _runningTimeToMark -= Time.deltaTime;
        }

        //If you release the LMB before the timer is up, send a raycast at the mouse's position. If that target is a person, mark him as suspect
        if (fireOnePressed && _runningTimeToMark > 0)
        {
            Ray ray = _camera.ScreenPointToRay(mousePosition);

            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _raycastMaxDistance, _interactablesLayer))
            {
                hit.collider.gameObject.GetComponent<PersonInteractions>().MarkSuspect();
            }
            _runningTimeToMark = 0;
        }
    }

    // Eliminate suspect with long click
    private void MarkCleared(Vector3 mousePostion)
    {
        // Check if the pointer (finger) is pressed down
        if (Input.GetMouseButtonDown(0))
        {
            isPointerDown = true;
            initialPosition = Input.mousePosition; // Store the initial position of the click
            isDragging = false; // Reset dragging to false when a new click is detected
        }

        // Check if the pointer (finger) is released, if so - reset
        if (Input.GetMouseButtonUp(0))
        {
            isPointerDown = false;
            pointerDownTimer = 0f;
            isDragging = false;
        }

        // If the pointer (finger) is currently pressed down and the mouse didnt move above threshold
        if (isPointerDown && !isDragging)
        {
            if (Vector3.Distance(initialPosition, Input.mousePosition) > dragThreshold)
            {
                // The pointer has moved too much, it's a drag
                isDragging = true;
            }

            pointerDownTimer += Time.deltaTime; 

            // Check if the pointer has been pressed down for the specified duration 
            if (pointerDownTimer >= longPressDuration && !isDragging)
            {
                Ray ray = _camera.ScreenPointToRay(mousePostion);

                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, _raycastMaxDistance, _interactablesLayer))
                {
                    hit.collider.gameObject.GetComponent<PersonInteractions>().MarkCleared();
                }

                // reset
                isPointerDown = false;
                pointerDownTimer = 0f;
            }
        }
    }

    private void DraggingControls(bool fireOnePressed, float mouseX, float mouseY)
    {
        if (fireOnePressed && !isOnUI)
        {

            //Create a target position from the mouse's current position, with the Y set to match the current camera's Y to prevent vertical movement
            var mousePosition = transform.right * -mouseX * _dragSpeed * Time.deltaTime +
                Vector3.ProjectOnPlane(transform.forward * -mouseY * _dragSpeed * Time.deltaTime + _cameraTransform.localPosition, Vector3.up);

            //Move the camera torwards the target position
            _cameraTransform.localPosition = new Vector3(Mathf.Clamp(mousePosition.x, 0, _horizontalMax), mousePosition.y, Mathf.Clamp(mousePosition.z, _depthMax, 0));
        }
    }

    private void ResetCamera(bool fireThreePressed)
    {
        if (fireThreePressed)
        {
            _cameraTransform.localPosition = _startingCameraPos;
            _camera.fieldOfView = _zoomMaxFOV;
        }
    }

    private void Rotation(bool qPressed, bool ePressed)
    {
        if (isOnUI) return;

        if (qPressed)
        {
            transform.Rotate(new Vector3(_Xrotation, -Time.deltaTime*_rotaionSpeed, 0f));
            transform.rotation = Quaternion.Euler(_Xrotation, transform.rotation.eulerAngles.y, 0.0f);
        }
        if (ePressed)
        {
            transform.Rotate(new Vector3(_Xrotation, Time.deltaTime * _rotaionSpeed, 0f));
            transform.rotation = Quaternion.Euler(_Xrotation, transform.rotation.eulerAngles.y, 0.0f);
        }
    }

    private static bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}

