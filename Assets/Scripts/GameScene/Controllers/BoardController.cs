using System;
using GameScene.Models.BoardModel;
using UnityEngine;
using Utils.Extensions;
using Zenject;

namespace GameScene.Controllers {
public class BoardController : MonoBehaviour, Board {
    public const int BoardSize = 8;
    public event OnSquareClick squareClick;
    
    [Header("Prefabs")]
    [SerializeField] Transform boardContainer;
    [SerializeField] GameObject lightSquarePrefab;
    [SerializeField] GameObject darkSquarePrefab;

    [Inject] DiContainer diContainer;
    [Inject] new Camera camera;

    readonly Square[,] squareArray = new Square[BoardSize, BoardSize];
    float squareLength;

    #region Board creation
    public void createBoard() {
        var cameraSize = camera.getSize();
        var boardLength = Mathf.Min(cameraSize.x, cameraSize.y);
        squareLength = boardLength / BoardSize;

        var center = new Vector3();
        var firstSquareOffset = (BoardSize / 2f) * squareLength - squareLength / 2f;
        var firstSquarePosition = new Vector3(center.x - firstSquareOffset, center.y - firstSquareOffset);

        for (var y = 0; y < BoardSize; y++) {
            for (var x = 0; x < BoardSize; x++) {
                var prefab = (y % 2 == 0 && x % 2 == 0) || (y % 2 != 0 && x % 2 != 0) 
                    ? darkSquarePrefab : lightSquarePrefab;
                var square = diContainer.InstantiatePrefabForComponent<Square>(prefab, boardContainer);
                square.transform.localScale = new Vector3(squareLength, squareLength);
                square.transform.localPosition = new Vector3(
                    firstSquarePosition.x + x * squareLength,
                    firstSquarePosition.y + y * squareLength
                );
                square.onClick = onSquareClick;
                square.indices = new Vector2Int(x, y);
                
                squareArray[y, x] = square;
            }
        }
    }

    void onSquareClick(Square square) {
        squareClick?.Invoke(square);
    }
    #endregion

    public Square getSquare(Vector2Int indices) => getSquare(indices.x, indices.y);

    public Square getSquare(int x, int y) {
        if (x is < 0 or >= BoardSize || y is < 0 or >= BoardSize) return null;
        return squareArray[y, x];
    }

    public Square getSquareRelativeToOrigin(Vector2Int origin, Vector2Int direction, int steps) {
        if (getSquare(origin) is null) {
            throw new ArgumentException($"no square exists at {origin}");
        }
        return getSquare(origin + direction * steps);
    }

    public float getSquareLength() => squareLength;
}

public delegate void OnSquareClick(Square square);
}