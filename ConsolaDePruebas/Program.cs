using System;
using Libreria_ED2;
namespace ConsolaDePruebas
{
    class Program
    {
        static void Main(string[] args)
        {
            //Convertir a numero una cadena
            //string ejemplo = "!#$%&/()=)asljfa12358";
            //char[] conversion = ejemplo.ToCharArray();
            //int total=0;
            //for (int i = 0; i < conversion.Length; i++)
            //{
            //    total += Convert.ToInt32(conversion[i]);
            //}
            //Console.WriteLine(total);

            //total = 0;
            //for (int i = 0; i < conversion.Length; i++)
            //{
            //    total += Convert.ToInt32(conversion[i]);
            //}
            //int quitar = 0;
            //while (total > 1024)
            //{
            //    total = total - quitar;
            //    quitar++;
            //}
            //Console.WriteLine(total);
            CifradorSDES cifrador = new CifradorSDES(1024);
            CompresorLZW compresor = new CompresorLZW(1024);
            cifrador.Cifrar("C:\\ABF\\hard-test.txt", "C:\\ABF\\Cifrados\\", "C:\\ABF\\Permutaciones.txt", "cifradosdes", 555);
            compresor.Comprimir("C:\\ABF\\Cifrados\\cifradosdes.sdes", "C:\\ABF\\Cifrados\\", "Compresion");
            compresor.Descomprimir("C:\\ABF\\Cifrados\\Compresion.lzw", "C:\\ABF\\Decifrados\\");
            cifrador.Decifrar("C:\\ABF\\Decifrados\\cifradosdes.sdes", "C:\\ABF\\Decifrados\\", "C:\\ABF\\Permutaciones.txt", 555);

        }
    }
}
