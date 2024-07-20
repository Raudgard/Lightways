using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fields
{
    [RequireComponent(typeof(CircleCollider2D))]
    public class MarkCircleByClick : MonoBehaviour, IClickableObject
    {
        public SpriteRenderer circleSpriteRenderer;
        private bool isCircleActive = false;

        public void OnClick()
        {
            SetActiveForCircle(!isCircleActive);
            isCircleActive = !isCircleActive;
        }

        public void SetActiveForCircle(bool active) => circleSpriteRenderer.enabled = active;

    }
}