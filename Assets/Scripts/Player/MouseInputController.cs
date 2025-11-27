using UnityEngine;
using Enums;
using StateMachine;
using Utilities;
using UnityEngine.EventSystems;

public class MouseInputController : AbstractSingleton<MouseInputController>
{
    [SerializeField] private LayerMask layersToCheck;
    [SerializeField] private LayerMask npcLayerMask;
    [SerializeField] private LayerMask interactableLayer;

    [SerializeField] private float maxRaycastDistance = 40f;

    private BaseInteractable _currentHoveredObject = null;
    private NPCController _currentHoveredNPC = null;

    public NPCController CurrentlySelectedNPC => _currentSelectedNPC;
    private NPCController _currentSelectedNPC = null;

    private Camera mainCamera;

    // This state variable should ideally be managed by the new influence system (AreaOfInfluence/NPCController), 
    // but we keep it here as it was part of the original logic.
    public bool IsInAreaOfInfluence = false;

    public bool IsTestingClickRay = false;
    [SerializeField] private LayerMask groundLayer;

    public NPCMovementController PathTesting;

    private int _interactableLayerID;
    private int _influenceLayerID;
    private int _npcLayerID;

    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;

        // Cache layer IDs
        _interactableLayerID = LayerMask.NameToLayer("Interactable");
        _influenceLayerID = LayerMask.NameToLayer("Influence");
        _npcLayerID = LayerMask.NameToLayer("NPC");

