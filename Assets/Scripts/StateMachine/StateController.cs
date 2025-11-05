using UnityEngine;
namespace StateMachine
{
    public class StateController : MonoBehaviour, IPausable
    {
        [SerializeField] private State firstState;
        public State CurrentState => _currentState;

        private State _currentState;
        private bool _isPaused = false;

        private void Start()
        {
            PauseManager.Instance.RegisterPausable(this);
            ChangeState(firstState);
        }

        private void FixedUpdate()
        {
            if (_isPaused) return;
            _currentState.FixedUpdateState();
            State newState = _currentState.CheckTransitions();

            if(newState != null) ChangeState(newState);
               
        }

        private void Update()
        {
            if (_isPaused) return;
            _currentState.UpdateState();
        }

        private void ChangeState(State newState)
        {
            if(_currentState != null)
            {
                _currentState.OnExit();
                Destroy(_currentState);
            }
            _currentState = Instantiate(newState);
            _currentState.setStateController(this);
            _currentState.OnEnter();
        }

        public void OnPause()
        {
            _isPaused = true;
        }

        public void OnResume()
        {
            _isPaused = false;
        }
    }
}

