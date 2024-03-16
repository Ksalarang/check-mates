using GameScene.Controllers;
using GameScene.Models.BoardModel;
using UnityEngine;
using Utils;
using Zenject;

namespace GameScene {
public class GameController : MonoBehaviour {
    [Inject] BoardController boardController;
    [Inject] PieceController pieceController;
    [Inject] SessionController sessionController;
    
    Log log;

    void Awake() {
        log = new Log(GetType());
    }

    void Start() {
        boardController.createBoard();
        pieceController.createPieces();
        sessionController.startNewSession();
    }
}
}