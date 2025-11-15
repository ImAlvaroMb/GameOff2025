using Unity.Cinemachine;
using UnityEngine;
using Utilities;
public class CameraController : AbstractSingleton<CameraController>
{
    public GameObject AllLevelCamera;
    public float ScrollSpeed = 10f;
    [Range(0.01f, 0.5f)]
    public float BorderThickness = 0.05f;
    public NPCController CurrentCameraTarget => _currentCameraTarget;
    private NPCController _currentCameraTarget = null;
    private bool _isAllMapCameraActive = false;
    private CinemachineCamera _vcam;
    public bool IsFollowingTraget => _isFollowingTarget;
    private bool _isFollowingTarget = false;
    private Vector3 lastScrollPosition;

    protected override void Awake()
    {
        base.Awake();
        _vcam = GetComponent<CinemachineCamera>();
        AllLevelCamera.SetActive(false);

        _vcam.Follow = null;
        _vcam.LookAt = null;
    }

    private void Update()
    {
        if(!_isAllMapCameraActive)
        {
            if(!_isFollowingTarget)
            {
                HandleBorderScrolling();
            }
        }
        
        CheckSpaceBarInput();
    }

    private void CheckSpaceBarInput()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(_isAllMapCameraActive)
            {
                AllLevelCamera.SetActive(false);
                _isAllMapCameraActive = false;
            } else
            {
                AllLevelCamera.SetActive(true);
                _isAllMapCameraActive = true;
            }
        }
    }

    public void StarFollowingTarget(NPCController target)
    {
        if(_isAllMapCameraActive)
        {
            AllLevelCamera.SetActive(false);
            _isAllMapCameraActive = false;
        }
        _currentCameraTarget = target;
        _isFollowingTarget = true;
    }

    public void StopFollowingTarget()
    {
        _isFollowingTarget = false;
        _currentCameraTarget = null;
    }

    private void HandleBorderScrolling()
    {
        Vector3 pos = transform.position;
        bool shouldMove = false;

        int leftBorder = (int)(Screen.width * BorderThickness);
        int topBorder = (int)(Screen.height * (1f - BorderThickness));
        int rightBorder = (int)(Screen.width * (1f - BorderThickness));
        int bottomBorder = (int)(Screen.height * BorderThickness);

        Vector3 mousePos = Input.mousePosition;

        // Right border
        if (mousePos.x >= rightBorder)
        {
            pos.x += ScrollSpeed * Time.deltaTime;
            shouldMove = true;
        }
        // Left border
        else if (mousePos.x <= leftBorder)
        {
            pos.x -= ScrollSpeed * Time.deltaTime;
            shouldMove = true;
        }

        // Top border
        if (mousePos.y >= topBorder)
        {
            pos.y += ScrollSpeed * Time.deltaTime;
            shouldMove = true;
        }
        // Bottom border
        else if (mousePos.y <= bottomBorder)
        {
            pos.y -= ScrollSpeed * Time.deltaTime;
            shouldMove = true;
        }

        if (shouldMove)
        {
            transform.position = pos;
            lastScrollPosition = pos; 
        }
    }
         
}
