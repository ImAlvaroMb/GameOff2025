using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering;
using Utilities;
public class CameraController : AbstractSingleton<CameraController>
{
    public GameObject[] AllLevelCamera;
    private int _currentAllLevelCameraIndex = 0;
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
        AllLevelCamera[_currentAllLevelCameraIndex].SetActive(false);

        _vcam.Follow = null;
        _vcam.LookAt = null;
    }

    private void Update()
    {
        /*if(!_isAllMapCameraActive)
        {
            if(!_isFollowingTarget)
            {
                HandleWASDScrolling();
            }
        }*/

        if (!_isFollowingTarget || _isAllMapCameraActive)
        {
            HandleWASDScrolling();
        }

        CheckSpaceBarInput();
    }

    private void CheckSpaceBarInput()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(_isAllMapCameraActive)
            {
                AllLevelCamera[_currentAllLevelCameraIndex].SetActive(false);
                _isAllMapCameraActive = false;
            } else
            {
                AllLevelCamera[_currentAllLevelCameraIndex].SetActive(true);
                _isAllMapCameraActive = true;
            }
        }
    }

    public void StarFollowingTarget(NPCController target)
    {
        if(_isAllMapCameraActive)
        {
            AllLevelCamera[_currentAllLevelCameraIndex].SetActive(false);
            _isAllMapCameraActive = false;
        }
        _currentCameraTarget = target;
        _isFollowingTarget = true;
    }

    public void StopFollowingTarget()
    {
        _currentCameraTarget?.StopCameraFollow();
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

    private void HandleWASDScrolling()
    {
        Vector3 pos = Vector3.zero;
        if(_isAllMapCameraActive)
        {
            pos = AllLevelCamera[_currentAllLevelCameraIndex].transform.position;
        } else
        {
            pos = transform.position;
        }
        bool shouldMove = false;
        float scrollAmount = ScrollSpeed * Time.deltaTime;

        Vector3 mousePos = Input.mousePosition;

        // Right border
        if (Input.GetKey(KeyCode.D))
        {
            pos.x += ScrollSpeed * Time.deltaTime;
            shouldMove = true;
        }
        // Left border
        else if (Input.GetKey(KeyCode.A))
        {
            pos.x -= ScrollSpeed * Time.deltaTime;
            shouldMove = true;
        }

        // Top border
        if (Input.GetKey(KeyCode.W))
        {
            pos.y += ScrollSpeed * Time.deltaTime;
            shouldMove = true;
        }
        // Bottom border
        else if (Input.GetKey(KeyCode.S))
        {
            pos.y -= ScrollSpeed * Time.deltaTime;
            shouldMove = true;
        }

        if (shouldMove)
        {
            if(_isAllMapCameraActive)
            {
                AllLevelCamera[_currentAllLevelCameraIndex].transform.position = pos;
            }
            else
            {
                transform.position = pos;
                lastScrollPosition = pos; 
            }
        }
    }

    public void ChangeAllCameraZone(int index)
    {
        if(index != _currentAllLevelCameraIndex)
        {
            if(CheckForAllLevelCameraTransition())
            {
                AllLevelCamera[_currentAllLevelCameraIndex].SetActive(false);
                AllLevelCamera[index].SetActive(true);
            }
            _currentAllLevelCameraIndex = index;
        }
    }

    private bool CheckForAllLevelCameraTransition()
    {
        return AllLevelCamera[_currentAllLevelCameraIndex].activeInHierarchy;
    }
         
}
