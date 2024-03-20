using UnityEngine;

namespace GameScene.UI {
public class ClickBlocker : MonoBehaviour {
    public void show() {
        gameObject.SetActive(true);
    }

    public void hide() {
        gameObject.SetActive(false);
    }
}
}