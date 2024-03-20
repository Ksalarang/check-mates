using System.Collections.Generic;
using System.Linq;
using GameScene.Models;
using GameScene.Models.BoardModel;
using GameScene.Models.Pieces;
using GameScene.Models.SessionModel;
using UnityEngine;
using Utils;
using Zenject;

namespace GameScene.Controllers {
public class SessionController : MonoBehaviour, Session {
    [Inject] BoardController board;
    [Inject] LogSettings logSettings;
    [Inject] PieceController pieceController;

    public event OnPawnPromotion onPawnPromotion;
    
    Log log;
    Player playerOne;
    Player playerTwo;
    Player currentPlayer;
    Piece selectedPiece;
    Turn currentTurn;
    
    List<Square> availableSquares;
    List<Turn> turns;

    #region Awake
    void Awake() {
        log = new Log(GetType(), logSettings.sessionController);
        playerOne = new Player();
        playerTwo = new Player();
        
        availableSquares = new List<Square>();
        turns = new List<Turn>();
        
        board.squareClick += onSquareClick;
    }
    #endregion

    #region Turns
    void onSquareClick(Square clickedSquare) {
        log.log($@"clicked {clickedSquare}{clickedSquare.hasPiece() switch {
                true => $" with {clickedSquare.currentPiece}",
                false => "" }
        }");
        
        if (selectedPiece is null) { // a piece is not selected yet
            if (clickedSquare.hasPiece()) { // clicked square has a piece
                if (currentPlayer.isWhite == clickedSquare.currentPiece.isWhite) { // clicked piece belongs to current player
                    onPieceSelected(clickedSquare);
                }
            }
        } else { // a piece is already selected
            if (clickedSquare.hasPiece()) { // clicked square has a piece
                if (currentPlayer.isWhite == clickedSquare.currentPiece.isWhite) { // clicked piece belongs to current player
                    onPieceSelected(clickedSquare);
                } else { // clicked piece belongs to opponent
                    if (availableSquares.Contains(clickedSquare)) {
                        beforeTurnEnd();
                        onPieceCaptured(clickedSquare);
                        onTurnEnded();
                    }
                }
            } else { // clicked piece is empty
                if (availableSquares.Contains(clickedSquare)) {
                    beforeTurnEnd();
                    checkEnPassantCapture(clickedSquare);
                    onPieceMoved(clickedSquare);
                    onTurnEnded();
                }
            }
        }
    }

    void onPieceSelected(Square square) {
        // ReSharper disable once Unity.NoNullPropagation
        selectedPiece?.currentSquare.setSquareSelected(false);
        selectedPiece = square.currentPiece;
        selectedPiece.currentSquare.setSquareSelected(true);
        
        clearAvailableSquares();
        availableSquares = selectedPiece.getAvailableSquares();
        setAvailableSquaresVisible(true);
        
        log.log($"{selectedPiece} is selected, available squares: {availableSquares.Count}");
    }

    void onPieceMoved(Square square) {
        log.log($"{selectedPiece} is moved to {square}");
        movePieceToSquare(selectedPiece, square);
    }

    void onPieceCaptured(Square square) {
        log.log($"{selectedPiece} captured {square.currentPiece} on {square}");
        currentTurn.capturedPiece = square.currentPiece;
        capturePiece(square.currentPiece);
        movePieceToSquare(selectedPiece, square);
    }

    void beforeTurnEnd() {
        selectedPiece.currentSquare.setSquareSelected(false);
        
        currentTurn.piece = selectedPiece;
        currentTurn.startSquare = selectedPiece.currentSquare;
    }

    void onTurnEnded() {
        if (!isPawnPromoted()) {
            afterTurnEnded();
        } else {
            onPawnPromotion?.Invoke(selectedPiece.isWhite);
        }
    }

    void afterTurnEnded() {
        clearAvailableSquares();
        
        currentTurn.endSquare = selectedPiece.currentSquare;
        currentTurn.determineType();
        turns.Add(currentTurn);
        log.log(currentTurn);
        currentTurn = new Turn(turns.Count + 1);
        
        currentPlayer = currentPlayer == playerOne ? playerTwo : playerOne;
    }

