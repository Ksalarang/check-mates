using System;
using GameScene.Models.Pieces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameScene.Models.BoardModel {
public class Square : MonoBehaviour, IPointerClickHandler {
    [SerializeField] public bool isLight;
    [SerializeField] SpriteRenderer outline;
    
    [HideInInspector] public new Transform transform;
    
    [HideInInspector] public Piece currentPiece;
    [HideInInspector] public Vector2Int indices;

    public Action<Square> onClick;

    void Awake() {
        transform = base.transform;
    }

    public void OnPointerClick(PointerEventData eventData) {
        onClick?.Invoke(this);
    }

    public bool tryPlacingPiece(Piece piece) {
        if (hasPiece()) return false;

        currentPiece = piece;
        currentPiece.currentSquare = this;
        currentPiece.transform.localPosition = transform.localPosition;
        return true;
    }

    public bool hasPiece() => currentPiece is not null;

    public void setOutlineVisible(bool visible) {
        outline.gameObject.SetActive(visible);
    }

    public override string ToString() {
        var color = isLight ? "l" : "d";
        var piece = currentPiece is not null ? $", {currentPiece.name}" : "";
        return $"{color}_square {indices}{piece}";
    }
}
}