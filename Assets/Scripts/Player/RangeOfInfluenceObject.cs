using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Utilities;

public class RangeOfInfluenceObject : MonoBehaviour
{
    public GameObject[] _areaSprites;
    [SerializeField] private float influenceAreaRadius;

    private int _npcLayerID;

    [Header("Visuals")]
    [SerializeField] private float pulseDuration;
    [SerializeField] private float minAlpha;
    [SerializeField] private float maxAlpha;
    [SerializeField] private float minScaleFactor = 0.8f;
    [SerializeField] private float maxScaleFactor = 1.2f;
    [SerializeField] private float rotationSpeed = 90f;

    private Vector3 _originalScale;
    private Vector3 _minScale;
    private Vector3 _maxScale;
    private bool _isPulsing = false;
    private CircleCollider2D _collider;

    private void Start()
    {
        //_originalScale = _areaSprites.transform.localScale;

        //_minScale = _originalScale * minScaleFactor;
        //_maxScale = _originalScale * maxScaleFactor;

        _npcLayerID = LayerMask.NameToLayer("NPC");

        _collider = GetComponent<CircleCollider2D>();
        InitializeCollider();
        StartCoroutine(PulseAlpha());
    }

    private void InitializeCollider()
    {
        TimerSystem.Instance.CreateTimer(0.15f, onTimerDecreaseComplete: () =>
        {
            _collider.radius = influenceAreaRadius;
        }, onTimerDecreaseUpdate: (progress) =>
        {
            float timeElapsed = 0.15f - progress;
            float t = timeElapsed / 0.15f;
            _collider.radius = Mathf.Lerp(0, influenceAreaRadius, t);
        });
    }

    private void Update()
    {

    }

    #region Area Visuals

    private IEnumerator PulseAlpha()
    {
        _isPulsing = true;

        while (_isPulsing)
        {
            yield return StartCoroutine(LerpAlpha(minAlpha, maxAlpha, pulseDuration));
            yield return StartCoroutine(LerpAlpha(maxAlpha, minAlpha, pulseDuration));
        }
    }

    private IEnumerator LerpAlpha(float startAlpha, float endAlpha, float duration)
    {
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            foreach (var obj in _areaSprites)
            {
                if (obj != null)
                {
                    var sr = obj.GetComponent<SpriteRenderer>();
                    if (sr != null)
                    {
                        Color c = sr.color;
                        c.a = alpha;
                        sr.color = c;
                    }
                }
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure exact end value
        foreach (var obj in _areaSprites)
        {
            if (obj != null)
            {
                var sr = obj.GetComponent<SpriteRenderer>();
                if (sr != null)
                {
                    Color c = sr.color;
                    c.a = endAlpha;
                    sr.color = c;
                }
            }
        }
    }

    private IEnumerator PulseScale()
    {
        _isPulsing = true;

        while (_isPulsing)
        {
            yield return StartCoroutine(ScaleToTarget(_minScale, _maxScale, pulseDuration));
            yield return StartCoroutine(ScaleToTarget(_maxScale, _minScale, pulseDuration));
        }
    }

    private IEnumerator ScaleToTarget(Vector3 startScale, Vector3 endScale, float duration)
    {
        float timeElapsed = 0f;

        while(timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            //_areaSprites.transform.localScale = Vector3.Lerp(startScale, endScale, t);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        //_areaSprites.transform.localScale = endScale;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == _npcLayerID)
        {
            collision.GetComponentInParent<NPCController>().SetIsInAreaOfInfluence(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == _npcLayerID)
        {
            collision.GetComponentInParent<NPCController>().SetIsInAreaOfInfluence(false);
        }
    }

    #endregion
}
