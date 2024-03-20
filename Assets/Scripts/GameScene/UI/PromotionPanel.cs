using System;
using System.Collections.Generic;
using GameScene.Controllers;
using GameScene.Models.Pieces;
using UnityEngine;
using UnityEngine.UI;
using Utils.Listeners;
using Zenject;

namespace GameScene.UI {
public class PromotionPanel : MonoBehaviour {
    [SerializeField] Image queenImage;
    [SerializeField] Image rookImage;
    [SerializeField] Image knightImage;
    [SerializeField] Image bishopImage;

    [Inject] SessionController sessionController;
    [Inject] PieceSpriteProvider spriteProvider;
    [Inject] ClickBlocker clickBlocker;

    List<Image> pieces;

    void Awake() {
        pieces = new List<Image> { queenImage, rookImage, knightImage, bishopImage };
        sessionController.onPawnPromotion += onPawnPromotion;
        addClickListeners();
        gameObject.SetActive(false);
    }

    void onPawnPromotion(bool white) {
        show();
        foreach (var piece in pieces) {
            piece.sprite = spriteProvider.getSprite(getPieceType(piece), white);
        }
    }

    void addClickListeners() {
        foreach (var piece in pieces) {
            addClickListener(piece);
        }
    }

    void addClickListener(Image image) {
        image.GetComponent<ClickListener>().onClick = () => {
            sessionController.onPromotedPieceSelected(getPieceType(image));
            hide();
        };
    }

    void show() {
        gameObject.SetActive(true);
        clickBlocker.show();
    }

    void hide() {
        gameObject.SetActive(false);
        clickBlocker.hide();
    }

    PieceType getPieceType(Image image) {
        if (image == queenImage) return PieceType.Queen;
        if (image == rookImage) return PieceType.Rook;
        if (image == knightImage) return PieceType.Knight;
        return PieceType.Bishop;
    } 
}
}