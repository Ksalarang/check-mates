using GameScene.Models.Pieces;
using UnityEngine;

namespace GameScene.Models {
public class Square : MonoBehaviour {
    public bool isLight;
    
    [HideInInspector] public new Transform transform;
    
    [HideInInspector] public Piece currentPiece;

    void Awake() {
        transform = base.transform;
    }

    public bool tryPlacingPiece(Piece piece) {
        if (hasPiece()) return false;

        currentPiece = piece;
        currentPiece.transform.localPosition = transform.localPosition;
        return true;
    }

    public bool hasPiece() => currentPiece is not null;
}
}