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

            var vm = new Cvl.VirtualMachine.VirtualMachine();
            var w1 = vm.Start<int>("Funkcja1", p, 1, 2, "3");

            Console.WriteLine($"w1: {w1}");

            var p2 = new ProcesTest();
            var wynik = p2.Funkcja1(1, 2, "3");
            Console.WriteLine($"wynik: {wynik}");

        }
    }
}
