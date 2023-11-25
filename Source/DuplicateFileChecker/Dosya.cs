using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DuplicateFileChecker
{
    public class Dosya
    {
        public string Ad { get; set; }
        public string HashKod { get; set; }
        public string Konum { get; set; }
        public DateTime DegistirmeTarih { get; set; }
    }
}