    void clearAvailableSquares() {
        setAvailableSquaresVisible(false);
        availableSquares.Clear();
    }
    
    void setAvailableSquaresVisible(bool visible) {
        foreach (var square in availableSquares) {
            square.setSquareAvailable(visible);
        }
    }

    void movePieceToSquare(Piece piece, Square square) {
        piece.currentSquare.removePiece();
        square.tryPlacingPiece(piece);
    }

    void capturePiece(Piece piece) {
        removePieceFromPlayer(piece);
        piece.gameObject.SetActive(false);
        piece.currentSquare.removePiece();
    }

    Player getPlayer(Piece piece) {
        return playerOne.isWhite == piece.isWhite ? playerOne : playerTwo;
    }

    #region En Passant Check
    void checkEnPassantCapture(Square endSquare) {
        if (selectedPiece.type == PieceType.Pawn
            && selectedPiece.position.x != endSquare.indices.x
            && !endSquare.hasPiece()) {
            
            currentTurn.capturedPiece = getEnPassantPiece(selectedPiece, endSquare);
            currentTurn.type = Turn.Type.EnPassantCapture;
            capturePiece(currentTurn.capturedPiece);
        }
    }

    Piece getEnPassantPiece(Piece chargingPiece, Square endSquare) {
        var direction = endSquare.indices.x < chargingPiece.position.x ? PieceDirection.Left : PieceDirection.Right;
        if (!chargingPiece.isBottom) direction = direction.getOpposite();
        return chargingPiece.getSquareInDirection(direction).currentPiece;
    }
    #endregion

    #region Pawn promotion
    bool isPawnPromoted() {
        return selectedPiece.type == PieceType.Pawn
               && selectedPiece.currentSquare.indices.y == selectedPiece.getRelativeIndex(7);
    }
    
    public void onPromotedPieceSelected(PieceType type) {
        log.log($"{selectedPiece} promoted to {type}");

        var newPiece = pieceController.createPiece(type, selectedPiece.isWhite, selectedPiece.isBottom);
        var square = selectedPiece.currentSquare;
        selectedPiece.currentSquare.removePiece();
        square.tryPlacingPiece(newPiece);
        
        addPieceToPlayer(newPiece);
        removePieceFromPlayer(selectedPiece);
        selectedPiece.gameObject.SetActive(false);
        selectedPiece = newPiece;
        
        afterTurnEnded();
    }
    #endregion

    void addPieceToPlayer(Piece piece) {
        getPlayer(piece).pieces.Add(piece);
    }
    
    void removePieceFromPlayer(Piece piece) {
        getPlayer(piece).pieces.Remove(piece);
    }
    #endregion

    #region Interface
    #region Session
    public void startNewSession() {
        var isPlayerOneWhite = true;
        playerOne.isWhite = isPlayerOneWhite;
        playerTwo.isWhite = !isPlayerOneWhite;
        setPiecesSides(isPlayerOneWhite);
        
        playerOne.pieces.Clear();
        playerOne.pieces.AddRange(pieceController.getPieces(playerOne.isWhite));
        playerTwo.pieces.Clear();
        playerTwo.pieces.AddRange(pieceController.getPieces(playerTwo.isWhite));

        currentPlayer = isPlayerOneWhite ? playerOne : playerTwo;
        turns.Clear();
        currentTurn = new Turn(1);
    }

    void setPiecesSides(bool isWhiteBottom) {
        foreach (var piece in pieceController.whitePieces) {
            piece.isBottom = isWhiteBottom;
        }
        foreach (var piece in pieceController.blackPieces) {
            piece.isBottom = !isWhiteBottom;
        }
    }
    #endregion
    #endregion

    #region Session interface
    public Turn getLastTurn() => turns.LastOrDefault();
    #endregion
}

public delegate void OnPawnPromotion(bool isWhite);
}