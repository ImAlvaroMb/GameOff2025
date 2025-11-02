using UnityEngine;

public class PhysicsPausable : MonoBehaviour, IPausable
{
    private Rigidbody2D _rigidBodyToPause;
    private Vector2 _originalVelocity;
    private float _originalAngularVelocity;
    private RigidbodyType2D _wasKinematic;

    private void Start()
    {
        _rigidBodyToPause = GetComponent<Rigidbody2D>();
        PauseManager.Instance.RegistedPausable(this);
    }

    private void OnDestroy()
    {
        PauseManager.Instance.UnregistedPausable(this);
    }

    public void OnPause()
    {
        if (_rigidBodyToPause == null) return;
        _originalVelocity = _rigidBodyToPause.linearVelocity;
        _originalAngularVelocity = _rigidBodyToPause.angularVelocity;
        _rigidBodyToPause.linearVelocity = Vector3.zero;
        _rigidBodyToPause.angularVelocity = 0f;
        _wasKinematic = _rigidBodyToPause.bodyType;
    }

    public void OnResume()
    {
        if (_rigidBodyToPause == null) return;
        _rigidBodyToPause.linearVelocity = _originalVelocity;
        _rigidBodyToPause.angularVelocity = _originalAngularVelocity;
        _rigidBodyToPause.bodyType = _wasKinematic;
    }
}