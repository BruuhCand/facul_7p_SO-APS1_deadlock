using System;
using System.Collections.Generic;
using System.Linq;
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
                    while (true)
                    {
                        double x = 0;
                        for (int j = 0; j < 100_000_000; j++)
                        {
                            x += Math.Sqrt(j) * Math.Sin(j); // CPU-bound
                        }
                    }
                });

                thread.IsBackground = true; // garante que encerram com a aplicação
                thread.Start();
            }
        }
    }
}
