using System;
using System.Threading;

namespace Simulador_deadlock
{
    public class Desfragmentador
    {
        private static readonly object _consoleLock = new object();

        public void Iniciar()
        {
            int numThreads = Environment.ProcessorCount;
            Console.WriteLine($"Usando {numThreads} threads para simular desfragmentação...");

            for (int i = 0; i < numThreads; i++)
            {
                var thread = new Thread(() =>
                {
                    Random random = new Random();
                    int?[] blocos = new int?[100_000_000];

                    
                    for (int j = 0; j < 100_000_000; j++)
                    {
                        blocos[j] = random.Next(0, 1000);
                    }

                   
                    for (int j = 0; j < 100; j++)
                    {
                        blocos[random.Next(0, 250)] = null;
                    }

                 
                    ImprimirEstado("ARRAY COM NULOS", blocos);

                   
                    int writeIndex = 0;
                    for (int readIndex = 0; readIndex < blocos.Length; readIndex++)
                    {
                        if (blocos[readIndex].HasValue)
                        {
                            blocos[writeIndex] = blocos[readIndex];
                            if (writeIndex != readIndex)
                                blocos[readIndex] = null;
                            writeIndex++;
                        }
                    }

                    
                    ImprimirEstado("ARRAY DESFRAGMENTADO", blocos);

                   
                    int[] contagem = new int[100];
                    for (int j = 0; j < writeIndex; j++)
                    {
                        if (blocos[j].HasValue)
                            contagem[blocos[j].Value]++;
                    }

                    int index = 0;
                    for (int valor = 0; valor < contagem.Length; valor++)
                    {
                        for (int c = 0; c < contagem[valor]; c++)
                        {
                            blocos[index++] = valor;
                        }
                    }

                    for (int j = index; j < blocos.Length; j++)
                        blocos[j] = null;

                    
                    ImprimirEstado("ARRAY ORGANIZADO POR VALORES IGUAIS", blocos);

                });

                thread.IsBackground = true;
                thread.Start();
            }
        }

        private void ImprimirEstado(string titulo, int?[] blocos)
        {
            lock (_consoleLock)
            {
                Console.WriteLine($"\n\n{titulo}\n");
                for (int j = 0; j < 100; j++)
                {
                    Console.Write(blocos[j]?.ToString("D2") + " " ?? "null ");
                }
                Console.WriteLine();
            }
        }
    }
}
