using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Core;
using Glitchey.Entities;
using Glitchey.Systems;
using Glitchey.Rendering;

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
            _entityManager = new EntityManager();
            _systemManager = new SystemManager(_entityManager);

            LoadLevel();
            LoadSystems();

        }

        Camera _camera;
        private void LoadLevel()
        {
            World world = new World("levels/q3dm1.bsp");
            _entityManager.AddEntity(world);

            Camera cam = _camera = new Camera();
            _entityManager.AddEntity(cam);
        }

        private void LoadSystems()
        {
            // Input
            _systemManager.AddSystem(new InputSystem());

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
            GL.DepthMask(false);
            GL.UseProgram(0);
            GL.CullFace(CullFaceMode.Back);
            GL.Disable(EnableCap.Lighting);

            int offset = 40;
            int cpt = 0;
            DrawHelper.DrawString("Position : " + _camera.Position.PositionVec.ToString() , Content.LoadFont(font), Color.Yellow, new Vector2(0, cpt++ * offset), false);
            DrawHelper.DrawString("Rotation : " + _camera.Rotation.RotationVector.ToString(), Content.LoadFont(font), Color.Yellow, new Vector2(0, cpt++ * offset), false);

            GL.Enable(EnableCap.Lighting);
            GL.CullFace(CullFaceMode.Front);
            GL.DepthMask(true);
            
        }


    }
}
