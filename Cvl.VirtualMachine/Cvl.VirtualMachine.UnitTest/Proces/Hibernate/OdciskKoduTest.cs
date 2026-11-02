using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Cvl.VirtualMachine.Core.Serializers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Proces.Hibernate
{
    /// <summary>
    /// Prawdziwe rekompilacje - odcisk nie może reagować na zmiany tokenów metadanych,
    /// bo każde wdrożenie blokowałoby wtedy wszystkie zapisane procesy
    /// </summary>
    public class OdciskKoduTest
    {
        private const string Wzorzec = @"
            public class Inne { public static string Wartosc = ""w""; }
            public class Proces
            {
                public string Start()
                {
                    var x = Pomocnik();
                    try { return ""tekst"" + x + Inne.Wartosc; }
                    catch (System.InvalidOperationException) { return ""blad""; }
                }

                public string Pomocnik() { return ""a""; }
            }";

        private static Assembly Kompiluj(string zrodlo, OptimizationLevel optymalizacja = OptimizationLevel.Debug)
        {
            var referencje = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
                .Split(Path.PathSeparator)
                .Select(p => MetadataReference.CreateFromFile(p));
            var kompilacja = CSharpCompilation.Create("ProcesTestowy", new[] { CSharpSyntaxTree.ParseText(zrodlo) }, referencje,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: optymalizacja));

            using var strumien = new MemoryStream();
            var wynik = kompilacja.Emit(strumien);
            Assert.IsTrue(wynik.Success, string.Join("\n", wynik.Diagnostics));
            return Assembly.Load(strumien.ToArray());
        }

        private static MethodInfo Metoda(Assembly assembly, string nazwa) => assembly.GetType("Proces")!.GetMethod(nazwa)!;

        private static string Odcisk(string zrodlo, string nazwa = "Start") => CodeFingerprint.Get(Metoda(Kompiluj(zrodlo), nazwa));

        [Test]
        public void Ponowna_kompilacja_daje_ten_sam_odcisk()
        {
            Assert.AreEqual(Odcisk(Wzorzec), Odcisk(Wzorzec));
        }

        [Test]
        public void Zmiana_w_innych_klasach_i_metodach_nie_zmienia_odcisku()
        {
            var zInnymKodem = @"public class Nowa { public string Cos() { return ""nowy tekst""; } public string Cos2() { return Cos(); } }"
                + Wzorzec.Replace("public string Start()", @"public string Dodana() { return ""dodana""; } public string Start()");

            var przed = Metoda(Kompiluj(Wzorzec), "Start");
            var po = Metoda(Kompiluj(zInnymKodem), "Start");

            //bez tego test niczego by nie dowodził: surowy IL musi się różnić tokenami
            CollectionAssert.AreNotEqual(przed.GetMethodBody()!.GetILAsByteArray(), po.GetMethodBody()!.GetILAsByteArray());
            Assert.AreEqual(CodeFingerprint.Get(przed), CodeFingerprint.Get(po));
        }

        [Test]
        public void Zmiana_tekstu_w_metodzie_zmienia_odcisk()
        {
            Assert.AreNotEqual(Odcisk(Wzorzec), Odcisk(Wzorzec.Replace(@"""tekst""", @"""inny""")));
        }

        [Test]
        public void Zmiana_wywolanej_metody_nie_zmienia_odcisku_wywolujacej()
        {
            var zmieniony = Wzorzec.Replace(@"return ""a"";", @"return ""b"";");
            Assert.AreEqual(Odcisk(Wzorzec), Odcisk(zmieniony));
            Assert.AreNotEqual(Odcisk(Wzorzec, "Pomocnik"), Odcisk(zmieniony, "Pomocnik"));
        }

        [Test]
        public void Zmiana_typu_w_catch_zmienia_odcisk()
        {
            Assert.AreNotEqual(Odcisk(Wzorzec), Odcisk(Wzorzec.Replace("InvalidOperationException", "ArgumentException")));
        }

        [Test]
        public void Inna_konfiguracja_kompilacji_zmienia_odcisk()
        {
            Assert.AreNotEqual(
                CodeFingerprint.Get(Metoda(Kompiluj(Wzorzec, OptimizationLevel.Debug), "Start")),
                CodeFingerprint.Get(Metoda(Kompiluj(Wzorzec, OptimizationLevel.Release), "Start")));
        }
    }
}
