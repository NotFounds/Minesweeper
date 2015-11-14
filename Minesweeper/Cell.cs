using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Minesweeper
{
    class Cell
    {
        public static readonly int SIZE = 16;

        private Rectangle _rect;
        private Point _pos;
        private Status _state;
        private bool _haveBomb;
        private Image _img;
        private int _num;

        // コンストラクタ
        public Cell(Point pos, Status state, int num)
        {
            this._pos = pos;
            this._state = state;
            this._num = num;
            this._haveBomb = false;
            this._rect = new Rectangle(SIZE * _pos.X + 3, SIZE * _pos.Y + 3, SIZE, SIZE);

            setImage();
        }

        public void Draw(Graphics g)
        {
            // 画像を描画
            g.DrawImage(_img, _rect);
        }

        // セルの画像を設定
        private void setImage()
        {
            if (_state == Status.Close)
            {
                _img = Properties.Resources.BlockClose;
            }
            else if (_state == Status.Flag)
            {
                _img = Properties.Resources.BlockFlag;
            }
            else if (_state == Status.Burst)
            {
                _img = Properties.Resources.BlockBurst;
            }
            else
            {
                if (_haveBomb)
                {
                    _img = Properties.Resources.BlockBomb;
                    return;
                }

                switch (_num)
                {
                    case 0:
                        _img = Properties.Resources.Block0;
                        break;

                    case 1:
                        _img = Properties.Resources.Block1;
                        break;

                    case 2:
                        _img = Properties.Resources.Block2;
                        break;

                    case 3:
                        _img = Properties.Resources.Block3;
                        break;

                    case 4:
                        _img = Properties.Resources.Block4;
                        break;

                    case 5:
                        _img = Properties.Resources.Block5;
                        break;

                    case 6:
                        _img = Properties.Resources.Block6;
                        break;

                    case 7:
                        _img = Properties.Resources.Block7;
                        break;

                    case 8:
                        _img = Properties.Resources.Block8;
                        break;
                }
            }
        }

        /// <summary>セルが爆弾を持っているかを取得、設定します</summary>
        public bool Bomb
        {
            get
            {
                return _haveBomb;
            }
            set
            {
                _haveBomb = value;
            }
        }

        /// <summary>セルの状態を取得、設定します</summary>
        public Status State
        {
            get 
            {
                return _state;
            }
            set 
            {
                _state = value;
                setImage();
            }
        }

        /// <summary>セルの周りの爆弾の数を取得、設定します</summary>
        public int Num
        {
            get
            {
                return _num;
            }
            set
            {
                _num = value;
                setImage();
            }
        }

        /// <summary>セルの場所(相対座標)を取得、設定します</summary>
        public Point Location
        {
            get
            {
                return _pos;
            }
            set
            {
                _pos = value;
            }
        }
    }
}
