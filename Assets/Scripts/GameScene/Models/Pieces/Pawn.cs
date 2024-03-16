using System.Collections.Generic;
using GameScene.Controllers;
using GameScene.Models.BoardModel;
using UnityEngine;

namespace GameScene.Models.Pieces {
public class Pawn : Piece {
    public override List<Square> getAvailableSquares() {
        var list = new List<Square>();
        var forward = getAbsoluteDirection(PieceDirection.Forward);
        
        // 1 step forward square
        if (isPathClear(forward, 1)) {
            list.Add(board.getSquare(position + forward));
        }
        // 2 steps forward square
        var startY = isBottom ? 1 : Board.getOppositeIndex(1);
        if (position.y == startY) {
            if (isPathClear(forward, 2)) {
                list.Add(board.getSquare(position + forward * 2));
            }
        }
        // capture squares
        var forwardLeftSquare = canCapturePiece(getAbsoluteDirection(PieceDirection.ForwardLeft));
        if (forwardLeftSquare is not null) {
            list.Add(forwardLeftSquare);
        }
        var forwardRightSquare = canCapturePiece(getAbsoluteDirection(PieceDirection.ForwardRight));
        if (forwardRightSquare is not null) {
            list.Add(forwardRightSquare);
        }
        
        //todo: en passant capture squares
        
        return list;
    }

    Square canCapturePiece(Vector2Int direction) {
        var square = board.getSquare(position + direction);
        return square is not null && square.hasPiece() && isWhite != square.currentPiece.isWhite ? square : null;
    }
}
}