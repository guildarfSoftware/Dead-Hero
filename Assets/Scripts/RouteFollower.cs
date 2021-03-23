using System;
using UnityEngine;

public class RouteFollower : MonoBehaviour
{
    [SerializeField] Transform stepsContainer;
    [SerializeField] float speed;
    int currentStep;
    Vector3 targetPosition;

    private void Start()
    {
        targetPosition = transform.position;
    }
    private void Update()
    {
        if (arrivedAtDestination())
        {
            if (stepsContainer.childCount > 1)
            {
                currentStep++;
                if (currentStep >= stepsContainer.childCount)
                {
                    currentStep = 0;
                }

                targetPosition = stepsContainer.GetChild(currentStep).position;
            }
        }

        Vector3 moveDirection = targetPosition - transform.position;

        transform.Translate(moveDirection.normalized * speed * Time.deltaTime);

    }

    private bool arrivedAtDestination()
    {
        const float errorMargin = 0.1f;

        float distance = Vector3.Distance(targetPosition, transform.position);

        return distance < errorMargin;
    }
}