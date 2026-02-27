using System;
using System.Collections;
using MyBox;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class AutoMove : MonoBehaviour
{
    #region Movement Settings

    [Foldout("Movement Direction")] [SerializeField]
    private int moveDirectionX = 1;

    [Foldout("Movement Direction")] [SerializeField]
    private int moveDirectionZ = 1;

    [Separator("Movement")] [SerializeField]
    private AnimationCurve smoothnessCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [SerializeField] private float drawHeight = .8f;
    [SerializeField] private AnimationCurve yMoveCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [SerializeField] private FieldScaleSO fieldScaleSO;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField, Tag] private string lavaFieldTag;
    [SerializeField, Tag] private string wallTag;

    [SerializeField] private bool moveInOne;

    [SerializeField] private bool canFallWhileMove = true;
    public FieldStatusChecker fieldStatusChecker;

    [Separator("Game Juice")] [SerializeField]
    private float ySaveDepth = 0f;

    [SerializeField] private float ySaveBounce = 7;

    [SerializeField] private float intendedY = 0.75f;

    private const float SLIDESAVE_CHECKDEPTH = 5;
    private const float DEFAULT_FIELDSURFACE_HEIGHT = 0.7f;


    [Separator("Scene specific")]
    [field: SerializeField]
    public RoundTimer RoundTimer { get; private set; }

    private SwipeInput swipeInput;

    #endregion

    #region Luck Settings

    [Separator("Luck Based")] [SerializeField]
    private bool moveShouldBeLucky;

    [ConditionalField(nameof(moveShouldBeLucky))] [Range(0f, 100f)] [SerializeField]
    private float luckPercentage = 50;

    #endregion

    #region Runtime

    public int CurrentFieldCount { get; private set; }
    public float ComesFromZ { get; private set; }

    private Vector3 moveDir;
    private float currentZOffset;
    private bool isMoving;
    public Action<Vector3> OnIntent;
    public Action<Vector3> OnLavaIntent;
    public Action<Vector3> OnBlocked;
    public Action OnMoveStart;
    public Action OnMoveEnd;
    public Action OnDeath;
    public Action OnFigureCrash;


    private Coroutine moveCoroutine;
    private Rigidbody rb;
    private bool isSinking;

    #endregion

    [SerializeField] private float skipSegValue = 20;

    [ButtonMethod]
    public void SkipSegment()
    {
        transform.position = transform.position.AddX(skipSegValue);
        rb.isKinematic = true;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        moveDir = new(moveDirectionX, 0, moveDirectionZ);
        moveDir = Vector3.Scale(moveDir, fieldScaleSO.fieldMultiplier);

        HandleMoveSide();
        ShowIntention();
    }

    private void OnEnable()
    {
        SwipeInput.OnSkip += SkipSegment;
    }

    public void Initialize(RoundTimer roundTimer, SwipeInput swipeInput)
    {
        RoundTimer = roundTimer;
        RoundTimer.OnTurn += OnTurn;

        this.swipeInput = swipeInput;
        swipeInput.OnSwipe += SubscribeSwipeIntentionUpdate;
    }

    private void OnDisable()
    {
        OnDeath?.Invoke();
        if (RoundTimer != null)
        {
            RoundTimer.OnTurn -= OnTurn;
        }

        if (swipeInput != null)
        {
            swipeInput.OnSwipe -= SubscribeSwipeIntentionUpdate;
        }

        SwipeInput.OnSkip -= SkipSegment;
    }

    private void SubscribeSwipeIntentionUpdate(int _)
    {
        Invoke(nameof(ShowIntention), 0.1f);
        Invoke(nameof(ShowIntention), 0.2f);
        Invoke(nameof(ShowIntention), 0.3f);
    }

    private void OnTurn(CurrentPhaseProperties phaseSettings)
    {
        if (!this) return;

        StartMoveturn(phaseSettings.moveTime, phaseSettings.thinkTime);
    }

    private void ShowIntention()
    {
        var intentionPosition = transform.position + moveDir;
        intentionPosition = CorrectCheckPosGrid(intentionPosition);

        MoveStatus intendedStatus = GetMoveIntendedStatus(intentionPosition, true);

        if (!moveInOne && intendedStatus.cantMoveReason == CantMoveReason.WALL)
        {
            intendedStatus = GetMoveIntendedStatus(intentionPosition, false);
        }

        switch (intendedStatus.cantMoveReason)
        {
            case CantMoveReason.WALL:
                OnBlocked?.Invoke((Vector3)intendedStatus.hinderPosition);
                return;
            
            case CantMoveReason.FIELD_LIMIT:
                
                return;

            case CantMoveReason.LAVA_FIELD:
                OnLavaIntent?.Invoke((Vector3)intendedStatus.hinderPosition);
                return;
        }

        OnIntent?.Invoke(intentionPosition);
    }

    private void StartMoveturn(float moveSeconds, float thinkSeconds)
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        moveCoroutine = StartCoroutine(MoveSetup(moveSeconds, thinkSeconds));
    }

    private IEnumerator MoveSetup(float moveSeconds, float thinkSeconds)
    {
        HandleMoveSide();
        ShowIntention();
        TryFall();
        OnMoveStart?.Invoke();

        currentZOffset = 0;
        ComesFromZ = transform.position.z;

        Vector3 targetPos = transform.position + moveDir;

        targetPos = CorrectCheckPosGrid(targetPos);


        MoveStatus intendedStatus = fieldStatusChecker.GetFieldStatus(targetPos);

        var frontFirst = ShouldMoveFrontFirst(ref intendedStatus, targetPos);

        ShowIntention();


        switch (intendedStatus.cantMoveReason)
        {
            case CantMoveReason.WALL:
                if (intendedStatus.hinderPosition == null) yield break;

                yield return BlockedCase(moveSeconds, intendedStatus);

                ShowIntention();

                moveCoroutine = null;
                yield break;

            case CantMoveReason.FIELD_LIMIT:
                HandleMoveSide();
                ShowIntention();
                break;
        }

        yield return Move(moveSeconds, targetPos, frontFirst);

        CurrentFieldCount++;
        OnMoveEnd?.Invoke();
        HandleMoveSide();
        ShowIntention();
        TryFall();

        yield return new WaitForSeconds(thinkSeconds);
        moveCoroutine = null;
    }

    private bool ShouldMoveFrontFirst(ref MoveStatus intendedStatus, Vector3 targetPos)
    {
        if (moveInOne) return false;

        if (intendedStatus.cantMoveReason == CantMoveReason.WALL) return false;

        intendedStatus = GetMoveIntendedStatus(targetPos, true);
        if (intendedStatus.cantMoveReason != CantMoveReason.WALL) return false;

        intendedStatus = GetMoveIntendedStatus(targetPos, false);
        if (intendedStatus.cantMoveReason == CantMoveReason.WALL) return false;

        return true;
    }

    private IEnumerator Move(float moveSeconds, Vector3 targetPos, bool frontFirst = false)
    {
        if (moveInOne || moveDirectionZ == 0)
        {
            yield return MoveTowards(moveSeconds, targetPos.x, targetPos.z);
            yield break;
        }

        float halfMoveTime = moveSeconds / 2;
        float? firstX = frontFirst ? targetPos.x : null;
        float? firstZ = frontFirst ? null : targetPos.z;

        yield return MoveTowards(halfMoveTime, firstX, firstZ);

        yield return MoveTowards(halfMoveTime, targetPos.x, targetPos.z);
    }

    private IEnumerator BlockedCase(float moveSeconds, MoveStatus intendedStatus)
    {
        Vector3 blockedPosition = intendedStatus.hinderPosition.Value;
        float halfMoveTime = moveSeconds / 2;
        float moveDivisor = 4;
        var visualizeFront = blockedPosition.x - transform.position.x > 0.1f;
        float initialBlockX = transform.position.x;

        float? firstX = visualizeFront ? transform.position.x + moveDir.x / moveDivisor : null;
        float? firstZ = visualizeFront ? transform.position.z : transform.position.z + moveDir.z / moveDivisor;

        float? secondX = visualizeFront ? initialBlockX : null;
        float? secondZ = visualizeFront ? transform.position.z : transform.position.z - moveDir.z / moveDivisor;

        yield return MoveTowards(halfMoveTime, firstX, firstZ);

        yield return MoveTowards(halfMoveTime, secondX, secondZ);
    }

    private Vector3 CorrectCheckPosGrid(Vector3 targetPos)
    {
        var multiplier = fieldScaleSO.fieldMultiplier;

        targetPos.x = Mathf.Round(targetPos.x / multiplier.x) * multiplier.x;
        targetPos.z = Mathf.Round(targetPos.z / multiplier.z) * multiplier.z;

        return targetPos;
    }


    /// <param name="xMove">Uses a nullable float in case you want to not use the value</param>
    /// <param name="zMove">Uses a nullable float in case you want to not use the value</param>
    private IEnumerator MoveTowards(float duration, float? xMove, float? zMove, AnimationCurve ownCurve = null)
    {
        isMoving = true;

        if (canFallWhileMove) rb.isKinematic = false;

        float corTime = 0f;

        var yOffset = CalculateYOffset();

        var initialY = transform.position.y;


        while (corTime < duration)
        {
            corTime += Time.deltaTime;
            var step = corTime / duration;

            xMove = BounceBackLogic(xMove, step);

            float currentCurveStep;

            currentCurveStep = EvaluateCurve(ownCurve, step);

            transform.position = CalculateMovementStep(xMove, zMove, currentZOffset, currentCurveStep);

            DrawCurrentHeight(initialY, yOffset, step);

            yield return null;
        }

        transform.position = CalculateMovementStep(xMove, zMove, currentZOffset, 1);

        currentZOffset = 0;

        isMoving = false;

        TryFall();
    }

    private void DrawCurrentHeight(float initialY, float yOffset, float step)
    {
        if (initialY < intendedY)
            initialY = intendedY;

        var currentYPos = Mathf.Lerp(initialY, initialY - yOffset, step);
        var heightGoal = initialY + drawHeight;

        transform.position = transform.position.ChangeY(Mathf.Lerp(currentYPos, heightGoal, yMoveCurve.Evaluate(step)));
    }

    private float EvaluateCurve(AnimationCurve ownCurve, float step)
    {
        float currentCurveStep;
        if (ownCurve != null)
        {
            currentCurveStep = ownCurve.Evaluate(step);
        }
        else
        {
            currentCurveStep = smoothnessCurve.Evaluate(step);
        }

        return currentCurveStep;
    }

    private float CalculateYOffset()
    {
        Physics.Raycast(transform.position + Vector3.up, Vector3.down, out RaycastHit hit, 2, groundLayerMask);

        var yOffset = hit.point.y - 0.5f;
        return yOffset;
    }

    private float? BounceBackLogic(float? xMove, float step)
    {
        if (step < 0.6f) return xMove;

        MoveStatus intendedStatus = GetMoveIntendedStatus((transform.position.AddX(0.2f)));

        if (intendedStatus.cantMoveReason != CantMoveReason.WALL) return xMove;

        if (isMoving)
        {
            xMove -= fieldScaleSO.fieldMultiplier.x;
        }

        isMoving = false;
        rb.isKinematic = false;
        OnMoveEnd?.Invoke();

        return xMove;
    }

    private void TryFall()
    {
        if (isSinking)
        {
            rb.isKinematic = false;
            return;
        }

        if (HasGround(transform.position, 0.3f))
            rb.isKinematic = true;
        else
            rb.isKinematic = false;
    }

    private Vector3 CalculateMovementStep(float? xMove, float? zMove, float zOffset, float step)
    {
        Vector3 target = transform.position;
        target.x = EvaluateNewValue(target.x, xMove);
        target.z = EvaluateNewValue(target.z, zMove);
        target.z += zOffset;

        return Vector3.Lerp(transform.position, target, step);
    }

    /// <param name="value">Fallback value</param>
    /// <param name="newValue">Nullable value to use</param>
    public float EvaluateNewValue(float value, float? newValue)
    {
        if (newValue == null) return value;

        return (float)newValue;
    }


    private void HandleMoveSide()
    {
        if (moveShouldBeLucky && UnityEngine.Random.value < luckPercentage / 100f)
            moveDir.z *= -1;

        if (!moveShouldBeLucky && fieldStatusChecker.GetFieldStatus(transform.position + moveDir).cantMoveReason ==
            CantMoveReason.FIELD_LIMIT)
            moveDir.z *= -1;
    }

    private bool HasGround(Vector3 position, float groundCheckDepth)
    {
        Debugger.DrawRay(position + Vector3.up * 2f, Vector3.down, groundCheckDepth + 2, Color.red, groundLayerMask,
            false);

        bool hasGround = Physics.Raycast(position + Vector3.up * 2f, Vector3.down, out RaycastHit hit,
            groundCheckDepth + 2,
            groundLayerMask);

        if (hit.collider == null) return false;
        if (hit.collider.CompareTag(lavaFieldTag)) return false;

        return hasGround;
    }

    private MoveStatus GetZBlockerFieldsUntil(Vector3 startPosition, Vector3 endPosition)
    {
        MoveStatus moveStatus = new();

        var direction = Mathf.Clamp(startPosition.z - endPosition.z, -1, 1);

        for (int i = 1; i < moveDirectionZ + 1; i++)
        {
            moveStatus =
                fieldStatusChecker.GetFieldStatus(
                    startPosition.AddZ(-direction * fieldScaleSO.fieldMultiplier.z * i));

            if (!moveStatus.CanMoveTo)
            {
                return moveStatus;
            }
        }

        return moveStatus;
    }

    public MoveStatus GetMoveIntendedStatus(Vector3 targetPos, bool checkSideFirst = true)
    {
        if (moveInOne)
        {
            var diagonalCheckPos = new Vector3(targetPos.x, transform.position.y, targetPos.z);
            return fieldStatusChecker.GetFieldStatus(diagonalCheckPos);
        }

        Vector3 firstStepPosition =
            checkSideFirst ? transform.position.SetZ(targetPos.z) : transform.position.SetX(targetPos.x);

        MoveStatus firstStatusCheck;
        if (checkSideFirst)
        {
            firstStatusCheck = GetZBlockerFieldsUntil(transform.position, firstStepPosition);
        }
        else
        {
            firstStatusCheck = fieldStatusChecker.GetFieldStatus(firstStepPosition);
        }

        if (!firstStatusCheck.CanMoveTo)
        {
            return firstStatusCheck;
        }


        Vector3 secondStepPosition =
            checkSideFirst ? firstStepPosition.SetX(targetPos.x) : firstStepPosition.SetZ(targetPos.z);

        MoveStatus secondStatusCheck;
        if (checkSideFirst)
        {
            secondStatusCheck = fieldStatusChecker.GetFieldStatus(secondStepPosition);
        }
        else
        {
            secondStatusCheck = GetZBlockerFieldsUntil(firstStepPosition, secondStepPosition);
        }

        return secondStatusCheck;
    }


    public void SetCurrentOffset(float zOffset)
    {
        zOffset *= fieldScaleSO.fieldMultiplier.z;
        var hasGround = HasGround(transform.position, 5);

        if (!hasGround)
        {
            SlideSaveGrace(zOffset);

            return; //hinders offset when no ground
        }

        var checkPos = transform.position.AddZ(zOffset);
        MoveStatus moveStatus = fieldStatusChecker.GetFieldStatus(checkPos);

        switch (moveStatus.cantMoveReason)
        {
            case CantMoveReason.FIELD_LIMIT:
                rb.isKinematic = false;
                return;

            case CantMoveReason.WALL:
                TryFall();
                break;

            case CantMoveReason.LAVA_FIELD:
                rb.isKinematic = false;
                break;
        }

        if (isMoving)
        {
            currentZOffset += zOffset;
        }
        else
        {
            transform.position = transform.position.AddZ(zOffset);
        }

        HandleMoveSide();
        ShowIntention();
    }

    public void BounceOff(float zOffset, float timeToBounce, AnimationCurve bounceCurve)
    {
        zOffset *= fieldScaleSO.fieldMultiplier.z;

        if (isMoving)
        {
            Debug.Log("Tried to bounce while Figure was already moving");
            return;
        }

        OnFigureCrash?.Invoke();

        var checkPos = transform.position.AddZ(zOffset);
        MoveStatus moveStatus = fieldStatusChecker.GetFieldStatus(checkPos);

        switch (moveStatus.cantMoveReason) //Could refactor for code dup with setoffset
        {
            case CantMoveReason.FIELD_LIMIT:
                rb.isKinematic = false;
                return;

            case CantMoveReason.WALL:
                TryFall();
                return;

            case CantMoveReason.LAVA_FIELD:
                rb.isKinematic = false;
                break;
        }

        var currentPhaseProperties = RoundTimer.GetCurrentPhaseInformation();
        float roundTimeLeft = currentPhaseProperties.thinkTime + currentPhaseProperties.moveTime -
                              currentPhaseProperties.currentRoundLength;

        var currentBounceTime = timeToBounce;

        if (roundTimeLeft < timeToBounce)
            currentBounceTime = roundTimeLeft;

        CoroutineOperation bounceOperation = new CoroutineOperation(this,
            StartCoroutine(MoveTowards(currentBounceTime, null, transform.position.z + zOffset, bounceCurve)),
            currentBounceTime);

        bounceOperation.OnFinished += b =>
        {
            HandleMoveSide();
            ShowIntention();
        };
    }

    private void SlideSaveGrace(float zOffset)
    {
        if (ySaveDepth > transform.position.y) return;
        if (!HasGround(transform.position.AddZ(-zOffset), SLIDESAVE_CHECKDEPTH)) return; //Check If there is a block to save to
        
        var differnceToAdd = DEFAULT_FIELDSURFACE_HEIGHT - transform.position.y;

        transform.position = transform.position.ChangeY(transform.position.y + differnceToAdd);
        rb.AddForce(new(0, ySaveBounce, 0), ForceMode.Impulse);
    }

    public void OnSinkingField()
    {
        isSinking = true;
        TryFall();
    }
}