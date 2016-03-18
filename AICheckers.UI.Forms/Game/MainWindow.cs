using System;
using System.Drawing;
using System.Windows.Forms;

namespace IACheckers.UI.Forms.Game
{
    public class MainWindow : Form
    {
        private readonly CheckersBoardView _mView;

        public MainWindow()
        {
            Text = "IA - Checkers";
            ClientSize = new Size(700, 700);

            _mView = new CheckersBoardView()
            {
                Location = new Point(0, 0),
                Size = ClientSize
            };
            Controls.Add(_mView);
        }
        
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (_mView != null)
            {
                _mView.Size = ClientSize;
                _mView.Invalidate();
            }
        }
    }
}