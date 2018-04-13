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
    public class Game : Form {
        #region Vars
        private MsLabel[,] labels;
        private Graphics graphics;
        private int countBombs; 
        #endregion  
        #region Init stuff
        public Game() {
            // init
            initForm();
            graphics = CreateGraphics();
            createLabels(new Point(30, 70), new Size(65, 65), 10, 10);
            setBombs((int)Math.Round((decimal)((labels.GetLength(0) * 0.5) * (labels.GetLength(1) * 0.5) * 0.65)));
            Location = new Point(Location.X - (int)(Width * 0.5), Location.Y - (int)(Height * 0.5));
            // Calculate how many bombs are in the game and set the lab value
            countBombs = 0;
            for (int i = 0; i < labels.GetLength(0); i++) { 
                for (int j = 0; j < labels.GetLength(1); j++) { 
                    if (labels[i, j].Bomb) countBombs++;
                    checkBombs(i, j);
                }
            }
        } 
        private void initForm() { 
            ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Game));
            SuspendLayout(); 
            ClientSize = new Size(0, 0);
            StartPosition = FormStartPosition.CenterScreen;
            Name = "Game";
            Text = "Minesweeper";
            BackColor = Color.LightGray;
            Icon = global::MineSweeper.Properties.Resources.favicon;
            Paint += new PaintEventHandler(this.Redraw);
            Resize += new EventHandler(this.createNewGraphics);
            ResumeLayout(false); 
        }
        private void createLabels(Point startPoint, Size size, int width, int height) { 
            labels = new MsLabel[width, height];
            int counter = 0;
            // init every label
            for (int i = 0; i < labels.GetLength(0); i++) {
                for (int j = 0; j < labels.GetLength(1); j++) {
                    labels[i,j] = new MsLabel(i ,j);
                    labels[i, j].Location = new Point(
                        startPoint.X + j * size.Width, 
                        startPoint.Y + i * size.Height);
                    labels[i, j].Name = "msLabel" + counter.ToString();
                    labels[i, j].Size = size; 
                    labels[i, j].TabIndex = labels[i, j].Value; 
                    labels[i, j].MouseClick += new MouseEventHandler(mouseClick);
                    Controls.Add(labels[i, j]);
                    counter++;
                }
            } 
            // set the form size
            Width = labels[labels.GetLength(0) - 1, labels.GetLength(1) - 1].Location.X +
                labels[labels.GetLength(0) - 1, labels.GetLength(1) - 1].Size.Width + 45;
            Height = labels[labels.GetLength(0) - 1, labels.GetLength(1) - 1].Location.Y +
                labels[labels.GetLength(0) - 1, labels.GetLength(1) - 1].Size.Height + 65;
        }
        #endregion
        #region Display stuff
        private void Redraw(object sender, PaintEventArgs e) {
            graphics.DrawString("Bombs:  " + countBombs, 
                new Font("Comic Sans MS", 16), new SolidBrush(Color.Black), new PointF(35f, 30f));
        } 
        private void createNewGraphics(object sender, EventArgs e) {
            graphics = CreateGraphics();  
            StartPosition = FormStartPosition.CenterScreen;
        }
        #endregion 
        #region Gamelogic  
        private void mouseClick(object sender, MouseEventArgs mouse) {
            MsLabel selectedLabel = (MsLabel)sender;
            selectedLabel.MouseClick -= mouseClick;

            // Set flag and reval the value
            if (mouse.Button.ToString().Equals("Right")) {
                selectedLabel.Flag = true;
            } else if (selectedLabel.Bomb) { 
                for (int i = 0; i < labels.GetLength(0); i++) {
                    for (int j = 0; j < labels.GetLength(1); j++) {
                        labels[i, j].revealAll();
                        labels[i, j].MouseClick -= mouseClick;
                    }
                }
            }
            selectedLabel.selectLab();
            if (selectedLabel.Value == 0 && !selectedLabel.Flag) {
                checkNull(selectedLabel.X, selectedLabel.Y);
            }
        } 
        private void setBombs(int countBombs) {
            int setBombs = 1;
            while (setBombs != countBombs) { 
                bool retry = true;
                do {
                    int x = new Random().Next(0, labels.GetLength(0) - 1);
                    int y = new Random().Next(0, labels.GetLength(1) - 1);

                    if (!labels[x, y].Bomb) {
                        retry = false;
                        labels[x, y].Bomb = true;
                    }
                } while (retry);
                setBombs++;
            }
        }
        private void checkBombs(int x, int y) {
            #region Hand
            // Left hand
            try
            {
                if (labels[x - 1, y].Bomb) labels[x, y].Value = labels[x, y].Value + 1;
            }
            catch (Exception e) { }
            // Right hand
            try
            {
                if (labels[x + 1, y].Bomb) labels[x, y].Value = labels[x, y].Value + 1;
            }
            catch (Exception e) { }
            #endregion
            #region Top
            // Top left
            try
            {
                if (labels[x - 1, y - 1].Bomb) labels[x, y].Value = labels[x, y].Value + 1;
            } catch (Exception e) { }
            // Top
            try {
                if (labels[x, y - 1].Bomb) labels[x, y].Value = labels[x, y].Value + 1;
            } catch (Exception e) { }
            // Top right
            try {
                if (labels[x + 1, y - 1].Bomb) labels[x, y].Value = labels[x, y].Value + 1;
            } catch (Exception e) { }
            #endregion
            #region Bottom
            // Bottom left
            try {
                if (labels[x - 1, y + 1].Bomb) labels[x, y].Value = labels[x, y].Value + 1;
            } catch (Exception e) { }
            // Bottom
            try {
                if (labels[x, y + 1].Bomb) labels[x, y].Value = labels[x, y].Value + 1;
            } catch (Exception e) { }
            // Bottom right
            try {
                if (labels[x + 1, y + 1].Bomb) labels[x, y].Value = labels[x, y].Value + 1;
            } catch (Exception e) { }
            #endregion
        }
        private void checkNull(int x, int y) { 
            // Left 
            try {
                if (labels[x - 1, y].Value == 0) labels[x - 1, y].Text = "0"; 
            } catch (Exception e) { }
            // Right 
            try {
                if (labels[x + 1, y].Value == 0) labels[x + 1, y].Text = "0";
            } catch (Exception e) { } 
            // Top
            try {
                if (labels[x, y - 1].Value == 0) labels[x, y - 1].Text = "0";
            } catch (Exception e) { }  
            // Bottom
            try {
                if (labels[x, y + 1].Value == 0) labels[x, y + 1].Text = "0";
            } catch (Exception e) { }  
        }
        #endregion 
    }
}
