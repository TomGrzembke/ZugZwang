using System;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class SwipeInput : MonoBehaviour
{
    [Separator("Player Swipe Input")] [SerializeField]
    private InputActionReference tapStartPositionActionName;

    [SerializeField] private InputActionReference tapPositionActionName;

    [SerializeField] private InputActionReference clickActionName;
    [SerializeField] private InputActionReference clickPositionActionName;

    [SerializeField] private InputActionReference touchPhase;

    [Space(10)] [Separator("Optional Debug Text Fields")] [SerializeField]
    private TextMeshProUGUI positionField;

    [SerializeField] private TextMeshProUGUI touchField;
    [SerializeField] private LayerMask rayAbleLayer;

    [SerializeField] private InputActionReference skipActionName;
    private InputAction skipAction;

    private InputAction clickAction;
    private InputAction clickPositionAction;

    private Ray currentCamRay;
    private Camera mainCam;
    private Vector2 mousePosition;

    private Vector2 position;
    private Vector2 startPosition;

    private SwipeDirection swipeDirection;
    private InputAction tapPositionAction;

    private InputAction tapStartPositionAction;
    private InputAction touchPhaseAction;

    public event Action<int /*Swipe Direction*/> OnSwipe;
    public static Action OnSkip;


    public TouchPhase Phase { get; private set; }

    private void Awake()
    {
        mainCam = Camera.main;

        tapStartPositionAction = tapStartPositionActionName;
        tapPositionAction = tapPositionActionName;

        clickAction = clickActionName;
        clickPositionAction = clickPositionActionName;

        touchPhaseAction = touchPhase;

        if (skipActionName != null)
            skipAction = skipActionName;

        RegisterActions();
    }

    private void OnEnable()
    {
        tapStartPositionAction.Enable();
        tapPositionAction.Enable();
        clickAction.Enable();
        clickPositionAction.Enable();
        touchPhaseAction.Enable();

        if (skipActionName != null)
            skipAction.Enable();
    }

    private void OnDisable()
    {
        tapStartPositionAction.Disable();
        tapPositionAction.Disable();
        clickAction.Disable();
        clickPositionAction.Disable();
        touchPhaseAction.Disable();

        if (skipActionName != null)
            skipAction.Disable();
    }

    private void OnDestroy()
    {
        tapStartPositionAction.performed -= OnTapStartPositionActionPerformed;
        tapPositionAction.performed -= OnTapPositionActionPerformed;

        touchPhaseAction.performed -= TouchPhase;
        touchPhaseAction.canceled -= TouchPhase;

        clickAction.performed -= OnClickPositionActionPerformed;
        clickPositionAction.performed -= OnMousePositionActionPerformed;

        clickAction.canceled -= OnMousePositionActionCancelled;
        skipAction.performed -= OnSkipPerformed;
    }

    private void RegisterActions()
    {
        if (Application.isMobilePlatform)
        {
            tapStartPositionAction.performed += OnTapStartPositionActionPerformed;
            tapPositionAction.performed += OnTapPositionActionPerformed;

            touchPhaseAction.performed += TouchPhase;
            touchPhaseAction.canceled += TouchPhase;

            if (positionField != null) positionField.text = "Mobile";
        }
        else
        {
            clickAction.performed += OnClickPositionActionPerformed;
            clickPositionAction.performed += OnMousePositionActionPerformed;
            clickAction.canceled += OnMousePositionActionCancelled;

            if (skipActionName != null)
                skipAction.performed += OnSkipPerformed;
            if (positionField != null) positionField.text = "Not Mobile";
        }
    }

    private void OnTapPositionActionPerformed(InputAction.CallbackContext context)
    {
        position = context.ReadValue<Vector2>();
    }

    private void OnMousePositionActionCancelled(InputAction.CallbackContext context)
    {
        CheckSwipe();
        Deselect();
    }


    private void OnTapStartPositionActionPerformed(InputAction.CallbackContext context)
    {
        startPosition = context.ReadValue<Vector2>();
        currentCamRay = mainCam.ScreenPointToRay(startPosition);
    }

    private void OnClickPositionActionPerformed(InputAction.CallbackContext context)
    {
        startPosition = mousePosition;
        currentCamRay = mainCam.ScreenPointToRay(startPosition);

        Select();
    }

    private void OnSkipPerformed(InputAction.CallbackContext context)
    {
        OnSkip?.Invoke();
    }

    private void Select()
    {
        foreach (var swipeable in GatherAllSelectables()) swipeable.OnSelect(currentCamRay);
    }

    private void Deselect()
    {
        foreach (var swipeable in GatherAllSelectables()) swipeable.OnDeselect(currentCamRay);
    }

    private List<ISwipable> GatherAllSelectables()
    {
        RaycastHit hit;

        if (!Physics.Raycast(currentCamRay, out hit, 100f, rayAbleLayer)) return new List<ISwipable>();
        var swipeables = hit.collider.GetComponentsInChildren<ISwipable>().ToList();

        return swipeables;
    }

    private void OnMousePositionActionPerformed(InputAction.CallbackContext context)
    {
        mousePosition = context.ReadValue<Vector2>();
        position = context.ReadValue<Vector2>();
    }

    private void CheckSwipe()
    {
        if (startPosition.x > position.x)
            swipeDirection = SwipeDirection.LEFT;
        else if (startPosition.x < position.x)
            swipeDirection = SwipeDirection.RIGHT;
        else
            swipeDirection = SwipeDirection.NONE;

        if (positionField != null) positionField.text = swipeDirection.ToString();


        SwipeRay(startPosition);
    }


    public void SwipeRay(Vector3 rayPos)
    {
        RaycastHit hit;

        if (!Physics.Raycast(currentCamRay, out hit, 100f, rayAbleLayer)) return;
        var swipeables = hit.collider.GetComponentsInChildren<ISwipable>().ToList();


        switch (swipeDirection)
        {
            case SwipeDirection.LEFT:
                OnSwipe?.Invoke(1);
                foreach (var swipeable in swipeables) swipeable.OnSwiped(new Vector2(1, 0));

                break;
            case SwipeDirection.RIGHT:
                OnSwipe?.Invoke(-1);
                foreach (var swipeable in swipeables) swipeable.OnSwiped(new Vector2(-1, 0));

                break;
            case SwipeDirection.NONE:
                break;
        }
    }

    private void TouchPhase(InputAction.CallbackContext ctx)
    {
        Phase = ctx.ReadValue<TouchPhase>();

        if (touchField != null) touchField.text = "touchPhase: " + Phase;

        switch (Phase)
        {
            case UnityEngine.InputSystem.TouchPhase.None:
                break;
            case UnityEngine.InputSystem.TouchPhase.Began:
                Select();
                break;
            case UnityEngine.InputSystem.TouchPhase.Moved:
                break;
            case UnityEngine.InputSystem.TouchPhase.Ended:
                CheckSwipe();
                Deselect();
                break;
            case UnityEngine.InputSystem.TouchPhase.Canceled:
                break;
            case UnityEngine.InputSystem.TouchPhase.Stationary:
                break;
        }
    }

    private enum SwipeDirection
    {
        NONE = 0,
        LEFT = 10,
        RIGHT = 20
    }
}