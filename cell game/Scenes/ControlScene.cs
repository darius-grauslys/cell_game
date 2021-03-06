using cell_game.Gameplay;
using isometricgame.GameEngine;
using isometricgame.GameEngine.Events.Arguments;
using isometricgame.GameEngine.Scenes;
using isometricgame.GameEngine.Systems.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cell_game.Scenes
{
    public class ControlScene : Scene
    {
        private ChildScene activeScene;

        private GameScene gameScene;
        private ChildScene[] gameScenes;

        public ControlScene(Game game, int initalScene=0) 
            : base(game)
        {
            gameScene = new GameScene(game, this);

            gameScenes = new ChildScene[]
            {
                new MainMenuScene(game, this),
                new GameCreationScene(game, this),
                new TutorialScene(game, this),
                gameScene
            };
            activeScene = gameScenes[0];
        }

        public override void UpdateFrame(FrameArgument e)
        {
            activeScene.UpdateFrame(e);
        }

        public override void RenderFrame(RenderService renderService, FrameArgument e)
        {
            activeScene.RenderFrame(renderService, e);
        }

        public void TransitionScene(int id)
        {
            activeScene.ExitScene();
            activeScene = gameScenes[id];
            activeScene.EnterScene();
        }

        public void BeginGame(List<Player> players, int width, int height)
        {
            gameScene.SetLevel(players, width, height);
            TransitionScene(gameScenes.Length - 1);
        }
    }
}
