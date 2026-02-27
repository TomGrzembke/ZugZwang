using System.Collections.Generic;
using UnityEngine;

public class DamageStates : MonoBehaviour
{
    [SerializeField] private FieldStatusChecker fieldStatusChecker;
    [SerializeField] private FieldScaleSO fielScaleSO;
    [SerializeField] private AutoMove autoMove;
    [SerializeField] private int ignoreTimes = 1;
    private int movedTimes;


    [SerializeField, Tooltip("Intended to hold different states 0 to 2")]
    private List<GameObject> states;


    private float initialY; //Prevents this feature from working in an upwards staircase scenario

    private void Start()
    {
        initialY = transform.position.y;
    }

    private void OnEnable()
    {
        autoMove.OnMoveEnd += UpdateState;
    }

    private void OnDisable()
    {
        autoMove.OnMoveEnd -= UpdateState;
    }

    public void UpdateState()
    {
        movedTimes++;
        
        if(movedTimes < ignoreTimes) return;
        
        int stateID = 0;
        var checkPos = transform.position;
        MoveStatus currentCheck;

        for (int i = 0; i < states.Count; i++)
        {
            if (i == 0)
            {
                states[i].SetActive(false);
                continue;
            }
            
            checkPos = transform.position.AddX(-(fielScaleSO.fieldMultiplier.x * i));
            checkPos = checkPos.ChangeY(initialY);
            
            currentCheck = fieldStatusChecker.GetFieldStatus(checkPos);
            states[i].SetActive(false);

            if (currentCheck.cantMoveReason != CantMoveReason.FIELD_LIMIT)
            {
                stateID++;
            }
        }


        states[stateID].SetActive(true);
    }
}