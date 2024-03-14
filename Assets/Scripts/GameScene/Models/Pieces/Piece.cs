using UnityEngine;

namespace GameScene.Models.Pieces {
public abstract class Piece : MonoBehaviour {
    public PieceType type;
    public bool isWhite;

    [HideInInspector] public SpriteRenderer spriteRenderer;

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        awake();
    }
    
    protected virtual void awake() {}
}

public enum PieceType {
    Pawn,
    Knight,
    Bishop,
    Rook,
    Queen,
    King
}
}