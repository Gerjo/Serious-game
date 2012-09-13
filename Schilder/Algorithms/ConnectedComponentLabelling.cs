using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Schilder
{
    public class ConnectedComponentLabelling
    {
        private Dictionary<short, List<int>> _labels;
        private List<String> _merge;
        private Pixel[,] _pixelData;
        private int _imageWidth;
        private int _imageHeight;
        private uint[] _refToCanvas;
        private short _labelCounter;
        private List<List<short?>> _union;
        private Thread _thread;

        public ConnectedComponentLabelling(uint[] refToCanvas, int imageWidth, int imageHeight)
        {
            _refToCanvas    = refToCanvas;
            _imageWidth     = imageWidth;
            _imageHeight    = imageHeight;
            _labels         = new Dictionary<short, List<int>>();
            _merge          = new List<String>();
            _labelCounter   = 0;
            _union          = new List<List<short?>>();

            LoadArray(refToCanvas);

            run();

            // Iterate over each relation mapping:
            foreach (String pair in _merge)
            {
                String[] chunks = pair.Split(',');
                short? a    = short.Parse(chunks[0]);
                short? b    = short.Parse(chunks[1]);
                bool added  = false;
                for (int i = 0; i < _union.Count(); ++i)
                {
                    // If this collection contains A, then lets add B to it.
                    if (_union[i].Contains(a))
                    {
                        added = true;
                        _union[i].Add(b);

                        // Search all collections for B, if B is found, merge it with the collection that holds A.
                        for (int j = 0; j < _union.Count(); ++j)
                        {
                            if (j != i && _union[j].Contains(b))
                            {
                                // Merge the ranges, then clear the old range for deletion lateron.
                                _union[i].AddRange(_union[j]);
                                _union[j].Clear();
                            }
                        }
                        break;
                    }
                }

                // If the label hasn't been added to any list, add it as a new list.
                if (!added)
                {
                    List<short?> tmp = new List<short?>();
                    tmp.Add(b);
                    tmp.Add(a);
                    _union.Add(tmp);
                }
            }

            // Cleanse the array from all empty lists.
            for (int i = 0; i < _union.Count(); ++i)
            {
                if (_union[i].Count() == 0)
                {
                    _union.RemoveAt(i);
                    if (i > 0) i--;
                }
            }

            Console.WriteLine("Thread started.");
            // Start the drawing thread. Using a thread gives a nice animation, and reduces the "hang" effect.
            _thread = new Thread(new ThreadStart(DrawLabels));
            _thread.Name = "ConnectedComponentLabelling main thread";
            _thread.Start();
        }

        private void DrawLabels()
        {
            // For each label group:
            for (short i = 0; i < _union.Count; ++i)
            {
                _labels.Add(i, new List<int>());

                //i = 2;
                for (int x = 0; x < _imageWidth; ++x)
                {
                    for (int y = 0; y < _imageHeight; ++y)
                    {
                        if (_union[i].Contains(_pixelData[x, y].TmpLabel))
                        {
                            _pixelData[x, y].Label = i;
                            _labels[i].Add(y * _imageWidth + x);
                        }
                    }
                }
               // break;
            }
        }

        // Loads the 1D array with pixels into a simple 2D array using Pixel containers.
        private void LoadArray(uint[] data)
        {
            _pixelData = new Pixel[_imageWidth, _imageHeight];

            for (int i = 0, height = 0, width = 0; i < data.Length; ++i, ++width)
            {
                if (width == _imageWidth)
                {
                    width = 0;
                    height++;
                }

                // Create a pixel container:
                _pixelData[width, height] = new Pixel { X = (short)width, Y = (short)height, Color = data[i], TmpLabel = null };
            }
        }

        private void run()
        {
            for (int x = 0; x < _imageWidth; ++x)
            {
                for (int y = 0; y < _imageHeight; ++y)
                {
                    // If the color isn't white, don't even bother going further.
                    if (_pixelData[x, y].Color != 4294967295) continue;

                    // Check for the west label, and clone if color match.
                    if (x != 0 && _pixelData[x - 1, y].Color == _pixelData[x, y].Color)
                    {
                        _pixelData[x, y].TmpLabel = _pixelData[x - 1, y].TmpLabel;
                    }

                    // There is no west, so check north for color match!
                    else if (y != 0 && _pixelData[x, y - 1].Color == _pixelData[x, y].Color)
                    {
                        _pixelData[x, y].TmpLabel = _pixelData[x, y - 1].TmpLabel;
                    }

                    // Oh no, west and north are the same color, but the label doesn't match!
                    if (x != 0 && y != 0)
                    {
                        if (_pixelData[x - 1, y].Color == _pixelData[x, y - 1].Color && _pixelData[x - 1, y].TmpLabel != _pixelData[x, y - 1].TmpLabel)
                        {
                            // Take note of this, so we can merge collections lateron.
                            _merge.Add(_pixelData[x - 1, y].TmpLabel + "," + _pixelData[x, y - 1].TmpLabel);
                        }
                    }

                    // No north and west match, thus create a new label!
                    if (_pixelData[x, y].TmpLabel == null)
                    {
                        _pixelData[x, y].TmpLabel = _labelCounter++;
                    }
                }
            }
        }

        public List<int> GetIndicesAround(int x, int y)
        {
            short label = _pixelData[x, y].Label;
            return _labels[label];
        }

        public bool IsRunning()
        {
            return (_thread != null && _thread.IsAlive);
        }
    }

     struct Pixel
    {
        public short X, Y;
        public short? TmpLabel;
        public short Label;
        public uint Color;
    }
}
