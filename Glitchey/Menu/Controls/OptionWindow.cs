using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Glitchey.Rendering;

using Gwen.Control;
using System.Drawing;

namespace Glitchey.Menu.Controls
{
    class OptionWindow : WindowControl
    {
        public OptionWindow(Base parent)
            :base(parent, "Options")
        {
            _ChangedControls = new HashSet<Base>();

            int w_width = GameOptions.GetVariable("w_width");
            int w_height = GameOptions.GetVariable("w_height");
            int recwidth = 500;
            int recheight = 500;
            Rectangle rec = new Rectangle(w_width / 2 - recwidth / 2, w_height / 2 - recheight / 2, recwidth, recheight);
            SetBounds(rec);

            this.DisableResizing();

            LoadTabs();

            int buttonHeight = 30;

            int bottom = 436;
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

            Button btnApply = new Button(this);
            btnApply.Text = "Apply";
            btnApply.SetPosition(173, bottom);
            btnApply.SetSize(100, buttonHeight);
            btnApply.Clicked += btnApply_Clicked;

            CancelValues();
        }

        void btnApply_Clicked(Base sender, ClickedEventArgs arguments)
        {
            this.ApplyValues();
            this.CancelValues();
        }

        void btnCancel_Clicked(Base sender, ClickedEventArgs arguments)
        {
            this.Hide();
            this.CancelValues();
        }

        void btnOk_Clicked(Base sender, ClickedEventArgs arguments)
        {
            this.ApplyValues();
            this.Hide();
            this.CancelValues();
        }

        private void ApplyValues()
        {
           

            // video
            if (_ChangedControls.Contains(cboResolution))
            {
                GameResolution res = (GameResolution)cboResolution.SelectedItem.UserData;
                GameOptions.SetVariable("w_width", res.Width, true);
                GameOptions.SetVariable("w_height", res.Height);
            }
            if (_ChangedControls.Contains(slTesselation))
                GameOptions.SetVariable("r_tesselation",(int)slTesselation.Value);
            if (_ChangedControls.Contains(cboDisplay))
                GameOptions.SetVariable("w_state", (int)cboDisplay.SelectedItem.UserData);

            //debug
            int val = chkPhysDebug.IsChecked ? 1 : 0;
            if (_ChangedControls.Contains(chkPhysDebug))
                GameOptions.SetVariable("phys_debug", val);

            


        }

        private void CancelValues()
        {
            // Video
            slTesselation.Value = GameOptions.GetVariable("r_tesselation");
            cboDisplay.SelectByUserData(GameOptions.GetVariable("w_state"));
            cboResolution.SelectByUserData(_resolutions.Where(o => o.Width == GameOptions.GetVariable("w_width") && o.Height == GameOptions.GetVariable("w_height")).ToList().First());

            // debug
            int phys = GameOptions.GetVariable("phys_debug");
            chkPhysDebug.IsChecked = phys == 1;

            _ChangedControls = new HashSet<Base>();
        }

        private void LoadTabs()
        {
            TabControl tab = new TabControl(this);
            tab.SetSize(Width - 12, Height- 67);
            tab.SetPosition(0, 0);

            TabButton video = tab.AddPage("video");
            Base page = video.Page;
            page.SetPosition(0, 0);
            page.SetSize(Width, Height);
            FillVideoTab(page);


            TabButton debug = tab.AddPage("debug");
            page = debug.Page;
            page.SetPosition(0, 0);
            page.SetSize(Width, Height);
            FillDebugTab(page);
            

        }

        HashSet<Base> _ChangedControls;
        private void ValueChange(Base sender, EventArgs e)
        {
            if (!_ChangedControls.Contains(sender))
                _ChangedControls.Add(sender);
        }

        ComboBox cboDisplay;
        ComboBox cboResolution;
        List<GameResolution> _resolutions;
        HorizontalSlider slTesselation;
        private void FillVideoTab(Base tab)
        {
            int comboWidth = 170;
            int comboHeight = 30;

            int top = 30;
            int left = 25;

            Gwen.Padding pad = new Gwen.Padding(7, 0, 0, 0);

            Label lblresolution = new Label(tab);
            lblresolution.Text = "Resolution";
            lblresolution.SetPosition(left, top);

            cboResolution = new ComboBox(tab);
            cboResolution.SetSize(comboWidth, comboHeight);
            cboResolution.SetPosition(left, top + 20);
            cboResolution.TextPadding = pad;
            cboResolution.ItemSelected += ValueChange;
           
            

            Label lblDisplay = new Label(tab);
            lblDisplay.Text = "Display Mode";
            lblDisplay.SetPosition(left, top + 80);

            cboDisplay = new ComboBox(tab);
            cboDisplay.SetSize(comboWidth, comboHeight);
            cboDisplay.SetPosition(left, top + 100);
            cboDisplay.TextPadding = pad;
            cboDisplay.ItemSelected += ValueChange;



            Label lblSlider = new Label(tab);
            lblSlider.Text = "Patch Tesselation";
            lblSlider.SetPosition(270, top);

            Label lblSliderMin = new Label(tab);
            lblSliderMin.Text = "Low";
            lblSliderMin.SetPosition(270, top + 45);
            Label lblSliderMax = new Label(tab);
            lblSliderMax.Text = "High";
            lblSliderMax.SetPosition(415, top + 45);

            slTesselation = new HorizontalSlider(tab);
            slTesselation.SetPosition(270, top + 20);
            slTesselation.SetSize(170, 20);
            slTesselation.SetRange(2, 15);
            slTesselation.SnapToNotches = true;
            slTesselation.NotchCount = 14;
            slTesselation.ValueChanged += ValueChange;
            

            _resolutions = GameRenderer.GetResolutions();
            foreach (GameResolution r in _resolutions)
            {
                string label = r.Width + " x " + r.Height;
                cboResolution.AddItem(label, label, r);
            }


            string lblfull = "Fullscreen";
            int fullValue = (int)OpenTK.WindowState.Fullscreen;
            cboDisplay.AddItem(lblfull,lblfull,fullValue);

            string lblwindow = "Windowed";
            int windowValue = (int)OpenTK.WindowState.Normal;
            cboDisplay.AddItem(lblwindow, lblwindow, windowValue);


        }
        LabeledCheckBox chkPhysDebug;
        private void FillDebugTab(Base tab)
        {
            int top = 30;
            int left = 25;
            Gwen.Padding pad = new Gwen.Padding(7, 0, 7, 0);

            chkPhysDebug = new LabeledCheckBox(tab);
            chkPhysDebug.SetPosition(left, top);
            chkPhysDebug.Text = " phys_debug";
            chkPhysDebug.CheckChanged += ValueChange;
            

        }


        


        public override void Show()
        {
            base.Show();
            int w_width = GameOptions.GetVariable("w_width");
            int w_height = GameOptions.GetVariable("w_height");
            SetPosition(w_width / 2 - Width / 2, w_height / 2 - Height/2);
        }
    }
}
