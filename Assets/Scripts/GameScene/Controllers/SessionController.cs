using System.Collections.Generic;
using GameScene.Models;
using GameScene.Models.BoardModel;
using GameScene.Models.Pieces;
using UnityEngine;
using Utils;
using Zenject;

namespace GameScene.Controllers {
public class SessionController : MonoBehaviour {
    public Player currentPlayer;

    [Inject] BoardController board;
    [Inject] LogSettings logSettings;
    [Inject] PieceController pieceController;

    Log log;
    Player playerOne;
    Player playerTwo;
    Piece selectedPiece;
    List<Square> availableSquares;

    #region Awake
    void Awake() {
        log = new Log(GetType(), logSettings.sessionController);
        playerOne = new Player();
        playerTwo = new Player();
        availableSquares = new List<Square>();
        
        board.squareClick += onSquareClick;
    }
    #endregion

    void onSquareClick(Square square) {
        log.log($"click {square}");
        if (selectedPiece == null) {
            if (square.currentPiece is not null && currentPlayer.isWhite == square.currentPiece.isWhite) {
                onPieceSelected(square);
            }
        } else {
            if (square.currentPiece is not null && currentPlayer.isWhite == square.currentPiece.isWhite) {
                onPieceSelected(square);
            } else {
                // move the selected piece to the selected square
            }
        }
    }

    void onPieceSelected(Square square) {
        selectedPiece = square.currentPiece;
        
        setAvailableSquaresVisible(false);
        availableSquares.Clear();
        availableSquares = selectedPiece.getAvailableSquares();
        setAvailableSquaresVisible(true);
        log.log($"onPieceSelected: available square count: {availableSquares.Count}");
    }

    void setAvailableSquaresVisible(bool visible) {
        foreach (var square in availableSquares) {
            square.setOutlineVisible(visible);
        }
    }

    public void startNewSession() {
        var isPlayerOneWhite = true;
        playerOne.isWhite = isPlayerOneWhite;
        playerTwo.isWhite = !isPlayerOneWhite;
        
        setPiecesSides(isPlayerOneWhite);

        currentPlayer = isPlayerOneWhite ? playerOne : playerTwo;
    }

    void setPiecesSides(bool isWhiteBottom) {
        foreach (var piece in pieceController.whitePieces) {
            piece.isBottom = isWhiteBottom;
        }
        foreach (var piece in pieceController.blackPieces) {
            piece.isBottom = !isWhiteBottom;
        }
    }
}
}