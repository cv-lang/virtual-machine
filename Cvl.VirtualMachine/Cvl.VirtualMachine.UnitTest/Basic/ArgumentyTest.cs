using System.Globalization;
using Cvl.VirtualMachine.Core.Attributes;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Basic.Argumenty
{
    /// <summary>
    /// Indeks argumentu z operandu (ldarga.s, ldarg.s, starg.s): this liczy się raz, tylko w metodzie instancji.
    /// Wcześniej ldarga.s w metodzie instancji wskazywał o jeden dalej (wywołanie na parametrze typu wartościowego dostawało null),
    /// a ldarg.s/starg.s w metodzie statycznej o jeden dalej.
    /// </summary>
    public class ArgumentyTest
    {
        private static T Wykonaj<T>(string metoda, params object[] parametry)
        {
            var wszystkie = new object[parametry.Length + 1];
            wszystkie[0] = new ArgumentyProces();
            parametry.CopyTo(wszystkie, 1);
            return new VirtualMachine().StartTestExecution<T>(metoda, wszystkie);
        }

        [Test] public void Ldarga_w_metodzie_instancji() => Assert.AreEqual("Faktura 12300.5 zł", Wykonaj<string>("TekstKwoty", 12300.5m));
        [Test] public void Ldarga_w_metodzie_statycznej() => Assert.AreEqual("12300.5", Wykonaj<string>("TekstKwotyStatycznie", 12300.5m));
        [Test] public void Ldarg_s_w_metodzie_statycznej() => Assert.AreEqual(15, Wykonaj<int>("SumaStatycznie", 1, 2, 3, 4, 5));
        [Test] public void Ldarg_s_w_metodzie_instancji() => Assert.AreEqual(15, Wykonaj<int>("Suma", 1, 2, 3, 4, 5));
        [Test] public void Starg_s_w_metodzie_statycznej() => Assert.AreEqual(10, Wykonaj<int>("ZmienPiatyStatycznie", 1, 2, 3, 4, 5));
    }

    [Interpret]
    public class ArgumentyProces
    {
        public string TekstKwoty(decimal kwota) => "Faktura " + kwota.ToString(CultureInfo.InvariantCulture) + " zł";

        public string TekstKwotyStatycznie(decimal kwota) => Tekst(kwota);

        private static string Tekst(decimal kwota) => kwota.ToString(CultureInfo.InvariantCulture);

        public int SumaStatycznie(int a, int b, int c, int d, int e) => Suma5(a, b, c, d, e);

        private static int Suma5(int a, int b, int c, int d, int e) => a + b + c + d + e;

        public int Suma(int a, int b, int c, int d, int e) => a + b + c + d + e;

        public int ZmienPiatyStatycznie(int a, int b, int c, int d, int e) => ZmienPiaty(a, b, c, d, e);

        private static int ZmienPiaty(int a, int b, int c, int d, int e)
        {
            e = e * 2;
            return e;
        }
    }
}
