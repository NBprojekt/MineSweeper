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
        private Timer timer;
        private int countBombs;
        private int points;
        private bool won = false;
        #endregion  
        #region Init stuff
        public Game(Size fieldSize) {
            // init
            initForm();
            graphics = CreateGraphics();
            createLabels(new Point(30, 70), new Size(65, 65), fieldSize.Height, fieldSize.Width);
            countBombs = 
                (int)Math.Round((decimal)((labels.GetLength(0) * 0.5) * (labels.GetLength(1) * 0.5) * 0.65));
            setBombs(countBombs);
            Location = new Point(Location.X - (int)(Width * 0.5), Location.Y - (int)(Height * 0.5)); 
            checkBombs(); 
            Validate();
            timer.Start();
        } 
        private void initForm() { 
            ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Game));
            SuspendLayout(); 
            ClientSize = new Size(0, 0);
            StartPosition = FormStartPosition.CenterScreen; 
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "Game";
            Text = "Minesweeper";
            BackColor = Color.LightGray;
            Icon = global::MineSweeper.Properties.Resources.favicon;
            Paint += new PaintEventHandler(Redraw);
            Resize += new EventHandler(createNewGraphics);
            FormClosing += new FormClosingEventHandler(exit);

            timer = new Timer();
            timer.Interval = 200;
            timer.Tick += new EventHandler(Update);

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
        private void exit(object sender, FormClosingEventArgs e) { Environment.Exit(0); }
        #endregion
        #region Display stuff
        private void Redraw(object sender, PaintEventArgs e) { 
            graphics.FillRectangle(new SolidBrush(BackColor), new Rectangle(new Point(0, 0), Size));
            graphics.DrawString("Bombs:  " + (countBombs - 1).ToString(),
                new Font("Comic Sans MS", 16), new SolidBrush(Color.Black), new PointF(35f, 30f));
            graphics.DrawString("Points:  " + points,
                new Font("Comic Sans MS", 16), new SolidBrush(Color.Black), new PointF(Width - 150, 30f));
        } 
        private void createNewGraphics(object sender, EventArgs e) {
            graphics = CreateGraphics();
            StartPosition = FormStartPosition.CenterScreen; 
        }
        #endregion 
        #region Gamelogic  
        public void mouseClick(object sender, MouseEventArgs mouse) {
            MsLabel selectedLabel = (MsLabel)sender;
            selectedLabel.MouseClick -= mouseClick;

            // Set flag and reval the value
            if (mouse.Button.ToString().Equals("Right")) {
                selectedLabel.Flag = !selectedLabel.Flag; 
            } else if (selectedLabel.Bomb) { 
                for (int i = 0; i < labels.GetLength(0); i++) {
                    for (int j = 0; j < labels.GetLength(1); j++) {
                        labels[i, j].revealAll();
                        labels[i, j].MouseClick -= mouseClick;
                    }
                }
            }
            selectedLabel.selectLab();
            if (!selectedLabel.Flag)  
                points += (int)(selectedLabel.Value * 1.6);
            if (selectedLabel.Flag)   selectedLabel.MouseClick += mouseClick; 
            Size = new Size(Size.Width, Size.Height - 1); 
            Size = new Size(Size.Width, Size.Height + 1); 
        } 
        private void setBombs(int countBombs) { 
            int setBombs = 1;
            while (setBombs != countBombs) { 
                bool retry = true;
                Random random = 
                    new Random(Environment.TickCount % Convert.ToInt32(Environment.Version.ToString().Split('.')[0]));
                do {
                    int x = random.Next(0, labels.GetLength(0) - 1);
                    int y = random.Next(0, labels.GetLength(1) - 1);

                    if (!labels[x, y].Bomb) {
                        retry = false;
                        labels[x, y].Bomb = true;
                    }
                } while (retry);
                setBombs++;
            }
        }
        private void checkBombs() {
            foreach (MsLabel label in labels) {
                if (label.Bomb) continue;
                for (int i = -1; i <= 1; i++) {
                    for (int j = -1; j <= 1; j++) {
                        int x = label.X + i;
                        int y = label.Y + j;
                        if (x < 0 || x > labels.GetLength(0) - 1 || y < 0 || y > labels.GetLength(1) - 1) continue;
                        if (labels[x, y].Bomb) label.Value++;
                    }
                }
            }
        }
        private void Update(object sender, EventArgs mouse) {
            checkNext();
            if (!won) checkWin();
        }
        private void checkNext() {
            foreach (MsLabel label in labels) {
                if (label.Text == "" || label.Value != 0) continue;
                for (int i = -1; i <= 1 ; i++) {
                    for (int j = -1; j <= 1; j++) {
                        int x = label.X + i;
                        int y = label.Y + j;
                        if (x < 0 || x > labels.GetLength(0) -1 || y < 0 || y > labels.GetLength(1) -1 ||
                            labels[x, y].Flag || labels[x, y].Bomb) continue;
                        labels[x, y].Text = labels[x, y].Value.ToString();
                    }
                }
            }
        }
        private void checkWin() {
            foreach (MsLabel label in labels) { if (!label.Flag && label.Bomb) { return; } } 
            won = true;
            timer.Stop(); 
            MessageBox.Show("U win with " + points.ToString() + " points"); 
        }
        #endregion  
    }
}
