using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using SeriousGameLib;

namespace Uitnodiging
{
    class Grid
    {
        int multiplier;

        public bool checkGrid(MouseState mouseCurrent, Piece piece)
        {

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {

                    if (mouseCurrent.X > i * 120 + 50 && mouseCurrent.X < i * 120 + 170 && mouseCurrent.Y > j * 120 + 50 && mouseCurrent.Y < j * 120 + 170)
                    {
                        piece._x = i * 120 + 50;
                        piece._y = j * 120 + 50;

                        // Hide the narrator when a piece is on the last row (behind the textballoon)
                        if (j == 4) Narrator.Instance.Hide();

                        return true;
                    }
                }
            }
            return false;
        }

        public bool checkSolved(Piece Piece)
        {
            if (Piece._id % 5 == 0) multiplier = 1;
            if (Piece._id % 10 == 0) multiplier = 2;
            if (Piece._id % 15 == 0) multiplier = 3;
            if (Piece._id % 20 == 0) multiplier = 4;
            if (Piece._id == 0) multiplier = 0;

            if ((Piece._id % 5) * 120 + 50 == Piece._x && multiplier * 120 + 50 == Piece._y)
            {
                return true;
            }

            return false;
        }

    }
}