        if (npcLayerMask.value == 0)
        {
            npcLayerMask = 1 << _npcLayerID;
        }
    }

    private void Update()
    {
        CheckForNPCHover();

        CheckForOtherHoves();

        CheckForInteractableHover();

        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (HandleNPCClick()) return;

            HandleGeneralClick();
        }
    }

    private void CheckForNPCHover()
    {
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, maxRaycastDistance, npcLayerMask);
        HandleNPCHover(hit);
    }

    private void CheckForOtherHoves()
    {
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, maxRaycastDistance, layersToCheck);

        //HandleIInteractableHover(hit);
        HandleAreaOfInfluenceHover(hit);
    }

    private void CheckForInteractableHover()
    {
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, maxRaycastDistance, interactableLayer);
        HandleIInteractableHover(hit);
    }

    private void HandleIInteractableHover(RaycastHit2D hit)
    {
        BaseInteractable newTarget = null;
        if (hit.collider != null && hit.collider.gameObject.layer == _interactableLayerID)
        {
            newTarget = hit.collider.GetComponent<BaseInteractable>();
        }

        if (newTarget != _currentHoveredObject)
        {
            if (_currentHoveredObject != null)
            {
                _currentHoveredObject.Dehighlight();
            }

            if (newTarget != null)
            {
                newTarget.Highlight();
            }

            _currentHoveredObject = newTarget;
        }
        else if (newTarget == null && _currentHoveredObject != null)
        {
            _currentHoveredObject.Dehighlight();
            _currentHoveredObject = null;
        }
    }

    private void HandleAreaOfInfluenceHover(RaycastHit2D hit)
    {
        if (hit.collider != null && hit.collider.gameObject.layer == _influenceLayerID)
        {
            // RangeOfInfluenceObject influenceObject = hit.collider.GetComponent<RangeOfInfluenceObject>(); // Not strictly needed for hover state
            IsInAreaOfInfluence = true;
        }
        else
        {
            IsInAreaOfInfluence = false;
        }
    }

    private void HandleNPCHover(RaycastHit2D hit)
    {
        NPCController newNPC = null;
        if (hit.collider != null)
        {
            // Note: Keep GetComponentInParent as it handles complex NPC prefab structures
            newNPC = hit.collider.GetComponentInParent<NPCController>();
        }

        if (newNPC != _currentHoveredNPC)
        {
            if (_currentHoveredNPC != null)
            {
                _currentHoveredNPC.OnStopHover();
            }

            if (newNPC != null)
            {
                if (newNPC == _currentSelectedNPC && _currentSelectedNPC != null)
                {
                    newNPC.OnHover(true);
                }
                else
                {
                    newNPC.OnHover(false);
                }
            }

            _currentHoveredNPC = newNPC;
        }
    }

    private bool HandleNPCClick()
    {
        if (_currentHoveredNPC != null)
        {
            CameraController cameraController = CameraController.Instance;
            if (cameraController.CurrentCameraTarget != _currentHoveredNPC.transform)
            {
                cameraController.CurrentCameraTarget?.StopCameraFollow();
                cameraController.StarFollowingTarget(_currentHoveredNPC);
                _currentHoveredNPC.StartCameraFollow();
            }
            if (_currentHoveredNPC.IsInInfluenceArea)
            {
                _currentSelectedNPC?.gameObject.GetComponent<NPCVisualController>().OnControlledChanged(false);
                _currentHoveredNPC.OnClicked();
                _currentSelectedNPC = _currentHoveredNPC;
                _currentSelectedNPC.gameObject.GetComponent<NPCVisualController>()?.OnControlledChanged(true);
            }
            return true; 
        }
        return false; 
    }

    private void HandleGeneralClick()
    {
        if (_currentHoveredObject != null && _currentHoveredObject.CanInteract())
        {
            _currentHoveredObject.Interact(null);
            return;
        } else if(_currentHoveredNPC != null && !_currentHoveredObject.CanInteract())
        {
            AlertSystemController.Instance.SendAlert("CAN ONLY INTERACT WITH THIS OBJECT WHEN INSIDE ARE OF INFLUENCE", 2f);
        }

        if (_currentSelectedNPC != null && _currentSelectedNPC.IsFullyControlled)
        {
            if (_currentHoveredObject != null)
            {
                _currentSelectedNPC.SetCurrentInteractable(_currentHoveredObject);
                _currentSelectedNPC.SetCurrentAction(NPCActions.DO_OBJECT_INTERACTION);
                return;
            }

            Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D groundHit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, maxRaycastDistance, groundLayer);

            if (groundHit.collider != null)
            {
                Vector3 clickPosition = groundHit.point;
                _currentSelectedNPC.gameObject.GetComponent<NPCMovementController>().SetTargetPoint(clickPosition);
                _currentSelectedNPC.SetCurrentAction(NPCActions.PATROL);
                return;
            }
        }
    }


    public void HandleNPCActionUIClick(ChooseActionUIType actionType)
    {
        switch (actionType)
        {
            case ChooseActionUIType.TALK_TO:
                _currentSelectedNPC.SetOtherNPCReference(_currentHoveredNPC);
                _currentSelectedNPC.SetTalkType(TalkType.TALKER);
                _currentHoveredNPC.SetTalkType(TalkType.LISTENER);
                break;

            case ChooseActionUIType.CAMERA_FOLLOW:
                CameraController cameraController = CameraController.Instance;
                if (cameraController.CurrentCameraTarget != _currentHoveredNPC.transform)
                {
                    cameraController.CurrentCameraTarget?.StopCameraFollow();
                    cameraController.StarFollowingTarget(_currentHoveredNPC);
                    _currentHoveredNPC.StartCameraFollow();
                    break;
                }

                if (cameraController.CurrentCameraTarget == _currentHoveredNPC && cameraController.IsFollowingTraget)
                {
                    cameraController.StopFollowingTarget();
                    _currentHoveredNPC?.StopCameraFollow();
                }
                break;

            case ChooseActionUIType.WAVE_TO:
                if(Vector2.Distance(_currentSelectedNPC.gameObject.transform.position, _currentHoveredNPC.gameObject.transform.position) < _currentHoveredNPC.GetWaveDistance())
                {
                    _currentSelectedNPC.SetWaveType(NPCWaveType.WAVER);
                    _currentHoveredNPC.SetOtherNPCReference(_currentHoveredNPC);
                    _currentHoveredNPC.SetOtherNPCReference(_currentSelectedNPC);
                    _currentHoveredNPC.SetWaveType(NPCWaveType.WAVE_RECEIVER);
                }
                break;

            case ChooseActionUIType.SELECT:
                if (_currentHoveredNPC != _currentSelectedNPC)
                {
                    _currentSelectedNPC?.gameObject.GetComponent<NPCVisualController>().OnControlledChanged(false);
                    _currentSelectedNPC = _currentHoveredNPC;
                    _currentSelectedNPC.gameObject.GetComponent<NPCVisualController>()?.OnControlledChanged(true);
                }
                break;
        }
    }

    #region Utilities

    public void UnselectCurrentSelectedNPC()
    {
        if(_currentSelectedNPC != null)
        {
            _currentSelectedNPC.gameObject.GetComponent<NPCVisualController>().OnControlledChanged(false);
            _currentSelectedNPC = null;
            AlertSystemController.Instance.SendAlert($"Currently no NPC is selected", 2f);
        }
    }

    #endregion
}
