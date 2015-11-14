using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Minesweeper
{
    public partial class Form1 : Form
    {
        private Board _board;
        private int SecondCnt;
        public Form1()
        {
            InitializeComponent();
            _board = new Board();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void Init()
        {
            _board.Init();
            timer1.Enabled = false;
            pictureBox1.Enabled = true;
            pictureBox3.Image = Properties.Resources.Smile;
            SecondCnt = 0;
            pictureBox1.Invalidate();
            pictureBox2.Invalidate();
        }

        // 座標変換
        private Point pxcelToPos(Point pos)
        {
            Point ret;

            if (pos.X < 3 || pos.X >= Cell.SIZE * _board.CELL_NUM + 6)
            {
                ret = new Point(-1, -1);
            }

            if (pos.Y < 3 || pos.Y >= Cell.SIZE * _board.CELL_NUM + 6)
            {
                ret = new Point(-1, -1);
            }

            ret = new Point((pos.X - 3) / Cell.SIZE, (pos.Y - 3) / Cell.SIZE);

            return ret;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            _board.Draw(e.Graphics);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Point pos = pxcelToPos(e.Location);
           
            if (pos.X < 0 || pos.Y < 0)
            {
                return;
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (_board.getGameState() == false)
                {
                    timer1.Start();
                }

                _board.Open(pos);
                pictureBox1.Invalidate();
                pictureBox2.Invalidate();

                // ゲームクリア
                if (_board.Check())
                {
                    timer1.Stop();
                    pictureBox3.Image = Properties.Resources.GlassSmile;
                    MessageBox.Show(String.Format("ゲームクリア！\nクリア時間:{0}秒", SecondCnt));
                    pictureBox1.Enabled = false;
                    return;
                }

                // ゲームオーバー
                if (_board.getGameState() == false)
                {
                    timer1.Stop();
                    pictureBox3.Image = Properties.Resources.NotSmile;
                    MessageBox.Show("ゲームオーバー");
                    pictureBox1.Enabled = false;
                    return;
                }
            }
            else
            {
                _board.SetFlag(pos);
                pictureBox1.Invalidate();
                pictureBox2.Invalidate();
            }
        }

        private void スタートGToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Init();
        }

        private void pictureBox3_MouseDown(object sender, MouseEventArgs e)
        {
            Init();
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            int PictBox_Right = pictureBox2.Location.X + pictureBox2.Size.Width;

            // 外枠
            g.DrawLine(new Pen(Brushes.Gray, 4), new Point(0, 0), new Point(0, pictureBox2.Height));
            g.DrawLine(new Pen(Brushes.Gray, 4), new Point(0, 0), new Point(pictureBox2.Width, 0));
            g.DrawLine(new Pen(Brushes.White, 1), new Point(pictureBox2.Width-1, 1), new Point(pictureBox2.Width-1, pictureBox2.Height));
            g.DrawLine(new Pen(Brushes.White, 1), new Point(pictureBox2.Width-2, 2), new Point(pictureBox2.Width-2, pictureBox2.Height));
            g.DrawLine(new Pen(Brushes.White, 1), new Point(1, pictureBox2.Height-1), new Point(pictureBox2.Width, pictureBox2.Height-1));
            g.DrawLine(new Pen(Brushes.White, 1), new Point(2, pictureBox2.Height-2), new Point(pictureBox2.Width, pictureBox2.Height-2));

            // 内枠
            int NUM_X = 13;
            int NUM_Y = 26;

            // 左
            g.DrawLine(new Pen(Brushes.Gray,  2), new Point(12, 5), new Point(12, 5 + NUM_Y));
            g.DrawLine(new Pen(Brushes.Gray,  2), new Point(12, 6), new Point(14 + NUM_X * 3, 6));
            g.DrawLine(new Pen(Brushes.White, 2), new Point(14 + NUM_X * 3, 6), new Point(14 + NUM_X * 3, 5 + NUM_Y));
            g.DrawLine(new Pen(Brushes.White, 2), new Point(13, 5 + NUM_Y), new Point(14 + NUM_X * 3, 5 + NUM_Y));

            // 右
            g.DrawLine(new Pen(Brushes.Gray, 2), new Point(PictBox_Right - 26 - NUM_X * 3, 5), new Point(PictBox_Right - 26 - NUM_X * 3, 5 + NUM_Y));
            g.DrawLine(new Pen(Brushes.Gray, 2), new Point(PictBox_Right - 26 - NUM_X * 3, 6), new Point(PictBox_Right - 24, 6));
            g.DrawLine(new Pen(Brushes.White, 2), new Point(PictBox_Right - 24, 6), new Point(PictBox_Right - 24, 5 + NUM_Y));
            g.DrawLine(new Pen(Brushes.White, 2), new Point(PictBox_Right - 26 - NUM_X * 3, 5 + NUM_Y), new Point(PictBox_Right - 24, 5 + NUM_Y));


            // 左側パネル
            int num = _board.BOMB_NUM - _board.getFlagCnt();
            int num_1 = num / 100;
            num %= 100;
            int num_2 = num / 10;
            num %= 10;
            int num_3 = num;

            g.DrawImage(getNumImg(num_1), new Point(13, 7));
            g.DrawImage(getNumImg(num_2), new Point(13 + NUM_X, 7));
            g.DrawImage(getNumImg(num_3), new Point(13 + NUM_X * 2, 7));

            

            // 右側パネル
            int sec = SecondCnt;
            int sec_1, sec_2, sec_3;

            if (SecondCnt / 1000 > 0)
            {
                // 4枚目のパネル
                int sec_0 = sec / 1000;
                sec %= 1000;

                g.DrawImage(getNumImg(sec_0), new Point(PictBox_Right - 25 - NUM_X * 4, 7));

                // 枠追加
                g.DrawLine(new Pen(Brushes.Gray, 2), new Point(PictBox_Right - 26 - NUM_X * 4, 5), new Point(PictBox_Right - 26 - NUM_X * 4, 5 + NUM_Y));
                g.DrawLine(new Pen(Brushes.Gray, 2), new Point(PictBox_Right - 26 - NUM_X * 4, 6), new Point(PictBox_Right - 26 - NUM_X * 3, 6));
                g.DrawLine(new Pen(Brushes.White, 2), new Point(PictBox_Right - 26 - NUM_X * 4, 5 + NUM_Y), new Point(PictBox_Right - 26 - NUM_X * 3, 5 + NUM_Y));
            } 
            
            sec_1 = sec / 100;
            sec %= 100;
            sec_2 = sec / 10;
            sec %= 10;
            sec_3 = sec;

            g.DrawImage(getNumImg(sec_1), new Point(PictBox_Right - 25 - NUM_X * 3, 7));
            g.DrawImage(getNumImg(sec_2), new Point(PictBox_Right - 25 - NUM_X * 2, 7));
            g.DrawImage(getNumImg(sec_3), new Point(PictBox_Right - 25 - NUM_X, 7));

        }

        private Image getNumImg(int n)
        {
            Image img;

            switch (n)
            { 
                case 0:
                    img = Properties.Resources.Num0;
                    break;
                case 1:
                    img = Properties.Resources.Num1;
                    break;
                case 2:
                    img = Properties.Resources.Num2;
                    break;
                case 3:
                    img = Properties.Resources.Num3;
                    break;
                case 4:
                    img = Properties.Resources.Num4;
                    break;
                case 5:
                    img = Properties.Resources.Num5;
                    break;
                case 6:
                    img = Properties.Resources.Num6;
                    break;
                case 7:
                    img = Properties.Resources.Num7;
                    break;
                case 8:
                    img = Properties.Resources.Num8;
                    break;
                case 9:
                    img = Properties.Resources.Num9;
                    break;
                default:
                    img = Properties.Resources.Num0;
                    break;
            }

            return img;

        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            SecondCnt++;
            pictureBox2.Invalidate();
        }

#region "メニューバー"
      
        private void 終了EToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /*ヒント機能*/
        // ヘルプ*2 + スマイルを右クリック
        private void ヘルプHToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Minesweeper (Ver1.0)\nLast Update : 2015/01/28", "Minesweeper"); // 初版：中級（20*20、20）のみ
            //MessageBox.Show("Minesweeper (Ver1.1)\nLast Update : 2015/01/29", "Minesweeper"); // 初級・中級・上級を追加
            //MessageBox.Show("Minesweeper (Ver2.0)\nLast Update : 2015/01/29", "Minesweeper"); // 盤面サイズを可変（座標計算の改良）
            MessageBox.Show("Minesweeper (Ver2.1)\nLast Update : 2015/01/30", "Minesweeper"); // バグ修正（フラグ数が正しく表示されないバグ）
        }

        // 難易度
    #region "難易度"
        // 10 * 10 = 206,300
        private void toolStripMenuItem14_Click(object sender, EventArgs e)
        {
            // 10 * 10    : 10%
            _board.CELL_NUM = 10;
            _board.BOMB_NUM = 10;
            this.Size = new System.Drawing.Size(206, 300);
            Init();
            CharengeClear();
            ((ToolStripMenuItem)sender).Checked = true;
        }

        // 20 * 20 = 366,459
        private void 初級ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 20*20 初級 : 5%
            _board.CELL_NUM = 20;
            _board.BOMB_NUM = 20;
            this.Size = new System.Drawing.Size(366, 459);
            Init();
            CharengeClear();
            ((ToolStripMenuItem)sender).Checked = true;
        }

        private void toolStripMenuItem12_Click(object sender, EventArgs e)
        {
            // 20*20 中級 : 10%
            _board.CELL_NUM = 20;
            _board.BOMB_NUM = 40;
            this.Size = new System.Drawing.Size(366, 459);
            Init();
            CharengeClear();
            ((ToolStripMenuItem)sender).Checked = true;
        }

        private void 上級ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 20*20 上級 : 12.5%
            _board.CELL_NUM = 20;
            _board.BOMB_NUM = 50;
            this.Size = new System.Drawing.Size(366, 459);
            Init();
            CharengeClear();
            ((ToolStripMenuItem)sender).Checked = true;
        }

        // 25 * 25 = 446,539 
        private void toolStripMenuItem13_Click(object sender, EventArgs e)
        {
            // 25*25 初級 : 8%
            _board.CELL_NUM = 25;
            _board.BOMB_NUM = 50;
            this.Size = new System.Drawing.Size(446, 539);
            Init();
            CharengeClear();
            ((ToolStripMenuItem)sender).Checked = true;
        }

        private void 中級ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 25*25 中級 : 12.8%
            _board.CELL_NUM = 25;
            _board.BOMB_NUM = 80;
            this.Size = new System.Drawing.Size(446, 539);
            Init();
            CharengeClear();
            ((ToolStripMenuItem)sender).Checked = true;
        }

        private void 上級ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // 25*25 上級 : 15.84%
            _board.CELL_NUM = 25;
            _board.BOMB_NUM = 99;
            this.Size = new System.Drawing.Size(446, 539);
            Init();
            CharengeClear();
            ((ToolStripMenuItem)sender).Checked = true;
        }
        private void 超級ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 40*40 上級 : 25%
            _board.CELL_NUM = 40;
            _board.BOMB_NUM = 400;
            this.Size = new System.Drawing.Size(686, 780);
            Init();
            CharengeClear();
            ((ToolStripMenuItem)sender).Checked = true;
        }

        private void CharengeClear()
        {
            toolStripMenuItem14.Checked = false;
            初級ToolStripMenuItem.Checked = false;
            toolStripMenuItem12.Checked = false;
            上級ToolStripMenuItem.Checked = false;
            toolStripMenuItem13.Checked = false;
            中級ToolStripMenuItem.Checked = false;
            上級ToolStripMenuItem1.Checked = false;
            超級ToolStripMenuItem.Checked = false;
        }
    #endregion

        // 透明度
    #region "透明度"
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            CheckClear();
            toolStripMenuItem2.Checked = true;
            this.Opacity = 0.1;
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            CheckClear();
            toolStripMenuItem3.Checked = true;
            this.Opacity = 0.2;
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            CheckClear();
            toolStripMenuItem4.Checked = true;
            this.Opacity = 0.3;
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            CheckClear();
            toolStripMenuItem5.Checked = true;
            this.Opacity = 0.4;
        }

        private void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            CheckClear();
            toolStripMenuItem6.Checked = true;
            this.Opacity = 0.5;
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            CheckClear();
            toolStripMenuItem7.Checked = true;
            this.Opacity = 0.6;
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            CheckClear();
            toolStripMenuItem8.Checked = true;
            this.Opacity = 0.7;
        }

        private void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            CheckClear();
            toolStripMenuItem9.Checked = true;
            this.Opacity = 0.8;
        }

        private void toolStripMenuItem10_Click(object sender, EventArgs e)
        {
            CheckClear();
            toolStripMenuItem10.Checked = true;
            this.Opacity = 0.9;
        }

        private void toolStripMenuItem11_Click(object sender, EventArgs e)
        {
            CheckClear();
            toolStripMenuItem11.Checked = true;
            this.Opacity = 1.0;
        }

        private void CheckClear()
        {
            toolStripMenuItem2.Checked  = false;
            toolStripMenuItem3.Checked  = false;
            toolStripMenuItem4.Checked  = false;
            toolStripMenuItem5.Checked  = false;
            toolStripMenuItem6.Checked  = false;
            toolStripMenuItem7.Checked  = false;
            toolStripMenuItem8.Checked  = false;
            toolStripMenuItem9.Checked  = false;
            toolStripMenuItem10.Checked = false;
            toolStripMenuItem11.Checked = false;
        }
    #endregion
#endregion


        private void Form1_Resize(object sender, EventArgs e)
        {
            pictureBox1.Invalidate();
            pictureBox2.Invalidate();
            pictureBox3.Invalidate();

            //System.Diagnostics.Debug.WriteLine("size:({0},{1})", this.Size.Width, this.Size.Height);
            // 10 * 10 = 206,300
            // 20 * 20 = 366,459
            // 25 * 25 = 446,539
            // 30 * 30 = 526,619 
            // 40 * 40 = 686,780
        }
    }
}
