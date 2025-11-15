using UnityEngine;
using Enums;
using StateMachine;
using Utilities;
using UnityEngine.EventSystems;

public class MouseInputController : AbstractSingleton<MouseInputController>
{
    [SerializeField] private LayerMask layersToCheck;
    [SerializeField] private float maxRaycastDistance = 40f;

    private BaseInteractable _currentHoveredObject = null;
    private NPCController _currentHoveredNPC = null;
    public NPCController CurrentlySelectedNPC => _currentSelectedNPC; // this is deff not a good praxis but its jam
    private NPCController _currentSelectedNPC = null;

    private Camera mainCamera;

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
        _interactableLayerID = LayerMask.NameToLayer("Interactable");
        _influenceLayerID = LayerMask.NameToLayer("Influence");
        _npcLayerID = LayerMask.NameToLayer("NPC");
    }
  
    private void Update()
    {
        CheckForHover();

        if(Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            HandleClick();
        }
    }

    private void CheckForHover()
    {
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, maxRaycastDistance, layersToCheck);
        HandleIInteractableHover(hit);
        HandleAreaOfInfluenceHover(hit);
        HandleNPCHover(hit);
    }

    private void HandleIInteractableHover(RaycastHit2D hit)
    {
        BaseInteractable newTarget = null;
        if (hit.collider != null && hit.collider.gameObject.layer == _interactableLayerID)
        {
            newTarget = hit.collider.GetComponent<BaseInteractable>();
        }
        else
        {
            if (_currentHoveredObject != null)
            {
                _currentHoveredObject.Dehighlight();
                _currentHoveredObject = null;
            }
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
    }

    private void HandleAreaOfInfluenceHover(RaycastHit2D hit)
    {
        RangeOfInfluenceObject influenceObject = null;
        if(hit.collider != null && hit.collider.gameObject.layer == _influenceLayerID)
        {
            influenceObject = hit.collider.GetComponent<RangeOfInfluenceObject>();
            IsInAreaOfInfluence = true;
        } else
        {
            IsInAreaOfInfluence = false;
        }
    }

    private void HandleNPCHover(RaycastHit2D hit)
    {
        NPCController newNPC = null;
        if(hit.collider != null && hit.collider.gameObject.layer == _npcLayerID)
        {
            newNPC = hit.collider.GetComponentInParent<NPCController>();
        }

        if(newNPC != _currentHoveredNPC)
        {
            if(_currentHoveredNPC != null)
            {
                _currentHoveredNPC.OnStopHover();
            }

            if(newNPC != null)
            {
                if(newNPC == _currentSelectedNPC && _currentSelectedNPC != null)
                {
                    newNPC.OnHover(true);
                } else
                {
                    newNPC.OnHover(false);
                }
            }

            _currentHoveredNPC = newNPC;
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
                if(cameraController.CurrentCameraTarget != _currentHoveredNPC.transform)
                {
                    cameraController.CurrentCameraTarget?.StopCameraFollow();
                    cameraController.StarFollowingTarget(_currentHoveredNPC);
                    _currentHoveredNPC.StartCameraFollow();
                    break;
                }
                
                if(cameraController.CurrentCameraTarget == _currentHoveredNPC && cameraController.IsFollowingTraget)
                {
                    cameraController.StopFollowingTarget();
                    _currentHoveredNPC?.StopCameraFollow();
                }
                break;

            case ChooseActionUIType.WAVE_TO:
                _currentSelectedNPC.SetOtherNPCReference(_currentHoveredNPC);
                _currentSelectedNPC.SetWaveType(NPCWaveType.WAVER);
                break;

            case ChooseActionUIType.SELECT:
                if(_currentHoveredNPC != _currentSelectedNPC)
                {
                    _currentSelectedNPC = _currentHoveredNPC;
                }
                break;
        }
    }

    private void HandleClick()
    {
        if (_currentHoveredNPC != null)
        {
            if (_currentHoveredNPC.IsInInfluenceArea)
            {
                _currentHoveredNPC.OnClicked();
                _currentSelectedNPC = _currentHoveredNPC;
            }
            return;
        }

        if (_currentHoveredObject != null && _currentHoveredObject.CanInteract())
        {
            _currentHoveredObject.Interact(null);
            return;
        }

        if (_currentSelectedNPC != null && _currentSelectedNPC.IsFullyControlled)
        {
            if(_currentHoveredObject != null)
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
}
