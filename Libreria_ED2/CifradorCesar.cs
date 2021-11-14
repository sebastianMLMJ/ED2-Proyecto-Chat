using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace Libreria_ED2
{
    public class CifradorCesar
    {
        int longitudBuffer;

        public CifradorCesar(int _longitudBuffer)
        {
            longitudBuffer = _longitudBuffer;
        }

        public void Cifrar(string dirLectura, string dirEscritura, string clave, string nombre)
        {

            char[] letrasClave = clave.ToCharArray();
            Dictionary<char, byte> eliminarRepetidosClave = new Dictionary<char, byte>();
            Dictionary<byte, byte> abcModificado = new Dictionary<byte, byte>();
            LinkedList<byte> abcModPrevio = new LinkedList<byte>();
            byte iterador = 0;
            int cantLeida = 0;

            for (int i = 0; i < letrasClave.Length; i++)
            {
                if (eliminarRepetidosClave.ContainsKey(letrasClave[i]) == false)
                {
                    eliminarRepetidosClave.Add(letrasClave[i], Convert.ToByte(letrasClave[i]));
                }
            }

            for (int i = 0; i < 256; i++)
            {
                abcModPrevio.AddLast(Convert.ToByte(i));
            }

            foreach (var item in eliminarRepetidosClave)
            {
                abcModPrevio.Remove(item.Value);
                abcModPrevio.AddFirst(item.Value);
            }

            foreach (var item in abcModPrevio)
            {
                abcModificado.Add(iterador, item);
                iterador++;
            }

            BinaryReader br = new BinaryReader(new FileStream(dirLectura, FileMode.OpenOrCreate));
            BinaryWriter bw = new BinaryWriter(new FileStream(dirEscritura+nombre+".csr", FileMode.Create));
            br.Close();
            bw.Close();
            long posLectura = 0;
            long posEscritura = 0;
            byte[] bytesLectura = new byte[longitudBuffer];
            byte[] bytesEscritura;


            do
            {

                br = new BinaryReader(new FileStream(dirLectura, FileMode.OpenOrCreate));
                br.BaseStream.Position = posLectura;
                cantLeida = br.Read(bytesLectura);
                posLectura = br.BaseStream.Position;
                br.Close();
                bytesEscritura = new byte[cantLeida];
                for (int i = 0; i < cantLeida; i++)
                {
                    bytesEscritura[i] = abcModificado[bytesLectura[i]];
                }

                bw = new BinaryWriter(new FileStream(dirEscritura + nombre + ".csr", FileMode.OpenOrCreate));
                bw.BaseStream.Position = posEscritura;
                bw.Write(bytesEscritura);
                posEscritura = bw.BaseStream.Position;
                bw.Close();

            } while (cantLeida == longitudBuffer);

        }

        public void Decifrar(string dirLectura, string dirEscritura, string clave, string nombre)
        {

            char[] letrasClave = clave.ToCharArray();
            Dictionary<char, byte> eliminarRepetidosClave = new Dictionary<char, byte>();
            Dictionary<byte, byte> abcModificado = new Dictionary<byte, byte>();
            LinkedList<byte> abcModPrevio = new LinkedList<byte>();
            byte iterador = 0;
            int cantLeida = 0;

            for (int i = 0; i < letrasClave.Length; i++)
            {
                if (eliminarRepetidosClave.ContainsKey(letrasClave[i]) == false)
                {
                    eliminarRepetidosClave.Add(letrasClave[i], Convert.ToByte(letrasClave[i]));
                }
            }

            for (int i = 0; i < 256; i++)
            {
                abcModPrevio.AddLast(Convert.ToByte(i));
            }

            foreach (var item in eliminarRepetidosClave)
            {
                abcModPrevio.Remove(item.Value);
                abcModPrevio.AddFirst(item.Value);
            }

            foreach (var item in abcModPrevio)
            {
                abcModificado.Add(item, iterador);
                iterador++;
            }

            BinaryReader br = new BinaryReader(new FileStream(dirLectura, FileMode.OpenOrCreate));
            BinaryWriter bw = new BinaryWriter(new FileStream(dirEscritura + nombre + ".txt", FileMode.Create));
            br.Close();
            bw.Close();
            long posLectura = 0;
            long posEscritura = 0;
            byte[] bytesLectura = new byte[longitudBuffer];
            byte[] bytesEscritura = new byte[longitudBuffer];


            do
            {
                br = new BinaryReader(new FileStream(dirLectura, FileMode.OpenOrCreate));
                br.BaseStream.Position = posLectura;
                cantLeida = br.Read(bytesLectura);
                posLectura = br.BaseStream.Position;
                br.Close();
                bytesEscritura = new byte[cantLeida];
                for (int i = 0; i < cantLeida; i++)
                {
                    bytesEscritura[i] = abcModificado[bytesLectura[i]];
                }

                bw = new BinaryWriter(new FileStream(dirEscritura + nombre + ".txt", FileMode.OpenOrCreate));
                bw.BaseStream.Position = posEscritura;
                bw.Write(bytesEscritura);
                posEscritura = bw.BaseStream.Position;
                bw.Close();

            } while (cantLeida == longitudBuffer);
        }
    }
}
