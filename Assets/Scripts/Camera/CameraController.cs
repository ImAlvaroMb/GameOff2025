using Unity.Cinemachine;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    public GameObject AllLevelCamera;
    public float ScrollSpeed = 10f;
    [Range(0.01f, 0.5f)]
    public float BorderThickness = 0.05f;

    private bool _isAllMapCameraActive = false;
    private Transform _followTarget;
    private CinemachineCamera _vcam;
    private bool _isFollowingTarget = false;
    private Vector3 lastScrollPosition;

    private void Start()
    {
        _vcam = GetComponent<CinemachineCamera>();
        AllLevelCamera.SetActive(false);

        _vcam.Follow = null;
        _vcam.LookAt = null;
    }

    private void Update()
    {
        if(!_isAllMapCameraActive)
        {
            if (_isFollowingTarget)
            {
                if (_vcam.Follow == null && _followTarget != null)
                {
                    _vcam.Follow = _followTarget;
                    _vcam.LookAt = _followTarget;
                }
            }
            else
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

    public void StarFollowingTarget(Transform target)
    {
        if (target == null) return;

        lastScrollPosition = transform.position; 
        _followTarget = target;
        _isFollowingTarget = true;
        _vcam.Follow = _followTarget;
        _vcam.LookAt = _followTarget;
        Debug.Log($"Camera is now following target: {target.name}");
    }

    public void StopFollowingTarget()
    {
        _isFollowingTarget = false;
        _followTarget = null;
        _vcam.Follow = null;
        _vcam.LookAt = null;
        transform.position = lastScrollPosition;
        Debug.Log("Camera is now back in border scrolling mode.");
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
