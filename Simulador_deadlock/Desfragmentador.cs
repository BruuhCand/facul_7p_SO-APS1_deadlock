using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulador_deadlock
{
    public class Desfragmentador
    {
        public void Iniciar(CancellationToken token)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Desfragmentação iniciada...");
            Console.ResetColor();

            while (!token.IsCancellationRequested)
            {
                for (int i = 0; i < 10_000_000; i++)
                {
                    double r = Math.Sqrt(i) * Math.Sin(i); // Simula CPU-bound
                }

                Console.WriteLine("Desfragmentando...");
                Thread.Sleep(1000); // Mantém fluidez no terminal
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Desfragmentação encerrada.");
            Console.ResetColor();
        }
    }
}
