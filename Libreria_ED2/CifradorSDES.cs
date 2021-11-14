using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace Libreria_ED2
{
    public class CifradorSDES:CifradorInterfaz
    {
        int longitudBuffer;
        string[] swbox1 = { "01", "00", "11", "10", "11", "10", "01", "00", "00", "10", "01", "11", "11", "01", "11", "10" };
        string[] swbox2 = { "00", "01", "10", "11", "10", "00", "01", "11", "11", "00", "01", "00", "10", "01", "00", "11" };
        int[] p10;
        int[] p8;
        int[] p4;
        int[] ep;
        int[] ip;
        int[] ipinv;

        public CifradorSDES(int _longitudBuffer)
        {
            longitudBuffer = _longitudBuffer;
        }
        public int ClaveChat(string cadena)
        {
            char[] conversion = cadena.ToCharArray();
            int total = 0;
            for (int i = 0; i < conversion.Length; i++)
            {
                total += Convert.ToInt32(conversion[i]);
            }
            int quitar = 0;
            while (total>1024)
            {
                total = total - quitar;
                quitar++;
            }
            return total;
        }
        public void Cifrar(string dirLectura, string dirEscritura, string dirPermutaciones, string nombre, int llave)
        {
            //Convirtiendo llave a bits
            string llaveBinaria = Convert.ToString(llave, 2);
            llaveBinaria=llaveBinaria.PadLeft(10, '0');
            char[] llavearreglo = llaveBinaria.ToCharArray();

            //Configurando permutaciones
            StreamReader sr = new StreamReader(new FileStream(dirPermutaciones, FileMode.OpenOrCreate));
            
            
            p10 = new int [10];
            p8 = new int[8];
            ip = new int[8];
            ipinv = new int[8];
            ep = new int[8];
            p4 = new int[4];

            string lineaLectura = sr.ReadLine();
            string[] div = lineaLectura.Split(',');
            for (int i = 0; i < p10.Length; i++)
            {
                p10[i] = Convert.ToInt32(div[i]) - 1;
            }
            
            lineaLectura = sr.ReadLine();
            div = lineaLectura.Split(',');
            for (int i = 0; i < p8.Length; i++)
            {
                p8[i] = Convert.ToInt32(div[i]) - 1;
            }

            lineaLectura = sr.ReadLine();
            div = lineaLectura.Split(',');
            for (int i = 0; i < p4.Length; i++)
            {
                p4[i] = Convert.ToInt32(div[i]) - 1;
            }

            lineaLectura = sr.ReadLine();
            div = lineaLectura.Split(',');
            for (int i = 0; i < ep.Length; i++)
            {
                ep[i] = Convert.ToInt32(div[i]) - 1;
            }
            lineaLectura = sr.ReadLine();
            div = lineaLectura.Split(',');
            for (int i = 0; i < ip.Length; i++)
            {
                ip[i] = Convert.ToInt32(div[i]) - 1;
            }
            lineaLectura = sr.ReadLine();
            div = lineaLectura.Split(',');
            for (int i = 0; i < ipinv.Length; i++)
            {
                ipinv[i] = Convert.ToInt32(div[i]) - 1;
            }

            sr.Close();

            char[] entp10 = llavearreglo;
            char[] salp10;
            salp10 = Permutar(entp10, p10);

            //generando k1
            char[] leftsh1 = LeftShiftUno(salp10);
            char[] k1 = Permutar(leftsh1, p8);
            string llave1="";
            for (int i = 0; i < k1.Length; i++)
            {
                llave1 += k1[i].ToString();
            }

            //generando k2
            char[] leftsh2 = LeftShiftDos(leftsh1);
            char[] k2 = Permutar(leftsh2, p8);

            string llave2 = "";
            for (int i = 0; i < k1.Length; i++)
            {
                llave2 += k2[i].ToString();
            }

            BinaryReader br;
            BinaryWriter bw = new BinaryWriter(new FileStream(dirEscritura + nombre + ".sdes", FileMode.Create));
            string nombreOriginal = Path.GetFileName(dirLectura);
            bw.Write(nombreOriginal);
            long posicionEscritura = bw.BaseStream.Position;
            bw.Close();
            
            long posicionLectura=0;
            byte[] bufferLectura = new byte[longitudBuffer];
            int cantidadLeida=0;
            string entrada;
            char[] arregloentrada;
            do
            {
                br = new BinaryReader(new FileStream(dirLectura, FileMode.OpenOrCreate));
                br.BaseStream.Position = posicionLectura;
                cantidadLeida = br.Read(bufferLectura);
                posicionLectura = br.BaseStream.Position;
                br.Close();
                byte[] bytesEscritura = new byte[cantidadLeida];

                for (int i = 0; i < cantidadLeida; i++)
                {
                    entrada = Convert.ToString(bufferLectura[i], 2);
                    entrada = entrada.PadLeft(8, '0');
                    arregloentrada = entrada.ToCharArray();
                    char[] ipS = Permutar(arregloentrada, ip);

                    string[] ronda1 = Ronda(ipS, llave1);
                    string swap = ronda1[1] + ronda1[0];
                  
                    char[] swapEntrada = swap.ToCharArray();

                    string[] ronda2 = Ronda(swapEntrada, llave2);
                    string salidaRonda2 = ronda2[0] + ronda2[1];
                    char[] salidaRonda2C = salidaRonda2.ToCharArray();
                    char[] SalidaFinal = Permutar(salidaRonda2C, ipinv);
                    string SalidaFinalString = "";

                    for (int j = 0; j < SalidaFinal.Length; j++)
                    {
                        SalidaFinalString += SalidaFinal[j];
                    }

                    byte byteFinal = Convert.ToByte(SalidaFinalString,2);

                    bytesEscritura[i] = byteFinal;

                }

                bw = new BinaryWriter(new FileStream(dirEscritura + nombre + ".sdes", FileMode.OpenOrCreate));
                bw.BaseStream.Position = posicionEscritura;
                bw.Write(bytesEscritura);
                posicionEscritura = bw.BaseStream.Position;
                bw.Close();


            } while (cantidadLeida==longitudBuffer);

        }



        public string[] Ronda(char[] entrada,string llave)
        {


            char[] ipS = entrada;
            char[] cuatroi = new char[4];
            char[] cuatrof = new char[4];
            string cuatrofcad = "";
            int k = 0;
            for (int j = 0; j < 8; j++)
            {
                if (j < 4)
                {
                    cuatroi[j] = ipS[j];
                }
                else
                {
                    cuatrof[k] = ipS[j];
                    cuatrofcad += ipS[j];
                    k++;
                }
            }

            //usar expandir y permutar con bits finales
            char[] epS = Permutar(cuatrof, ep);

            string epScadena = "";

            for (int j = 0; j < epS.Length; j++)
            {
                epScadena += epS[j];
            }
            byte llave1b = Convert.ToByte(llave, 2);
            byte epScadenab = Convert.ToByte(epScadena, 2);
            string resultadoXor = Convert.ToString(llave1b ^ epScadenab, 2);
            resultadoXor = resultadoXor.PadLeft(8, '0');
            char[] arregloxor = resultadoXor.ToCharArray();

            char[] resultadoswp = SwapBox(arregloxor);
            char[] sp4 = Permutar(resultadoswp, p4);

            string cadenaSp4 = "";
            string cadena4iniciales = "";

            for (int j = 0; j < sp4.Length; j++)
            {
                cadenaSp4 += sp4[j];
                cadena4iniciales += cuatroi[j];
            }



            byte byteSp4 = Convert.ToByte(cadenaSp4, 2);
            byte byte4i = Convert.ToByte(cadena4iniciales, 2);
            resultadoXor = Convert.ToString(byteSp4 ^ byte4i, 2);
            resultadoXor = resultadoXor.PadLeft(4, '0');
            string[] devolver = new string[2];
            devolver[0] = resultadoXor;
            devolver[1] = cuatrofcad;
            return devolver;

        }

        public char[] SwapBox(char[] entrada)
        {
            string f1 = Convert.ToString(entrada[0]) + Convert.ToString(entrada[3]);
            string c1 = Convert.ToString(entrada[1]) + Convert.ToString(entrada[2]);

            int fila1 = Convert.ToInt32(f1, 2);
            int columna1 = Convert.ToInt32(c1, 2);

            string f2 = Convert.ToString(entrada[4]) + Convert.ToString(entrada[7]);
            string c2 = Convert.ToString(entrada[5]) + Convert.ToString(entrada[6]);

            int fila2 = Convert.ToInt32(f2, 2);
            int columna2 = Convert.ToInt32(c2, 2);

            int posicion1 = (4 * fila1) + columna1;
            int posicion2 = (4 * fila2) + columna2;

            string resultado = swbox1[posicion1] + swbox2[posicion2];
            char[] devolver = resultado.ToCharArray();
            return devolver;

        }

        private char[] Permutar(char[] entrada, int[]permutacion)
        {
            char[] salida = new char[permutacion.Length];
            for (int i = 0; i < permutacion.Length; i++)
            {
                salida[i] = entrada[permutacion[i]];
            }
            return salida;
        }
        
        private char[] LeftShiftUno(char[] entrada)
        {
            int mitad = entrada.Length / 2;
            LinkedList<char> salida1 = new LinkedList<char>();
            LinkedList<char> salida2 = new LinkedList<char>();

            for (int i = 0; i < mitad; i++)
            {
                salida1.AddLast(entrada[i]); 
            }
           
            for (int i = mitad; i < entrada.Length; i++)
            {
                salida2.AddLast(entrada[i]);
            }

            char primerCaracter = salida1.First.Value;
            salida1.RemoveFirst();
            salida1.AddLast(primerCaracter);

            primerCaracter = salida2.First.Value;
            salida2.RemoveFirst();
            salida2.AddLast(primerCaracter);

            char[] union = new char[entrada.Length];
            int posicion = 0;

            foreach (var item in salida1)
            {
                union[posicion] = item;
                posicion++;
            }
            foreach (var item in salida2)
            {
                union[posicion] = item;
                posicion++;
            }

            return union;
        }

        private char[] LeftShiftDos(char[] entrada)
        {
            int mitad = entrada.Length / 2;
            LinkedList<char> salida1 = new LinkedList<char>();
            LinkedList<char> salida2 = new LinkedList<char>();

            for (int i = 0; i < mitad; i++)
            {
                salida1.AddLast(entrada[i]);
            }

            for (int i = mitad; i < entrada.Length; i++)
            {
                salida2.AddLast(entrada[i]);
            }

            char primerCaracter; 
            primerCaracter = salida1.First.Value;
            salida1.RemoveFirst();
            salida1.AddLast(primerCaracter);

            primerCaracter = salida1.First.Value;
            salida1.RemoveFirst();
            salida1.AddLast(primerCaracter);

            primerCaracter = salida2.First.Value;
            salida2.RemoveFirst();
            salida2.AddLast(primerCaracter);

            primerCaracter = salida2.First.Value;
            salida2.RemoveFirst();
            salida2.AddLast(primerCaracter);

            char[] union = new char[entrada.Length];
            int posicion = 0;

            foreach (var item in salida1)
            {
                union[posicion] = item;
                posicion++;
            }
            foreach (var item in salida2)
            {
                union[posicion] = item;
                posicion++;
            }

            return union;
        }

        public string Decifrar(string dirLectura, string dirEscritura, string dirPermutaciones, int llave)
        {
            //Convirtiendo llave a bits
            string llaveBinaria = Convert.ToString(llave, 2);
            llaveBinaria = llaveBinaria.PadLeft(10, '0');
            char[] llavearreglo = llaveBinaria.ToCharArray();

            //Configurando permutaciones
            StreamReader sr = new StreamReader(new FileStream(dirPermutaciones, FileMode.OpenOrCreate));
            //string permutacion;
            p10 = new int[10];
            p8 = new int[8];
            ip = new int[8];
            ipinv = new int[8];
            ep = new int[8];
            p4 = new int[4];

            string lineaLectura = sr.ReadLine();
            string[] div = lineaLectura.Split(',');
            for (int i = 0; i < p10.Length; i++)
            {
                p10[i] = Convert.ToInt32(div[i]) - 1;
            }

            lineaLectura = sr.ReadLine();
            div = lineaLectura.Split(',');
            for (int i = 0; i < p8.Length; i++)
            {
                p8[i] = Convert.ToInt32(div[i]) - 1;
            }

            lineaLectura = sr.ReadLine();
            div = lineaLectura.Split(',');
            for (int i = 0; i < p4.Length; i++)
            {
                p4[i] = Convert.ToInt32(div[i]) - 1;
            }

            lineaLectura = sr.ReadLine();
            div = lineaLectura.Split(',');
            for (int i = 0; i < ep.Length; i++)
            {
                ep[i] = Convert.ToInt32(div[i]) - 1;
            }
            lineaLectura = sr.ReadLine();
            div = lineaLectura.Split(',');
            for (int i = 0; i < ip.Length; i++)
            {
                ip[i] = Convert.ToInt32(div[i]) - 1;
            }
            lineaLectura = sr.ReadLine();
            div = lineaLectura.Split(',');
            for (int i = 0; i < ipinv.Length; i++)
            {
                ipinv[i] = Convert.ToInt32(div[i]) - 1;
            }

            sr.Close();
            
            //entp10 = entrada-permutacion10, salp10 = salida permutacion10
            char[] entp10 = llavearreglo;
            char[] salp10;
            salp10 = Permutar(entp10, p10);

            //generando k1
            char[] leftsh1 = LeftShiftUno(salp10);
            char[] k1 = Permutar(leftsh1, p8);
            string llave1 = "";
            for (int i = 0; i < k1.Length; i++)
            {
                llave1 += k1[i].ToString();
            }

            //generando k2
            char[] leftsh2 = LeftShiftDos(leftsh1);
            char[] k2 = Permutar(leftsh2, p8);

            string llave2 = "";
            for (int i = 0; i < k1.Length; i++)
            {
                llave2 += k2[i].ToString();
            }

            

            BinaryReader br = new BinaryReader(new FileStream(dirLectura, FileMode.OpenOrCreate));
            string nombreOriginal = br.ReadString();
            long posicionLectura = br.BaseStream.Position;
            br.Close();
            BinaryWriter bw = new BinaryWriter(new FileStream(dirEscritura + nombreOriginal, FileMode.Create));
            bw.Close();
            
            long posicionEscritura = 0;
            byte[] bufferLectura = new byte[longitudBuffer];
            int cantidadLeida = 0;
            string entrada;
            char[] arregloentrada;
            do
            {
                br = new BinaryReader(new FileStream(dirLectura, FileMode.OpenOrCreate));
                br.BaseStream.Position = posicionLectura;
                cantidadLeida = br.Read(bufferLectura);
                posicionLectura = br.BaseStream.Position;
                br.Close();
                byte[] bytesEscritura = new byte[cantidadLeida];

                for (int i = 0; i < cantidadLeida; i++)
                {
                    entrada = Convert.ToString(bufferLectura[i], 2);
                    entrada = entrada.PadLeft(8, '0');
                    arregloentrada = entrada.ToCharArray();
                    char[] ipS = Permutar(arregloentrada, ip);

                    string[] ronda1 = Ronda(ipS, llave2);
                    string swap = ronda1[1] + ronda1[0];
                    char[] swapEntrada = swap.ToCharArray();

                    string[] ronda2 = Ronda(swapEntrada, llave1);
                    string salidaRonda2 = ronda2[0] + ronda2[1];
                    char[] salidaRonda2C = salidaRonda2.ToCharArray();
                    char[] SalidaFinal = Permutar(salidaRonda2C, ipinv);
                    string SalidaFinalString = "";

                    for (int j = 0; j < SalidaFinal.Length; j++)
                    {
                        SalidaFinalString += SalidaFinal[j];
                    }

                    byte byteFinal = Convert.ToByte(SalidaFinalString, 2);

                    bytesEscritura[i] = byteFinal;

                }

                bw = new BinaryWriter(new FileStream(dirEscritura + nombreOriginal, FileMode.OpenOrCreate));
                bw.BaseStream.Position = posicionEscritura;
                bw.Write(bytesEscritura);
                posicionEscritura = bw.BaseStream.Position;
                bw.Close();


            } while (cantidadLeida == longitudBuffer);

            return nombreOriginal;
        }
    }
}
