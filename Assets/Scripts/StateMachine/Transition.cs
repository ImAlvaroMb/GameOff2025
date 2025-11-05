using UnityEngine;

namespace StateMachine
{
    [System.Serializable]
    public class Transition
    {
        [SerializeField] private Decision deciosionToBeMade;
        [SerializeField] private State onDecisionTrueExitState;
        [SerializeField] private State onDecisionFalseExitState;

        public State getExitState(StateController stateController)
        {
            return deciosionToBeMade.Decide(stateController) ? onDecisionTrueExitState : onDecisionFalseExitState;
        }
    }
}

