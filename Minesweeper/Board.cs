using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Minesweeper
{
    class Board
    {
        public int CELL_NUM = 20; // ボードの大きさ＝CELL_NUM*CELL_NUM
        public int BOMB_NUM = 40; // 地雷の数

        private Cell[,] _cells;
        private bool gameState; // ゲームの状態 (false = ゲームか始まっていない)

        int[] dx = { 0, 1, 1, 1, 0, -1, -1, -1 };
        int[] dy = { 1, 1, 0, -1, -1, -1, 0, 1 };

        private int flagCnt = 0; // 旗の数

        public Board()
        {
            Init();
        }
        
        // 初期化
        public void Init()
        {
            _cells = new Cell[CELL_NUM, CELL_NUM];

            for (int x = 0; x < CELL_NUM; x++)
            {
                for (int y = 0; y < CELL_NUM; y++)
                {
                    _cells[x, y] = new Cell(new Point(x, y), Status.Close, 0);
                }
            }
            flagCnt = 0;
            gameState = false;
        }

        // 描画
        public void Draw(Graphics g)
        {
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

            // それぞれのセルに描画命令
            foreach (Cell c in _cells)
            {   
                if (!gameState)
                {
                    if (c.Bomb && c.State != Status.Burst)
                    {
                        c.State = Status.Open;
                    }
                }
                c.Draw(g);
            }
                        
            // ボードを囲むラインの描画
            g.DrawLine(new Pen(Color.FromArgb(132, 130, 132),  5), new Point(0, 0), new Point(0, Cell.SIZE * CELL_NUM + 6));
            g.DrawLine(new Pen(Color.FromArgb(132, 130, 132),  5), new Point(0, 0), new Point(Cell.SIZE * CELL_NUM + 6, 0));
            g.DrawLine(new Pen(Brushes.White, 1), new Point(Cell.SIZE * CELL_NUM + 5, 1), new Point(Cell.SIZE * CELL_NUM + 5, Cell.SIZE * CELL_NUM + 4));
            g.DrawLine(new Pen(Brushes.White, 1), new Point(Cell.SIZE * CELL_NUM + 4, 2), new Point(Cell.SIZE * CELL_NUM + 4, Cell.SIZE * CELL_NUM + 4));
            g.DrawLine(new Pen(Brushes.White, 1), new Point(Cell.SIZE * CELL_NUM + 3, 3), new Point(Cell.SIZE * CELL_NUM + 3, Cell.SIZE * CELL_NUM + 4));
            g.DrawLine(new Pen(Brushes.White, 1), new Point(3, Cell.SIZE * CELL_NUM + 3), new Point(Cell.SIZE * CELL_NUM + 4, Cell.SIZE * CELL_NUM + 3));
            g.DrawLine(new Pen(Brushes.White, 1), new Point(2, Cell.SIZE * CELL_NUM + 4), new Point(Cell.SIZE * CELL_NUM + 4, Cell.SIZE * CELL_NUM + 4));
            g.DrawLine(new Pen(Brushes.White, 1), new Point(1, Cell.SIZE * CELL_NUM + 5), new Point(Cell.SIZE * CELL_NUM + 4, Cell.SIZE * CELL_NUM + 5));
        }

        // セルを開ける
        public void Open(Point pos)
        {
            if (gameState)
            {
                if (Cells(pos).State == Status.Flag)
                {
                    // フラグが立ってたら何もしない
                    return;
                }

                if (Cells(pos).Bomb)
                {
                    // ゲームオーバー
                    Cells(pos).State = Status.Burst;
                    gameState = false;
                }
                else
                {
                    Cells(pos).State = Status.Open;

                    // 自分が地雷を持っておらず、自分の周りに地雷がない場合セルを開ける（再帰）
                    if (Cells(pos).Num == 0)
                    { 
                        for (int i = 0; i < 8; i++)
                        {
                            int x = pos.X + dx[i];
                            int y = pos.Y + dy[i];

                            if (x < 0 || x >= CELL_NUM)
                            {
                                continue;
                            }

                            if (y < 0 || y >= CELL_NUM)
                            {
                                continue;
                            }

                            if (Cells(new Point(x, y)).Bomb == false && Cells(new Point(x, y)).State == Status.Close)
                            {
                                Open(new Point(x, y));
                            }
                        }
                    }

                }
            }
            else
            {
                setBombs(pos);
                gameState = true;
                Open(pos);
            }
        }

        // 旗を設置
        public void SetFlag(Point pos)
        {
            if (gameState)
            {
                switch (Cells(pos).State)
                { 
                    case Status.Close:
                        if (flagCnt < BOMB_NUM) // 旗が地雷の数より多くることはない
                        { 
                            Cells(pos).State = Status.Flag;
                            flagCnt++;
                        }
                        break;

                    case Status.Flag:
                        Cells(pos).State = Status.Close;
                        flagCnt--;
                        break;
                }
            }
        }

        // 地雷設置
        private void setBombs(Point pos)
        {
            Random r = new Random(DateTime.Now.Minute + DateTime.Now.Second);

            int x;
            int y;

            for (int i = 0; i < BOMB_NUM; i++)
            {
                while (true)
                {
                    x = r.Next(0, CELL_NUM);
                    y = r.Next(0, CELL_NUM);

                    if (Math.Abs(pos.X - x) < 2 && Math.Abs(pos.Y - y) < 2)
                    {
                        // 最初にクリックした場所の8近傍には爆弾は置かない
                        continue;
                    }
                    else
                    {
                        if (!Cells(new Point(x, y)).Bomb)
                        { 
                            // 爆弾を置く
                            Cells(new Point(x, y)).Bomb = true;
                            break;
                        }
                    }
                }
            }

            // セルに自分の8近傍の地雷数をセット
            for (x = 0; x < CELL_NUM; x++)
            {
                for (y = 0; y < CELL_NUM; y++)
                {
                    int cnt = 0;
                    for (int i = 0; i < 8; i++)
                    { 
                        // 数値をセット
                        int xx = x + dx[i];
                        int yy = y + dy[i];

                        if (xx < 0 || xx >= CELL_NUM)
                        {
                            continue;
                        }

                        if (yy < 0 || yy >= CELL_NUM)
                        {
                            continue;
                        }

                        if (Cells(new Point(xx, yy)).Bomb)
                        {
                            cnt++;
                        }
                    }
                    Cells(new Point(x, y)).Num = cnt;
                }
            }
        }

        public Cell Cells(Point pos)
        {
            return _cells[pos.X, pos.Y];
        }

        // ゲームの状態を返す
        public bool getGameState()
        {
            return gameState;
        }

        // 旗の数を返す
        public int getFlagCnt()
        {
            return flagCnt;
        }

        // ゲームの終了判定
        public bool Check()
        {
            bool ret = false;

            int cnt = 0;
            for (int x = 0; x < CELL_NUM; x++)
            { 
                for (int y = 0; y < CELL_NUM; y++)
                {
                    if ((Cells(new Point(x, y)).State == Status.Flag || Cells(new Point(x, y)).State == Status.Close) && Cells(new Point(x, y)).Bomb)
                    {
                        cnt++;
                    }
                    else if (Cells(new Point(x, y)).State == Status.Flag || Cells(new Point(x, y)).State == Status.Close)
                    {
                        cnt--;
                    }
                }
            }

            if (cnt == BOMB_NUM)
            {
                ret = true;
            }

            return ret;
        }
    }
}
