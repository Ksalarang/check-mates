using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Utils.Listeners {
public class ClickListener : MonoBehaviour, IPointerClickHandler {
    public Action onClick;

    public void OnPointerClick(PointerEventData eventData) {
        onClick?.Invoke();
    }
}
}