using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MineSweeper {
    public class Loadscreen : Form {
        private IContainer components;
        private Timer timer;
        private PictureBox load;
        private Game newGame;
    
        public Loadscreen() {
            init();
            newGame = new Game(new Size(15, 10));
            load.Image = global::MineSweeper.Properties.Resources.load;
            timer.Start();
        }
        private void init() {
            components = new Container();
            load = new PictureBox();
            timer = new Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.load)).BeginInit();
            SuspendLayout(); 

            load.Location = new Point(0, 0);
            load.Name = "load";
            load.Size = new Size(500, 310);
            load.SizeMode = PictureBoxSizeMode.StretchImage;
            load.TabIndex = 0;
            load.TabStop = false; 

            timer.Interval = 6350;
            timer.Tick += new EventHandler(this.startGame);

            ClientSize = load.Size;
            ControlBox = false;
            Controls.Add(this.load);
            ShowInTaskbar = false;
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "Loadscreen";
            StartPosition = FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.load)).EndInit();
            this.ResumeLayout(false);

        } 
        private void startGame(object sender, EventArgs e) {
            Visible = false;
            timer.Stop();
            timer.Dispose();
            newGame.Show();
        } 
    }
}
