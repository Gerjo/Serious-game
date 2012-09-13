using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Threading;

namespace Schilder
{
    public class FloodFill
    {
        private uint[] _data;
        private uint[] _colors;
        private Queue<Point> _todo;
        private uint _color;

        private int _imageWidth;
        private int _imageHeight;
        private Thread _thread;

        public DateTime StartTime { get; private set; }

        private bool _forceHalt;

        public FloodFill(uint[] data, uint[] colors, int imageWidth, int imageHeight, Point click, uint _color)
        {
            if (data.Length != colors.Length) throw new Exception("data[] length does not match colors[]. This means the given images are NOT of the same size.");
            this._color       = _color;
            this._colors      = colors;
            this._imageHeight = imageHeight;
            this._imageWidth  = imageWidth;
            this._data        = data;
            _forceHalt        = false;
            _todo             = new Queue<Point>(200);

            // Make sure we dont firstly click on a black pixel:
            if (_data[_imageWidth * click.Y + click.X] == 0xff000000) return;

            // Don't even bother if the target field already has the same color:
            if (_data[_imageWidth * click.Y + click.X] == _color) return;

            // Start point is where the mouseclicks.
            _todo.Enqueue(click);

            _thread         = new Thread(new ThreadStart(Start));
            _thread.Name    = "Floodfill main thread";
            _thread.Start();

            StartTime = DateTime.Now;
        }

        public bool IsRunning()
        {
            return (_thread != null && _thread.IsAlive);
        }

        private void Start()
        {
            // If we use recursion indead of this, a stack overflow is very prone.
            while (!_forceHalt && _todo.Count() > 0) Run();
        }

        public void Stop()
        {
            _forceHalt = true;
        }

        private Point IndexToPoint(int index)
        {
            return new Point(index % _imageWidth, (int)Math.Floor((float)index / _imageHeight));
        }

        private int PointToIndex(Point p)
        {
            return _imageWidth * p.Y + p.X;
        }

        private void Run()
        {
            Point current = _todo.Dequeue();
            
            Point[] around = new Point[8];
            around[0] = new Point(current.X +  0, current.Y +  1);
            around[1] = new Point(current.X +  1, current.Y +  0);
            around[2] = new Point(current.X + -1, current.Y +  0);
            around[3] = new Point(current.X +  0, current.Y + -1);

            around[4] = new Point(current.X +  1, current.Y +  1);
            around[5] = new Point(current.X +  1, current.Y + -1);
            around[6] = new Point(current.X + -1, current.Y + -1);
            around[7] = new Point(current.X + -1, current.Y +  1);

            for (int i = 0; i < around.Length; ++i)
            {
                if (around[i].X < _imageWidth && around[i].Y < _imageHeight && around[i].X >= 0 && around[i].Y >= 0)
                {
                    if (!_todo.Contains(around[i]))
                    {
                        if (_data[_imageWidth * around[i].Y + around[i].X] != 0xff000000 && _data[_imageWidth * around[i].Y + around[i].X] != _color) // 4294967295 = "white" or FFFFFFFF
                        {
                            _todo.Enqueue(around[i]);
                        }
                    }
                }
            }

            // Copy the data from the colored drawing into the "white/blank" drawing:
            _data[_imageWidth * current.Y + current.X] = _color;// _color;
        }
    }
}
