using System;
using GameScene.Models.Pieces;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameScene.Models.BoardModel {
public class Square : MonoBehaviour, IPointerClickHandler {
    [SerializeField] public bool isWhite;
    [SerializeField] Color availableSquareColor;
    [SerializeField] Color selectedSquareColor;
    [SerializeField] SpriteRenderer selectionRenderer;

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

    public void removePiece() {
        currentPiece.currentSquare = null;
        currentPiece = null;
    }

    public bool hasPiece() => currentPiece is not null;

    public void setSquareAvailable(bool available) {
        selectionRenderer.color = availableSquareColor;
        selectionRenderer.gameObject.SetActive(available);
    }

    public void setSquareSelected(bool selected) {
        selectionRenderer.color = selectedSquareColor;
        selectionRenderer.gameObject.SetActive(selected);
    }

    public override string ToString() {
        return $@"{isWhite switch { true => "w", false => "b" }}_square{indices}";
    }
}
}