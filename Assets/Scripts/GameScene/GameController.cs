using GameScene.Controllers;
using UnityEngine;
using Utils;
using Zenject;

namespace GameScene {
public class GameController : MonoBehaviour {
    [Inject] BoardController boardController;
    [Inject] PieceController pieceController;
    
    Log log;

    void Awake() {
        log = new Log(GetType());
    }

    void Start() {
        boardController.createBoard();
        pieceController.createPieces();
    }
}
}