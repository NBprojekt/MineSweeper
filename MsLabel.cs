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
    public class MsLabel : Label {
        private Timer timer;
        private bool bomb;
        private bool flag;  
        private int value;
        private int x, y;

        public MsLabel(int x, int y) : base() { 
            AutoSize = false;
            Font = new Font("Comic Sans MS", 12.3F);
            ForeColor = Color.White;
            BorderStyle = BorderStyle.FixedSingle;
            TextAlign = ContentAlignment.MiddleCenter;
            BackColor = Color.DarkGray;
            Text = "";
            value = 0;

            timer = new Timer();
            timer.Interval = 2000;
            timer.Tick += new EventHandler(bgUpdate);

            this.x = x;
            this.y = y;

            bomb = flag = false;
        }
        #region Properties
        public int Value {
            set { this.value = value; }
            get { return value; }
        }
        public bool Bomb {
            set { 
                bomb = value;
            }
            get { return bomb; }
        }
        public bool Flag {
            set { 
                flag = value; 
            }
            get { return flag; }
        }
        public int X {
            get { return x; }
        }
        public int Y {
            get { return y; }
        }
        #endregion
        #region Methods
        public void selectLab() {
            if (flag)
                Image = global::MineSweeper.Properties.Resources.flag;
            else if (bomb) {
                Image = global::MineSweeper.Properties.Resources.explosion;
                timer.Start();
            } else {
                Text = value.ToString();  
                Image = null;
            }
        }
        public void revealAll() {
            if (bomb) 
                Image = global::MineSweeper.Properties.Resources.bomb; 
            else if (!flag)
                Text = value.ToString();
        }
        private void bgUpdate (object sender, EventArgs e) {
            Image = null;
            Text = "lost";
            ForeColor = Color.Red;
            timer.Stop(); 
        } 
        #endregion
    }
}
