using Mono.Reflection;
using System;

namespace Cvl.VirtualMachine.TestApp
{
    public class ProcesTest
    {
        public string JakasPropercja { get; set; }
        private int zmienna = 4;

        public void Metoda1()
        {
            zmienna += 3;
        }

        public int Funkcja1(int p1, string p2, ProcesTest p3)
        {
            var zmienalokaln1 = p1;
            var zmiennaLokaln2 = p2 + p1.ToString();
            
            Metoda1();

            var zmienna3 = zmienalokaln1 + zmiennaLokaln2;

            return zmienna + p1 + p2.Length + zmienna3.Length;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var p = new ProcesTest();

            var metoda = p.GetType().GetMethod("Funkcja1");
            var instrukcje = metoda.GetInstructions();


            var vm = new VirtualMachine.WirtualnaMaszyna();
            vm.Start("Funkcja1", p);
        }
    }
}
