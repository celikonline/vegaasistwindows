using System;
using System.Drawing;
using System.Windows.Forms;

namespace VegaAsis.Windows.Forms
{
    public class PlaceholderForm : Form
    {
        public PlaceholderForm(string title)
        {
            Text = title;
            Size = new Size(500, 300);
            StartPosition = FormStartPosition.CenterParent;
            var lbl = new Label
            {
                Text = title + " ekranı yakında eklenecek.",
                AutoSize = true,
                Left = 20,
                Top = 20
            };
            Controls.Add(lbl);
        }
    }
}
