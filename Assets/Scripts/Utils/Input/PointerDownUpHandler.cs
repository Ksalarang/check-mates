using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils.Input {
public class PointerDownUpHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
    public Action onPointerDown;
    public Action onPointerUp;

    public void OnPointerDown(PointerEventData eventData) {
        onPointerDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData) {
        onPointerUp?.Invoke();
    }
}
}