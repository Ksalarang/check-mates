using GameScene.Models;
using UnityEngine;
using Utils.Extensions;
using Zenject;

namespace GameScene.Controllers {
public class BoardController : MonoBehaviour {
    public const int BoardSize = 8;
    
    [Header("Prefabs")]
    [SerializeField] Transform boardContainer;
    [SerializeField] GameObject lightSquarePrefab;
    [SerializeField] GameObject darkSquarePrefab;

    [Inject] DiContainer diContainer;
    [Inject] new Camera camera;

    readonly Square[,] squareArray = new Square[BoardSize, BoardSize];
    float squareLength;

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
                
                squareArray[y, x] = square;
            }
        }
    }

    public Square getSquare(Vector2Int position) => getSquare(position.x, position.y);
    
    public Square getSquare(int x, int y) => squareArray[y, x];

    public float getSquareLength() => squareLength;
}
}