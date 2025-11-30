using System.Collections;
using UnityEngine;

public class AirVentInteractable : BaseInteractable
{
    [SerializeField] private Transform firstPoint;
    [SerializeField] private Transform secondPoint;

    [SerializeField] private float travelDuration = 1.5f;

    private bool isVentBlocked = false;
    public override void Highlight()
    {
        base.Highlight();
    }

    public override void Dehighlight()
    {
        base.Dehighlight();
    }

    public override void Interact(NPCController interactingNPC)
    {
        base.Interact(interactingNPC);
        if(!interactingNPC.gameObject.GetComponent<NPCAwarness>().IsTeacher())
        {
            StartCoroutine(MoveInterctor(interactingNPC.transform));
        }
    }

    private Transform DetectClosestPoint(Transform interactorPos)
    {
        float distToFirst = (interactorPos.position - firstPoint.position).sqrMagnitude;
        float distToSecond = (interactorPos.position - secondPoint.position).sqrMagnitude;

        if (distToFirst < distToSecond)
        {
            return firstPoint;
        }
        else
        {
            return secondPoint;
        }
    }

    private IEnumerator MoveInterctor(Transform interactorTransform)
    {
        isVentBlocked = true;

        Transform entryPoint = DetectClosestPoint(interactorTransform);
        Transform exitPoint = (entryPoint == firstPoint) ? secondPoint : firstPoint;

        Vector3 startPosition = entryPoint.position;
        Vector3 targetPosition = exitPoint.position;
        float elapsedTime = 0f;

        while (elapsedTime < travelDuration)
        {
            float t = elapsedTime / travelDuration;

            interactorTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        interactorTransform.position = targetPosition;
        isVentBlocked = false;
    }


}
