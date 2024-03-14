using GameScene.Models;
using UnityEngine;
using Utils.Extensions;
using Zenject;

namespace GameScene.Controllers {
public class BoardController : MonoBehaviour {
    const int BoardSize = 8;
    
    [SerializeField] GameObject lightSquarePrefab;
    [SerializeField] GameObject darkSquarePrefab;
    [SerializeField] Transform container;

    [Inject] DiContainer diContainer;
    [Inject] new Camera camera;

    Square[,] squareArray = new Square[BoardSize, BoardSize];

    public void createBoard() {
        var cameraSize = camera.getSize();
        var minDimension = Mathf.Min(cameraSize.x, cameraSize.y);
        var boardLength = minDimension;
        var squareLength = boardLength / BoardSize;

        var center = new Vector3();
        var firstSquareOffset = (BoardSize / 2f) * squareLength - squareLength / 2f;
        var firstSquarePosition = new Vector3(center.x - firstSquareOffset, center.y - firstSquareOffset);

        for (var i = 0; i < BoardSize; i++) {
            for (var j = 0; j < BoardSize; j++) {
                var prefab = (i % 2 == 0 && j % 2 == 0) || (i % 2 != 0 && j % 2 != 0) 
                    ? darkSquarePrefab : lightSquarePrefab;
                var square = diContainer.InstantiatePrefabForComponent<Square>(prefab, container);
                square.transform.localScale = new Vector3(squareLength, squareLength);
                square.transform.localPosition = new Vector3(
                    firstSquarePosition.x + j * squareLength,
                    firstSquarePosition.y + i * squareLength
                );
                
                squareArray[i, j] = square;
            }
        }
    }
}
}