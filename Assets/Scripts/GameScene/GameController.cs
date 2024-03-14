using GameScene.Controllers;
using UnityEngine;
using Utils;
using Zenject;

namespace GameScene {
public class GameController : MonoBehaviour {
    [Inject] BoardController boardController;
    
    Log log;

    void Awake() {
        log = new Log(GetType());
    }

    void Start() {
        
    }
}
}