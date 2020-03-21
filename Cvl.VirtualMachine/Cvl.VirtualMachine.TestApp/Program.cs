using Cvl.VirtualMachine.Test;
using Mono.Reflection;
using System;

namespace Cvl.VirtualMachine.TestApp
{    

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var p = new ProcesTest();

            var metoda = p.GetType().GetMethod("Funkcja1");
            var instrukcje = metoda.GetInstructions();

            //Mo

            var vm = new VirtualMachine.WirtualnaMaszyna();
            vm.Start("Funkcja1", p);
        }
    }
}
