using System;
using UnityEngine;

namespace Utils.Listeners {
public class Trigger2DListener : MonoBehaviour {
    public Action<Collider2D> onTriggerEnter;

    void OnTriggerEnter2D(Collider2D other) {
        onTriggerEnter?.Invoke(other);
    }
}
}