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

    void onSquareClick(Square clickedSquare) {
        log.log($"clicked {clickedSquare}");
        
        if (selectedPiece is null) { // a piece is not selected yet
            if (clickedSquare.currentPiece is not null) { // clicked square has a piece
                if (currentPlayer.isWhite == clickedSquare.currentPiece.isWhite) { // clicked piece belongs to current player
                    onPieceSelected(clickedSquare);
                }
            }
        } else { // a piece is already selected
            if (clickedSquare.currentPiece is not null) { // clicked square has a piece
                if (currentPlayer.isWhite == clickedSquare.currentPiece.isWhite) { // clicked piece belongs to current player
                    onPieceSelected(clickedSquare);
                } else { // clicked piece belongs to opponent
                    if (availableSquares.Contains(clickedSquare)) {
                        onPieceCaptured(clickedSquare);
                        onTurnEnded();
                    }
                }
            } else { // clicked piece is empty
                if (availableSquares.Contains(clickedSquare)) {
                    onPieceMoved(clickedSquare);
                    onTurnEnded();
                }
            }
        }
    }

    void onPieceSelected(Square square) {
        selectedPiece = square.currentPiece;
        
        clearAvailableSquares();
        availableSquares = selectedPiece.getAvailableSquares();
        setAvailableSquaresVisible(true);
        log.log($"onPieceSelected: available square count: {availableSquares.Count}");
    }

    void onPieceMoved(Square square) {
        log.log($"{selectedPiece} is moved to {square}");
        movePieceToSquare(selectedPiece, square);
    }

    void onPieceCaptured(Square square) {
        log.log($"{selectedPiece} captured {square.currentPiece}");
        capturePieceAtSquare(square);
        movePieceToSquare(selectedPiece, square);
    }

    void onTurnEnded() {
        log.log($"{currentPlayer}'s turn ended");
        clearAvailableSquares();
        currentPlayer = currentPlayer == playerOne ? playerTwo : playerOne;
        log.log($"{currentPlayer} is current");
    }

    void clearAvailableSquares() {
        setAvailableSquaresVisible(false);
        availableSquares.Clear();
    }
    
    void setAvailableSquaresVisible(bool visible) {
        foreach (var square in availableSquares) {
            square.setOutlineVisible(visible);
        }
    }

    void movePieceToSquare(Piece piece, Square square) {
        piece.currentSquare.removePiece();
        square.tryPlacingPiece(piece);
    }

    void capturePieceAtSquare(Square square) {
        square.currentPiece.gameObject.SetActive(false);
        square.removePiece();
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