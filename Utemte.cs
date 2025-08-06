using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrickBreakerss
{
    [Serializable]
    class Utemte : IComparable
    {
        string username;
        int punteggio;

        public Utemte()
        {
            punteggio = 0;

        }

        public string Username { get { return " "+username; } set { username = value; } }
        public int Punteggio { get { return punteggio; } set { punteggio = value; } }

        public override string ToString()
        {
            return "a";
        }

        public int CompareTo(object obj)
        {
            Utemte i = obj as Utemte;
            return i.punteggio.CompareTo(this.punteggio);
        }
    }
}
