using System.Collections.Generic;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Basic.Isinst
{
    /// <summary>
    /// isinst zostawia na stosie obiekt (as, is T t), a nie samo "tak" - wcześniej "o as string" dawało True
    /// </summary>
    public class IsinstTest
    {
        private static T Wykonaj<T>(string metoda, object parametr) =>
            new VirtualMachine().StartTestExecution<T>(metoda, new IsinstProces(), parametr);

        [Test] public void As_daje_obiekt() => Assert.AreEqual("abc", Wykonaj<string>("AsTekst", "abc"));
        [Test] public void As_innego_typu_daje_null() => Assert.IsNull(Wykonaj<string>("AsTekst", 5));
        [Test] public void Is_ze_zmienna() => Assert.AreEqual("abc!", Wykonaj<string>("IsZeZmienna", "abc"));
        [Test] public void As_klasy_pozwala_czytac_pola() => Assert.AreEqual(3, Wykonaj<int>("AsKlasy", new List<int> { 1, 2, 3 }));
        [Test] public void Is_bez_zmiennej() => Assert.AreEqual(true, Wykonaj<bool>("IsTekst", "abc"));
        [Test] public void Is_struktury() => Assert.AreEqual(7, Wykonaj<int>("IsInt", 7));

        //po to był znacznik true: pudełko z zerem przez brtrue wyglądałoby jak fałsz
        [Test] public void Is_int_z_zerem() => Assert.AreEqual(1, Wykonaj<int>("CzyInt", 0));
        [Test] public void Is_bool_z_false() => Assert.AreEqual(1, Wykonaj<int>("CzyBool", false));
    }

    public class IsinstProces
    {
        public string AsTekst(object o) => o as string;

        public string IsZeZmienna(object o)
        {
            if (o is string t)
            {
                return t + "!";
            }
            return "nie";
        }

        public int AsKlasy(object o)
        {
            var lista = o as List<int>;
            return lista == null ? -1 : lista.Count;
        }

        public bool IsTekst(object o) => o is string;

        public int IsInt(object o)
        {
            if (o is int i)
            {
                return i;
            }
            return -1;
        }

        public int CzyInt(object o) => o is int ? 1 : 0;

        public int CzyBool(object o) => o is bool ? 1 : 0;
    }
}
