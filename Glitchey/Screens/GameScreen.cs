using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Core;
using Glitchey.Entities;
using Glitchey.Systems;
using Glitchey.Rendering;

using QuickFont;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Glitchey.Screens
{
    class GameScreen : BaseScreen
    {

        public GameScreen()
        {
            Load();
            GameEvents.onStateChanged += GameEvents_onStateChanged;
            GameRenderer.SetupViewportGame();
        }

        void GameEvents_onStateChanged(object sender, StateChangedEventArgs e)
        {
            if(e.State == ScreenState.Game)
                GameRenderer.SetupViewportGame();
        }

        EntityManager _entityManager;
        SystemManager _systemManager;
        public override void Load()
        {

            LoadLevel();
            LoadSystems();

        }

        Player _camera;
        private void LoadLevel()
        {
            _entityManager = new EntityManager();
            _systemManager = new SystemManager(_entityManager);

            GameWorld world = new GameWorld("levels/q3dm8.bsp");
            _entityManager.AddEntity(world);

            float[] vec = new float[3];
            world.Level.BspFile.FindVectorByName("info_player_deathmatch", ref vec);

            Vector3 vector = BspRenderer.V3FromFloatArray(vec);

            Player cam = _camera = new Player(vector);
            _entityManager.AddEntity(cam);
        }

        private void LoadSystems()
        {
            // Input
            _systemManager.AddSystem(new InputSystem());

            // physics
            _systemManager.AddSystem(new PhysicSystem());

            //Rendering
            _systemManager.AddSystem(new CameraSystem());
            _systemManager.AddSystem(new RenderingSystem());

            
        }

        public override void Update()
        {
            _systemManager.Update();
        }

        public override void Render()
        {
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
            _systemManager.Render();
            DrawHud();
        }

        Font font = new Font("Consolas", 20, FontStyle.Regular);
        private void DrawHud()
        {
            string fontname = "Consolas";
            int size = 20;
            QFont font = Content.LoadFont(fontname,size);

            GL.DepthMask(false);
            GL.UseProgram(0);
            GL.CullFace(CullFaceMode.Back);

            int offset = 40;
            int cpt = 0;
            DrawHelper.DrawString("Position : " + _camera.Position.PositionVec.ToString(), font, Color.Yellow, new Vector2(0, cpt++ * offset), false);
            DrawHelper.DrawString("Rotation : " + _camera.Rotation.RotationVector.ToString(), font, Color.Yellow, new Vector2(0, cpt++ * offset), false);
            DrawHelper.DrawString("FPS : " + GameVariables.fps.ToString(), font, Color.Yellow, new Vector2(0, cpt++ * offset), false);

            GL.CullFace(CullFaceMode.Front);
            GL.DepthMask(true);

        }


    }
}
