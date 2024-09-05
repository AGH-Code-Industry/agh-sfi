using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DeadFusion.GameCore
{
    public class ViewmodelSprintAnimation : MonoBehaviour
    {
        [SerializeField] CharacterMovement characterMovement;

        [SerializeField] AnimationCurve xPosition;
        [SerializeField] AnimationCurve yPosition;
        [SerializeField] AnimationCurve zPosition;
        [SerializeField] AnimationCurve xRotation;
        [SerializeField] AnimationCurve yRotation;
        [SerializeField] AnimationCurve zRotation;

        Vector3 sprintPosition;
        Quaternion sprintRotation;
        float timer;
        float lerpTimer;
        [SerializeField] float lerpSpeed;
        [SerializeField] float cycleSpeed;
        bool isSprinting;

        private void Update()
        {
            isSprinting = characterMovement.IsRunning();

            lerpTimer = Mathf.MoveTowards(lerpTimer, isSprinting ? 1f : 0f, Time.deltaTime * lerpSpeed);
            timer = isSprinting ? timer + Time.deltaTime * cycleSpeed : Mathf.MoveTowards(timer % 1f - (timer % 1f > 0.5f ? 1f : 0f), 0f, Time.deltaTime * cycleSpeed);

            sprintPosition = new Vector3(xPosition.Evaluate(timer), yPosition.Evaluate(timer), zPosition.Evaluate(timer));
            sprintRotation = Quaternion.Euler(xRotation.Evaluate(timer), yRotation.Evaluate(timer), zRotation.Evaluate(timer));

            transform.localPosition = Vector3.Lerp(Vector3.zero, sprintPosition, lerpTimer);
            transform.localRotation = Quaternion.Slerp(Quaternion.identity, sprintRotation, lerpTimer);
        }
    }
}