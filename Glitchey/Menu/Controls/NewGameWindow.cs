using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Gwen.Control;
using System.Drawing;
using System.IO;

namespace Glitchey.Menu.Controls
{
    class NewGameWindow : WindowControl
    {
        private Main _main;
        public NewGameWindow(Base parent, Main main)
            :base(parent, "New Game")
        {
            _main = main;

            int w_width = GameOptions.GetVariable("w_width");
            int w_height = GameOptions.GetVariable("w_height");
            int recwidth = 500;
            int recheight = 300;
            Rectangle rec = new Rectangle(w_width / 2 - recwidth / 2, w_height / 2 - recheight / 2, recwidth, recheight);
            SetBounds(rec);

            this.DisableResizing();

            SetupMapList(this);


            int buttonHeight = 30;

            int bottom = 236;
            Button btnOk = new Button(this);
            btnOk.Text = "Ok";
            btnOk.SetPosition(387, bottom);
            btnOk.SetSize(100, buttonHeight);
            btnOk.Clicked += btnOk_Clicked;


            Button btnCancel = new Button(this);
            btnCancel.Text = "Cancel";
            btnCancel.SetPosition(280, bottom);
            btnCancel.SetSize(100, buttonHeight);
            btnCancel.Clicked += btnCancel_Clicked;
        }

        private void btnCancel_Clicked(Base sender, ClickedEventArgs arguments)
        {
            this.Hide();
        }

        private void btnOk_Clicked(Base sender, ClickedEventArgs arguments)
        {
            StartGame();
        }

        ListBox _mapList;
        private void SetupMapList(Base parent)
        {
            _mapList = new ListBox(parent);
            _mapList.SetPosition(0, 0);
            _mapList.SetSize(490, 230);

            string[] files = Directory.GetFiles("Content/levels");

            foreach (string file in files)
            {
                int start = file.LastIndexOf("\\") + 1;
                int length = file.LastIndexOf(".") - start;
                string name = file.Substring(start, length);
                _mapList.AddRow(name);
            }

        }



        private void StartGame()
        {
            this.Hide();
            _main.StartGame(_mapList.SelectedRow.Text);
            GameActions.ChangeScreenState(ScreenState.Game);
        }

    }
}
