using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Uitnodiging
{
    class SpawnCards
    {
        List<int> Ids;
        public Piece[] piece;
        Random someNum = new Random();

        public void initSpawns() 
        { 
            Ids = new List<int>();
            for(int i=0; i<25; i++)
            {                
                Ids.Add(i);
            }
            spawnNext();
        }
        
        public void spawnNext()
        {
            if (Ids.Count == 0)
                return;

            int randomId = someNum.Next(Ids.Count);
            if (freshSpawn())
            {
                int number = Ids.ElementAt(randomId);
                piece[number].spawned = true;
                piece[number]._x = 800;
                piece[number]._y = 200;
                Ids.RemoveAt(randomId);
            }
        }

        public bool freshSpawn() 
        {
            for (int i = 0; i < 25; i++) 
            {
                if (piece[i]._x == 800 && piece[i]._y == 200) 
                {
                    return false;
                }
            }
            return true;
        }
    } 
}



