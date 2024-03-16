using System;
using UnityEngine;

namespace GameScene {
public class GameSettings : MonoBehaviour {
    public const int BoardSize = 8;

    public LogSettings log;
}

[Serializable]
public class LogSettings {
    public bool gameController;
    public bool sessionController;
}
}