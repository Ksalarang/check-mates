﻿using GameScene.Controllers;
using GameScene.UI;
using UnityEngine;
using Zenject;

// ReSharper disable All

namespace GameScene {
public class GameInstaller : MonoInstaller {
    [Header("Controllers")]
    [SerializeField] GameObject controllers;
    [Header("UI")]
    [SerializeField] PromotionPanel promotionPanel;
    [SerializeField] ClickBlocker clickBlocker;
    [Header("Misc")]
    [SerializeField] GameSettings gameSettings;
    [SerializeField] new Camera camera;

    public override void InstallBindings() {
        // controllers
        bind(controllers.GetComponent<GameController>());
        bindWithInterfaces(controllers.GetComponent<BoardController>());
        bindWithInterfaces(controllers.GetComponent<SessionController>());
        bind(controllers.GetComponent<PieceSpriteProvider>());
        bind(controllers.GetComponent<PieceController>());
        // ui
        bind(promotionPanel);
        bind(clickBlocker);
        // settings
        bind(gameSettings);
        bind(gameSettings.log);
        // misc
        bind(camera);
    }
    
    void bind<T>(T instance) {
        Container.BindInstance(instance);
    }

    void bind<T>(T instance, object id) {
        Container.Bind<T>().WithId(id).FromInstance(instance);
    }
    
    void bindWithInterfaces<T>(T instance) {
        Container.BindInterfacesAndSelfTo<T>().FromInstance(instance);
    }
}
}