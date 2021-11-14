using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace Libreria_ED2
{ 
public class CifradorZigzag
{

    public void Cifrar(string dirLectura, string dirEscritura, int clave, string nombre)
    {
        string Data = System.IO.File.ReadAllText(dirLectura, Encoding.Default);
        string mensaje = Data;
        var lineas = new List<StringBuilder>();
        for (int i = 0; i < clave; i++)
        {
            lineas.Add(new StringBuilder());
        }
        int ActualL = 0;
        int Direccion = 1;
        //For para saber donde empezamos

        for (int i = 0; i < mensaje.Length; i++)
        {
            lineas[ActualL].Append(mensaje[i]);

            if (ActualL == 0)
                Direccion = 1;
            else if (ActualL == clave - 1)
                Direccion = -1;

            ActualL += Direccion;
        }
        StringBuilder CifradoFinal = new StringBuilder();

        //Saber donde se encuentra cada caracter
        for (int i = 0; i < clave; i++)
            CifradoFinal.Append(lineas[i].ToString());

        string Cifrados = CifradoFinal.ToString();

        File.WriteAllText(dirEscritura + nombre + ".zz", Cifrados);

    }

    public void Descifrar(string dirLectura, string dirEscritura, int clave, string nombre)
    {
        string Data = System.IO.File.ReadAllText(dirLectura, Encoding.Default);
        string mensaje = Data;
        var lineas = new List<StringBuilder>();
        int niveles = clave;


        for (int i = 0; i < clave; i++)
        {
            lineas.Add(new StringBuilder());
        }

        int[] LineaI = Enumerable.Repeat(0, clave).ToArray();

        int ActualL = 0;
        int Direccion = 1;

        //Donde inicia
        for (int i = 0; i < mensaje.Length; i++)
        {
            LineaI[ActualL]++;

            if (ActualL == 0)
                Direccion = 1;
            else if (ActualL == clave - 1)
                Direccion = -1;

            ActualL += Direccion;
        }

        int ActualPosicion = 0;

        for (int j = 0; j < clave; j++)
        {
            for (int c = 0; c < LineaI[j]; c++)
            {
                lineas[j].Append(mensaje[ActualPosicion]);
                ActualPosicion++;
            }
        }

        StringBuilder descifrado = new StringBuilder();

        ActualL = 0;
        Direccion = 1;

        int[] LP = Enumerable.Repeat(0, clave).ToArray();

        //Une el nuevo orden de las letras
        for (int i = 0; i < mensaje.Length; i++)
        {
            descifrado.Append(lineas[ActualL][LP[ActualL]]);
            LP[ActualL]++;

            if (ActualL == 0)
                Direccion = 1;
            else if (ActualL == clave - 1)
                Direccion = -1;

            ActualL += Direccion;
        }

        string DescifradoF = descifrado.ToString();

        File.WriteAllText(dirEscritura + nombre + ".txt", DescifradoF);

    }
}
}
