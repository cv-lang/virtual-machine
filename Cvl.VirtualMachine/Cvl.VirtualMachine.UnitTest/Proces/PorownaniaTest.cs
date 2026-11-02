using System;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Proces
{
    /// <summary>
    /// Równość w kodzie interpretowanym. Release kompiluje `if (x == 1)` do bne.un albo beq, a Debug do ceq, więc te same testy
    /// trzeba uruchamiać w obu konfiguracjach. Bne porównywał pudełka przez referencję: każda liczba wychodziła „różna”.
    /// </summary>
    public class PorownaniaTest
    {
        public enum Kolor { Czerwony, Zielony, Niebieski }

        public class Pudelko
        {
            public Kolor Barwa { get; set; } = Kolor.Niebieski;
            public int Liczba { get; set; } = 7;
            public string Tekst { get; set; } = "x";
            public object Pusty => null;
        }

        public class Proces
        {
            public int LiczbaRowna()
            {
                var x = Dwa();
                if (x == 2) return 1;
                return 0;
            }

            public int LiczbaRozna()
            {
                var x = Dwa();
                if (x != 3) return 1;
                return 0;
            }

            public int LiczbaInnaNieWchodzi()
            {
                var x = Dwa();
                if (x == 3) return 0;
                return 1;
            }

            public int EnumRowny()
            {
                var p = new Pudelko();
                if (p.Barwa == Kolor.Niebieski) return 1;
                return 0;
            }

            public int EnumInny()
            {
                var p = new Pudelko();
                if (p.Barwa == Kolor.Zielony) return 0;
                if (p.Barwa != Kolor.Niebieski) return 0;
                return 1;
            }

            public int SwitchPoEnumie()
            {
                var p = new Pudelko();
                switch (p.Barwa)
                {
                    case Kolor.Czerwony: return 0;
                    case Kolor.Zielony: return 0;
                    case Kolor.Niebieski: return 1;
                    default: return 0;
                }
            }

            public int WlasciwoscLiczbowa()
            {
                var p = new Pudelko();
                if (p.Liczba == 7 && p.Liczba != 8) return 1;
                return 0;
            }

            public int ReferencjeTaSama()
            {
                var a = new Pudelko();
                object b = a;
                object c = new Pudelko();
                if (a == b && a != c) return 1;
                return 0;
            }

            public int Znak()
            {
                var t = "ab";
                if (t[0] == 'a' && t[1] != 'a') return 1;
                return 0;
            }

            public int Ulamek()
            {
                var d = Polowa();
                if (d == 0.5) return 1;
                return 0;
            }

            public int NaNNieJestRowne()
            {
                var d = double.NaN;
                if (d == d) return 0;
                return 1;
            }

            public int Tak()
            {
                var b = Prawda();
                if (b == true) return 1;
                return 0;
            }

            public int NullZObiektem()
            {
                var p = new Pudelko();
                var o = p.Pusty;
                if (o == null && p != null) return 1;
                return 0;
            }

            private static int Dwa() => 2;
            private static double Polowa() => 0.5;
            private static bool Prawda() => true;
        }

        [TestCase(nameof(Proces.LiczbaRowna))]
        [TestCase(nameof(Proces.LiczbaRozna))]
        [TestCase(nameof(Proces.LiczbaInnaNieWchodzi))]
        [TestCase(nameof(Proces.EnumRowny))]
        [TestCase(nameof(Proces.EnumInny))]
        [TestCase(nameof(Proces.SwitchPoEnumie))]
        [TestCase(nameof(Proces.WlasciwoscLiczbowa))]
        [TestCase(nameof(Proces.ReferencjeTaSama))]
        [TestCase(nameof(Proces.Znak))]
        [TestCase(nameof(Proces.Ulamek))]
        [TestCase(nameof(Proces.NaNNieJestRowne))]
        [TestCase(nameof(Proces.Tak))]
        [TestCase(nameof(Proces.NullZObiektem))]
        public void Porownanie_daje_to_samo_co_kod_natywny(string metoda)
        {
            var natywnie = (int)typeof(Proces).GetMethod(metoda).Invoke(new Proces(), null);
            Assert.AreEqual(1, natywnie);
            Assert.AreEqual(natywnie, new VirtualMachine().Start<int>(metoda, new Proces()).Result);
        }
    }
}
