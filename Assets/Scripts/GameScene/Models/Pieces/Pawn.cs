using System.Collections.Generic;
using GameScene.Models.BoardModel;
using UnityEngine;
using Utils;

namespace GameScene.Models.Pieces {
public class Pawn : Piece {
    public override List<Square> getAvailableSquares() {
        var list = new List<Square>();
        var forward = getRelativeDirection(PieceDirection.Forward);
        
        // 1 step forward square
        if (isPathClear(forward, 1)) {
            list.Add(board.getSquare(position + forward));
        }
        // 2 steps forward square
        var startY = getRelativeIndex(1);
        if (position.y == startY) {
            if (isPathClear(forward, 2)) {
                list.Add(board.getSquare(position + forward * 2));
            }
        }
        // capture squares
        //todo: exclude squares that expose own king
        var forwardLeftSquare = getSquareForCapture(getRelativeDirection(PieceDirection.ForwardLeft));
        if (forwardLeftSquare is not null) {
            list.Add(forwardLeftSquare);
        }
        var forwardRightSquare = getSquareForCapture(getRelativeDirection(PieceDirection.ForwardRight));
        if (forwardRightSquare is not null) {
            list.Add(forwardRightSquare);
        }
        // en passant capture squares
        var enPassantRow = getRelativeIndex(4);
        if (position.y == enPassantRow) {
            var leftSquare = getSquareInDirection(PieceDirection.Left);
            if (leftSquare is not null && isEnPassant(leftSquare)) {
                list.Add(getSquareInDirection(PieceDirection.ForwardLeft));
            }
            var rightSquare = getSquareInDirection(PieceDirection.Right);
            if (rightSquare is not null && isEnPassant(rightSquare)) {
                list.Add(getSquareInDirection(PieceDirection.ForwardRight));
            }
        }
        checkAbsolutePin(list);
        return list;
    }

    Square getSquareForCapture(Vector2Int direction) {
        var square = board.getSquare(position + direction);
        return square is not null 
               && square.hasPiece() 
               && isWhite != square.currentPiece.isWhite
               && square.currentPiece.type != PieceType.King
            ? square
            : null;
    }

    bool isEnPassant(Square square) {
        var lastTurn = session.getLastTurn();
        return square.indices.x == lastTurn.endSquare.indices.x && lastTurn.isEnPassant();
    }
}
}