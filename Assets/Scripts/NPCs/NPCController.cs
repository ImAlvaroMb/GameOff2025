using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Enums;

public class NPCController : MonoBehaviour
{
    [SerializeField] private List<AIAction> actionsProbabilities = new List<AIAction>();
    public NPCActions CurrentAction => _currentAction;
    private NPCActions _currentAction;
    private float _currentTotalProbability;

    [Header("Patrol")]
    public Vector2 OriginalPosition => _originalPosition;
    private Vector2 _originalPosition;
    public float RangeToPatrol => rangeToPatrol;
    [SerializeField] private float rangeToPatrol;
    private void OnValidate()
    {
        _currentTotalProbability = actionsProbabilities.Sum(item => item.Probability);

        const float target = 100f;
        const float tolerance = 0.01f; 

        if (Mathf.Abs(_currentTotalProbability - target) > tolerance)
        {
            Debug.LogError($"Validation Error on {gameObject.name}: Action Probabilities must sum to 100. Current sum is {_currentTotalProbability:F2}%.");
        }
        else
        {
            Debug.Log($"Validation Success on {gameObject.name}: Total probability is exactly 100%.");
        }
    }

    private void Start()
    {
        _originalPosition = transform.position;
    }

    public void DecideNextAction()
    {
        ReevaluateProbabilities();
    }

    private void ReevaluateProbabilities()
    {
        ChooseAction();
    }

    private void ChooseAction()
    {
        float randomNumber = Random.Range(0f, _currentTotalProbability);
        float cumulativeWeight = 0f;

        foreach (var action in actionsProbabilities)
        {
            cumulativeWeight += action.Probability;

            if(randomNumber <= cumulativeWeight)
            {
                _currentAction = action.Action;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, rangeToPatrol);
    }
}
