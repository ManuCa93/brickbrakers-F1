using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace BrickBreakerss
{
    internal class FileManager
    {
        static FileStream puntatore;
        public static void Salva(List<Utemte> dati, string path)
        {
            BinaryFormatter serializzatore = new BinaryFormatter();
            puntatore = new FileStream(path, FileMode.OpenOrCreate);

            serializzatore.Serialize(puntatore, dati);
            puntatore.Close();
        }

        public static List<Utemte> Leggi(string path)
        {
            try
            {
                List<Utemte> temp;
                BinaryFormatter serializzatore = new BinaryFormatter();

                if (File.Exists(path))
                {
                    puntatore = new FileStream(path, FileMode.Open);
                    temp = (List<Utemte>)serializzatore.Deserialize(puntatore);
                    puntatore.Close();
                }
                else temp = new List<Utemte>();

                return temp;
            }
            catch (Exception ec)
            {
                Console.WriteLine(ec.ToString());
                puntatore.Close();
                return new List<Utemte>();
            }
        }
    }
}
