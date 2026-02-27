using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class PlayerInput : MonoBehaviour
{
    [Header(nameof(InputActionAsset))] [SerializeField]
    private InputActionReference touchStart;

    [SerializeField] private InputActionReference touch;
    [SerializeField] private InputActionReference tap;
    [SerializeField] private InputActionReference lmb;
    [SerializeField] private InputActionReference touchPhase;
    [SerializeField] private InputActionReference mousePos;

    [SerializeField] private TextMeshProUGUI touchText;
    [SerializeField] private TextMeshProUGUI swipeText;

    [SerializeField] private TextMeshProUGUI currentPosText;
    [SerializeField] private TextMeshProUGUI startPosText;
    [SerializeField] private TextMeshProUGUI endPosText;

    [field: SerializeField] public TouchPhase phase { get; private set; }

    public bool lmbPerformed;
    public bool touchPerformed;
    private Vector2 endPos;
    private InputAction lmbAction;
    private InputAction mousePosAction;

    private Vector2 startPos;
    private InputAction tapAction;
    private InputAction touchAction;
    private InputAction touchPhaseAction;
    private InputAction touchStartInteraction;

    public Vector2 touchStartPos { get; private set; }
    public Vector2 touchPos { get; private set; }

    public bool TapPerformed => tapAction.WasPerformedThisFrame();

    public bool lmbEnded => lmbAction.WasReleasedThisFrame();

    private void Awake()
    {
        touchAction = touchStart;
        touchStartInteraction = touch;
        touchPhaseAction = touchPhase;
        tapAction = tap;
        lmbAction = lmb;
        mousePosAction = mousePos;

        RegisterActions();
    }

    private void Update()
    {
        // if (Input.touchCount > 0)
        //     touchPos = Input.GetTouch(0).position;
        // currentPosText.text = touchPos.ToString();
    }

    private void OnEnable()
    {
        touchAction.Enable();
        touchStartInteraction.Enable();
        tapAction.Enable();
        lmbAction.Enable();
        touchPhaseAction.Enable();
        mousePosAction.Enable();
    }

    private void OnDisable()
    {
        touchAction.Disable();
        touchStartInteraction.Disable();
        tapAction.Disable();
        lmbAction.Disable();
        mousePosAction.Disable();
    }

    private void RegisterActions()
    {
        //touchStartInteraction.performed += TouchStartActionPerformed;
        //touchStartInteraction.canceled += TouchStartActionPerformed;
        touchAction.performed += TouchActionPerformed;
        touchAction.canceled += TouchActionPerformed;
        touchPhaseAction.performed += TouchPhase;
        touchPhaseAction.canceled += TouchPhase;
        //mousePosAction.performed += MouseAction;
        //lmbAction.performed += LMBActionStarted;
        //lmbAction.canceled += LMBActionEnded;
    }

    private void MouseAction(InputAction.CallbackContext ctx)
    {
    }

    private void TouchStartActionPerformed(InputAction.CallbackContext ctx)
    {
        touchStartPos = ctx.ReadValue<Vector2>();

        if (lmbAction.WasPressedThisFrame() || touchAction.WasPressedThisFrame()) startPos = touchStartPos;
    }

    private void TouchActionPerformed(InputAction.CallbackContext ctx)
    {
        touchPos = ctx.ReadValue<Vector2>();
        currentPosText.text = touchPos.ToString();
    }

    private void LMBActionStarted(InputAction.CallbackContext ctx)
    {
        //startPos = ctx.ReadValue<Vector2>();
    }

    private void LMBActionEnded(InputAction.CallbackContext ctx)
    {
        //endPos = ctx.ReadValue<Vector2>();

        var swipeValue = (startPos - endPos).normalized;
        //Debug.Log((touchPos - endPos).normalized);
        swipeText.text = swipeValue.ToString();
    }

    private void TouchPhase(InputAction.CallbackContext ctx)
    {
        phase = ctx.ReadValue<TouchPhase>();

        touchText.text = phase.ToString();

        switch (phase)
        {
            case UnityEngine.InputSystem.TouchPhase.None:
                break;

            case UnityEngine.InputSystem.TouchPhase.Began:
                startPos = touchPos;
                startPosText.text = "startPos: " + startPos;


                break;
            case UnityEngine.InputSystem.TouchPhase.Moved:
                break;

            case UnityEngine.InputSystem.TouchPhase.Ended:
                endPos = touchPos;

                endPosText.text = "endPos: " + endPos;
                var swipeValue = (startPos - endPos).normalized;
                //Debug.Log((touchPos - endPos).normalized);
                swipeText.text = "swipeValue: " + swipeValue;
                break;


            case UnityEngine.InputSystem.TouchPhase.Canceled:
                break;
            case UnityEngine.InputSystem.TouchPhase.Stationary:
                break;
        }
    }
}