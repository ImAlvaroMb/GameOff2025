using UnityEngine;

public class MouseInputController : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float maxRaycastDistance = 40f;

    private IInteractable currentHoveredObject = null;

    private Camera mainCamera;

    public bool IsTestingClickRay = false;
    [SerializeField] private LayerMask groundLayer;

    public PathTesting PathTesting;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        CheckForHover();

        if(Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    private void CheckForHover()
    {
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, maxRaycastDistance, interactableLayer);
        IInteractable newTarget = null;
        if(hit.collider != null)
        {
            newTarget = hit.collider.GetComponent<IInteractable>();
        } else
        {
            if(currentHoveredObject != null)
            {
                currentHoveredObject.Dehighlight();
                currentHoveredObject = null;
            }
        }

        if (newTarget != currentHoveredObject)
        {
           
            if (currentHoveredObject != null)
            {
                currentHoveredObject.Dehighlight();
            }

            if (newTarget != null)
            {
                newTarget.Highlight();
            }

            currentHoveredObject = newTarget;
        }
    }

    private void HandleClick()
    {
        if(currentHoveredObject != null && currentHoveredObject.CanInteract()) 
        {
            currentHoveredObject.Interact();
            return;
        }

        if(IsTestingClickRay)
        {
            Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D groundHit = Physics2D.Raycast(mouseWorldPos, Vector2.zero, maxRaycastDistance, groundLayer);

            if(groundHit.collider != null)
            {
                Vector3 clickPositon = groundHit.point;

                Node closestNode = AStarManager.Instance.FindNearestNode(clickPositon);
                Debug.Log($"Closest node: {closestNode}");
                if (PathTesting != null) PathTesting.GoToPosition(clickPositon);
            }

        }
    }
}
