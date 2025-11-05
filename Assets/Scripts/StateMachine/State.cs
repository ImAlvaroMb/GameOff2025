using UnityEngine;
namespace StateMachine
{
    public abstract class State : ScriptableObject
    {
        protected StateController stateController;
        [SerializeField] private Transition[] possibleTransitions;
        public bool IsDone => _isDone;

        private bool _isDone = false;

        public abstract void OnEnter();
        public abstract void FixedUpdateState();
        public abstract void UpdateState();
        public abstract void OnExit();
        public abstract void FinishState();

        public void setStateController(StateController stateController)
        {
            this.stateController = stateController;
        }

        public State CheckTransitions()
        {
            State exitState = null;
            for (int i = 0; i < possibleTransitions.Length && exitState == null; i++)
            {
                exitState = possibleTransitions[i].getExitState(stateController);
            }

            return exitState;
        }
    }

}
