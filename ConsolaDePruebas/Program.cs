using System;

namespace ConsolaDePruebas
{
    class Program
    {
        static void Main(string[] args)
        {
            //Convertir a numero una cadena
            string ejemplo = "!#$%&/()=)asljfa12358";
            char[] conversion = ejemplo.ToCharArray();
            int total=0;
            for (int i = 0; i < conversion.Length; i++)
            {
                total += Convert.ToInt32(conversion[i]);
            }
            Console.WriteLine(total);

            total = 0;
            for (int i = 0; i < conversion.Length; i++)
            {
                total += Convert.ToInt32(conversion[i]);
            }
            int quitar = 0;
            while (total > 1024)
            {
                total = total - quitar;
                quitar++;
            }
            Console.WriteLine(total);
        }
    }
}
