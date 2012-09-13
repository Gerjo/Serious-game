using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Schilder
{
    public class DrawingContainer
    {
        public String Outline { get; set; }
        public String Thinline { get; set; }
        public String Colored { get; set; }
        public String CheckPoints { get; set; }

        public DrawingContainer(string name)
        {
            this.CheckPoints    = "Schilder/Drawings/" + name + "/" + name + "_checkpoints";
            this.Outline        = "Schilder/Drawings/" + name + "/" + name + "_outline";
            this.Thinline       = "Schilder/Drawings/" + name + "/" + name + "_thinline";
            this.Colored        = "Schilder/Drawings/" + name + "/" + name + "_colored";
        }
    }
}
