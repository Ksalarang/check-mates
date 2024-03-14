using GameScene.Controllers;
using UnityEngine;
using Zenject;

// ReSharper disable All

namespace GameScene {
public class GameInstaller : MonoInstaller {
    [Header("Controllers")]
    [SerializeField] GameObject controllers;
    [Header("Misc")]
    [SerializeField] GameSettings gameSettings;
    [SerializeField] new Camera camera;

    public override void InstallBindings() {
        // controllers
        bind(controllers.GetComponent<GameController>());
        bind(controllers.GetComponent<BoardController>());
        // settings
        bind(gameSettings);
        bind(camera);
    }
    
    void bind<T>(T instance) {
        Container.BindInstance(instance);
    }

    void bind<T>(T instance, object id) {
        Container.Bind<T>().WithId(id).FromInstance(instance);
    }
}
}