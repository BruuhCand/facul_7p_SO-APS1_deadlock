using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulador_deadlock
{
     class Processo
    {
        public string Nome { get; }
        public string[] RecursosNecessarios { get; }
        public string Estado { get; set; }
        public string? RecursoAtual { get; set; }
        public bool Finalizado { get; set; } = false;
        public Thread? Thread { get; set; }

        public Processo(string nome, string[] recursos)
        {
            Nome = nome;
            RecursosNecessarios = recursos;
        }
    }
}
