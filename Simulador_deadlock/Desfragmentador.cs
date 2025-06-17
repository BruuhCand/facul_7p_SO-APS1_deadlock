using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Simulador_deadlock
{
    public class Desfragmentador
    {
        public void Iniciar()
        {
            int numThreads = Environment.ProcessorCount; // 100% de todos os núcleos
            Console.WriteLine($"Usando {numThreads} threads para simular desfragmentação...");

            for (int i = 0; i < numThreads; i++)
            {
                var thread = new Thread(() =>
                {
                    Random random = new Random();
                    //List<double> blocos = new List<double>();
                    double?[] blocos = new double?[100_000_000];
                    while (true)
                    {
                        double x = 0;
                        for (int j = 0; j < 100_000_000; j++)
                        {
                            x += Math.Sqrt(j) * Math.Sin(j) * Math.Log(j + 1);  // CPU-bound

                            int numeroAleatorio = random.Next(0, 50);
                            blocos[j] = numeroAleatorio;

                           

                           
                            //blocos.Add(x);
                            //blocos.Sort();

                        }

                        for(int j = 0; j < 50; j++)
                        {
                           
                            int numeroAleatorio = random.Next(0, 100_000);

                            blocos[numeroAleatorio] = null;
                        }

                        Console.WriteLine("\n\nARRAY COM NULOS\n\n");
                        for (int j = 0; j < 100; j++)
                        {
                            if (blocos[j].HasValue)
                                Console.Write($"{blocos[j].Value:F2} ");
                            else
                                Console.Write("null ");
                        }


                        //for (int j = 0; j < 100_000_000; j++)
                        //{
                        //    if (blocos[j] == null)
                        //    {
                        //        for(int k = 99_000_000; k > 0; k--)
                        //        {
                        //            if(blocos[k] != null)
                        //            {
                        //                blocos[j] = blocos[k];
                        //                blocos[k] = null;
                        //                break;
                        //            }
                        //        }
                        //    }

                        //}

                        int writeIndex = 0;
                        int readIndex = 0;

                        while (readIndex < blocos.Length)
                        {
                            if (blocos[readIndex].HasValue)
                            {
                                blocos[writeIndex] = blocos[readIndex];
                                if (writeIndex != readIndex)
                                    blocos[readIndex] = null;

                                writeIndex++;
                            }
                            readIndex++;
                        }

                        Console.WriteLine("\n\nARRAY DESFRAGMENTADO\n\n");
                        for (int j = 0; j < 100; j++)
                        {
                            if (blocos[j].HasValue)
                                Console.Write($"{blocos[j].Value:F2} ");
                            else
                                Console.Write("null ");
                        }




                    }
                });

                thread.IsBackground = true;
                thread.Start();
            }
        }
    }
}
