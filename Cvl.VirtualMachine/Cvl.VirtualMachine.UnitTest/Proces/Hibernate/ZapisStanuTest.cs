using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Cvl.VirtualMachine.Core;
using Cvl.VirtualMachine.Core.Attributes;
using Cvl.VirtualMachine.Core.Serializers;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Proces.Hibernate
{
    /// <summary>
    /// Każde wznowienie idzie przez pełny zapis do JSON i odczyt - tak jak po restarcie serwera
    /// </summary>
    public class ZapisStanuTest
    {
        private static object Uruchom(object proces)
        {
            var vm = new VirtualMachine();
            var wynik = vm.Start<object>("Start", proces);
            int limit = 50;
            while (wynik.State == VirtualMachineState.Hibernated && limit-- > 0)
            {
                vm = VirtualMachine.DeserializeVirtualMachine(VirtualMachine.SerializeVirtualMachine(vm));
                wynik = vm.Resume<object>("wznowienie");
            }
            Assert.AreEqual(VirtualMachineState.Executed, wynik.State);
            return wynik.Result;
        }

        [Test] public void Prosty_przeplyw() => Assert.AreEqual(3, Uruchom(new PProsty()));
        [Test] public void Metoda_prywatna() => Assert.AreEqual(2, Uruchom(new PPrywatna()));
        [Test] public void Przeciazone_metody() => Assert.AreEqual(2, Uruchom(new PPrzeciazenia()));
        [Test] public void Foreach_po_liscie() => Assert.AreEqual("abc", Uruchom(new PForeach()));
        [Test] public void Zmienne_lokalne() => Assert.AreEqual("x|7|2|12,50|2030", Uruchom(new PLokalne()));
        [Test] public void Try_catch_przez_hibernacje() => Assert.AreEqual("zlapany", Uruchom(new PTryCatch()));
        [Test] public void Hibernacja_w_catch() => Assert.AreEqual("po catch: blad", Uruchom(new PHibernacjaWCatch()));
        [Test] public void Finally_po_wznowieniu() => Assert.AreEqual("try+finally", Uruchom(new PFinally()));
        [Test] public void Pole_prywatne() => Assert.AreEqual(5, Uruchom(new PPole()));
        [Test] public void Interpolacja() => Assert.AreEqual("wartosc 3 1,50 [  x]", Uruchom(new PInterpolacja()));
        [Test] public void Lambda_z_domknieciem() => Assert.AreEqual(6, Uruchom(new PLambda()));
        [Test] public void Linq_przez_hibernacje() => Assert.AreEqual(2, Uruchom(new PLinq()));
        [Test] public void Obiekt_wspoldzielony() => Assert.AreEqual("Jan/2/True", Uruchom(new PObiekt()));
        [Test] public void Slownik_i_zbior() => Assert.AreEqual("1:a|B:2|True|2", Uruchom(new PSlownik()));
        [Test] public void Foreach_po_slowniku() => Assert.AreEqual("a1b2", Uruchom(new PForeachSlownik()));
        [Test] public void Wyjatek_w_zmiennej() => Assert.AreEqual("boom/wew", Uruchom(new PWyjatekWZmiennej()));
        [Test] public void Wznowienie_przekazuje_dane() => Assert.AreEqual("wznowienie", Uruchom(new PDaneZZewnatrz()));

        [Test]
        public void Params_span_daje_czytelny_blad()
        {
            var blad = Assert.Throws<NotSupportedException>(() => new VirtualMachine().Start<object>("Start", new PParamsSpan()));
            StringAssert.Contains("new[]", blad!.Message);
        }

        [Test] public void Dlugie_laczenie_tekstow() => Assert.AreEqual("abcde", Uruchom(new PLaczenie()));

        //w Release "o != null" to brtrue, a pudełko z zerem wygląda na stosie jak liczba 0
        [Test, Explicit("Znane ograniczenie: Release, brtrue na object z pudełkiem 0")]
        public void Obiekt_z_zerem_nie_jest_nullem() => Assert.AreEqual(1, new VirtualMachine().Start<int>("Start", new PObiektZZerem(), (object)0).Result);

        [Test]
        public void Foreach_po_slowniku_po_usunieciu_jest_odrzucany()
        {
            var vm = new VirtualMachine();
            vm.Start<object>("Start", new PForeachPoUsunieciu());
            var blad = Assert.Throws<StateSerializationException>(() => VirtualMachine.SerializeVirtualMachine(vm));
            StringAssert.Contains("usunięto", blad!.Message);
        }

        [Test]
        public void Typ_z_zasobami_daje_czytelny_blad()
        {
            var vm = new VirtualMachine();
            vm.Start<object>("Start", new PStrumien());
            var blad = Assert.Throws<StateSerializationException>(() => VirtualMachine.SerializeVirtualMachine(vm));
            StringAssert.Contains("MemoryStream", blad!.Message);
            StringAssert.Contains("_strumien", blad.Message);
        }

        [Test]
        public void Pole_NonSerialized_odtwarza_hook()
        {
            Assert.AreEqual("odtworzony", Uruchom(new PNonSerialized()));
        }

        [Test]
        public void Metoda_async_w_interpreterze_daje_czytelny_blad()
        {
            var blad = Assert.Catch(() => new VirtualMachine().Start<object>("Start", new PAsync()));
            Assert.IsInstanceOf<NotSupportedException>(blad!.InnerException);
            StringAssert.Contains("async", blad.InnerException!.Message);
        }

        [Test]
        public void Stary_format_XML_jest_odrzucany()
        {
            var blad = Assert.Throws<StateSerializationException>(() => VirtualMachine.DeserializeVirtualMachine("<Complex name=\"Root\"/>"));
            StringAssert.Contains("format", blad!.Message);
        }

        [Test]
        public void Nieznana_wersja_formatu_jest_odrzucana()
        {
            var blad = Assert.Throws<StateSerializationException>(() => VirtualMachine.DeserializeVirtualMachine("{\"format\":999}"));
            StringAssert.Contains("999", blad!.Message);
        }

        [Test]
        public void Stan_nie_zalezy_od_kolejnosci_kluczy()
        {
            //jsonb w PostgreSQL przestawia klucze obiektów
            var vm = new VirtualMachine();
            var wynik = vm.Start<object>("Start", new PForeachSlownik());
            int limit = 50;
            while (wynik.State == VirtualMachineState.Hibernated && limit-- > 0)
            {
                var stan = OdwrocKlucze(JsonNode.Parse(VirtualMachine.SerializeVirtualMachine(vm)))!.ToJsonString();
                vm = VirtualMachine.DeserializeVirtualMachine(stan);
                wynik = vm.Resume<object>("wznowienie");
            }
            Assert.AreEqual("a1b2", wynik.Result);
        }

        private static JsonNode? OdwrocKlucze(JsonNode? node)
        {
            switch (node)
            {
                case JsonObject obiekt:
                    var odwrocony = new JsonObject();
                    foreach (var para in obiekt.Reverse().ToList())
                    {
                        odwrocony[para.Key] = OdwrocKlucze(para.Value);
                    }
                    return odwrocony;
                case JsonArray tablica:
                    return new JsonArray(tablica.Select(OdwrocKlucze).ToArray());
                default:
                    return node?.DeepClone();
            }
        }

        [Test]
        public void Stan_nie_zawiera_znakow_odrzucanych_przez_jsonb()
        {
            var stan = new FieldGraphSerializer().Serialize(new[] { "a\0b", "\ud800" });
            StringAssert.DoesNotContain("\\u0000", stan);
            StringAssert.DoesNotContain("\\ud800", stan.ToLowerInvariant());
        }

        [Test]
        public void Stan_jest_czytelny()
        {
            var stan = new FieldGraphSerializer().Serialize(new Klient { Imie = "Michał" });
            StringAssert.Contains("\"Imie\":\"Michał\"", stan);
        }

        [Test]
        public void Tworzenie_typu_systemowego_spoza_listy_jest_odrzucane()
        {
            var stan = "{\"format\":1,\"root\":{\"r\":1},\"objects\":[{\"id\":1,\"c\":\"System.IO.MemoryStream, System.Private.CoreLib\"}]}";
            Assert.Throws<StateSerializationException>(() => new FieldGraphSerializer().Deserialize<object>(stan));
        }

        [Test]
        public void Znaki_specjalne_w_tekstach_przezywaja_zapis()
        {
            foreach (var tekst in new[] { "", " spacje ", "a\nb\r\nc\td", "<&>\"'", "zażółć", "😀", "\0\u0001", "\ud800" })
            {
                var s = new FieldGraphSerializer();
                Assert.AreEqual(tekst, s.Deserialize<string>(s.Serialize(tekst)));
            }
        }

        [Test]
        public void Wartosci_proste_przezywaja_zapis()
        {
            var s = new FieldGraphSerializer();
            var data = new DateTime(2030, 1, 2, 3, 4, 5, DateTimeKind.Utc);
            var w = s.Deserialize<WszystkieTypy>(s.Serialize(new WszystkieTypy
            {
                Bajt = 200, Znak = 'ą', Dlugi = long.MinValue, Bez = ulong.MaxValue, Zmienny = double.NaN,
                Kwota = 79228162514264337593543950335m, Data = data, Czas = TimeSpan.FromHours(30), Id = Guid.Parse("11111111-2222-3333-4444-555555555555"),
                Stan = VirtualMachineState.Hibernated, Opcjonalny = 5, Pusty = null, Para = (1, "a"), Tablica = new[] { 1, 0, 3 }, Bajty = new byte[] { 1, 2 },
                Typ = typeof(List<string>), Adres = new Uri("https://agidot.eu/a?b=1"),
            }));

            Assert.AreEqual(200, w!.Bajt);
            Assert.AreEqual('ą', w.Znak);
            Assert.AreEqual(long.MinValue, w.Dlugi);
            Assert.AreEqual(ulong.MaxValue, w.Bez);
            Assert.IsNaN(w.Zmienny);
            Assert.AreEqual(79228162514264337593543950335m, w.Kwota);
            Assert.AreEqual(data, w.Data);
            Assert.AreEqual(DateTimeKind.Utc, w.Data.Kind);
            Assert.AreEqual(TimeSpan.FromHours(30), w.Czas);
            Assert.AreEqual(VirtualMachineState.Hibernated, w.Stan);
            Assert.AreEqual(5, w.Opcjonalny);
            Assert.IsNull(w.Pusty);
            Assert.AreEqual((1, "a"), w.Para);
            CollectionAssert.AreEqual(new[] { 1, 0, 3 }, w.Tablica);
            CollectionAssert.AreEqual(new byte[] { 1, 2 }, w.Bajty);
            Assert.AreEqual(typeof(List<string>), w.Typ);
            Assert.AreEqual("https://agidot.eu/a?b=1", w.Adres!.ToString());
        }

        [Test]
        public void Referencje_cykliczne_i_pudelka_zachowuja_tozsamosc()
        {
            var a = new Wezel { Nazwa = "a" };
            var b = new Wezel { Nazwa = "b", Nastepny = a };
            a.Nastepny = b;
            object pudelko = (1, 2);
            a.PudelkoA = pudelko;
            a.PudelkoB = pudelko;

            var s = new FieldGraphSerializer();
            var w = s.Deserialize<Wezel>(s.Serialize(a))!;

            Assert.AreSame(w, w.Nastepny!.Nastepny);
            Assert.AreEqual("b", w.Nastepny.Nazwa);
            Assert.AreSame(w.PudelkoA, w.PudelkoB);
        }

        [Test]
        public void Delegaty_przezywaja_zapis()
        {
            var licznik = new Licznik();
            Action zlozony = licznik.Dodaj;
            zlozony += licznik.Dodaj;
            Func<int, int> statyczny = Licznik.Podwoj;

            var s = new FieldGraphSerializer();
            var w = s.Deserialize<Delegaty>(s.Serialize(new Delegaty { Licznik = licznik, Zlozony = zlozony, Statyczny = statyczny }))!;

            w.Zlozony!();
            Assert.AreEqual(2, w.Licznik!.Stan);
            Assert.AreEqual(4, w.Statyczny!(2));
        }

        [Test]
        public void Slownik_ze_stringami_dziala_w_nowym_stanie_i_zachowuje_porownywacz()
        {
            var s = new FieldGraphSerializer();
            var slownik = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { ["Klucz"] = 1 };
            var w = s.Deserialize<Dictionary<string, int>>(s.Serialize(slownik))!;
            Assert.AreEqual(1, w["KLUCZ"]);
        }

        [Test]
        public void Zmiana_kodu_przed_wznowieniem_jest_wykrywana()
        {
            var vm = new VirtualMachine();
            vm.Start<object>("Start", new PWersjaA());
            var stan = VirtualMachine.SerializeVirtualMachine(vm).Replace("PWersjaA", "PWersjaB");
            var blad = Assert.Throws<ProcessCodeChangedException>(() => VirtualMachine.DeserializeVirtualMachine(stan));
            StringAssert.Contains("PWersjaB.Start", blad!.Message);
            Assert.AreNotEqual(blad.SavedFingerprint, blad.CurrentFingerprint);
        }

        [Test]
        public void Inny_odcisk_ramki_stosu_blokuje_odczyt()
        {
            var vm = new VirtualMachine();
            vm.Start<object>("Start", new PPrywatna());
            var stan = VirtualMachine.SerializeVirtualMachine(vm);
            var zmieniony = System.Text.RegularExpressions.Regex.Replace(stan, "\"OdciskKodu\":\"[0-9A-F]+\"", "\"OdciskKodu\":\"00\"");
            Assert.AreNotEqual(stan, zmieniony);
            Assert.Throws<ProcessCodeChangedException>(() => VirtualMachine.DeserializeVirtualMachine(zmieniony));
        }

        [Test]
        public void Metoda_spoza_stosu_nie_wchodzi_do_stanu()
        {
            //pomocnik zakończył się przed hibernacją, więc można go zmieniać bez blokowania procesu
            var vm = new VirtualMachine();
            vm.Start<object>("Start", new PPomocnikPrzedHibernacja());
            StringAssert.DoesNotContain("Pomocnik", VirtualMachine.SerializeVirtualMachine(vm).Replace("PPomocnikPrzedHibernacja", ""));
        }

        [Test]
        public void Zmieniony_typ_pola_daje_czytelny_blad()
        {
            var vm = new VirtualMachine();
            vm.Start<object>("Start", new PPole());
            var stan = VirtualMachine.SerializeVirtualMachine(vm).Replace("\"_licznik\":5", "\"_licznik\":\"piec\"");
            var blad = Assert.Throws<StateSerializationException>(() => VirtualMachine.DeserializeVirtualMachine(stan));
            StringAssert.Contains("_licznik", blad!.Message);
        }

        [Test] public void Catch_innego_typu_nie_lapie() => Assert.AreEqual("zewnetrzny", Uruchom(new PCatchInnegoTypu()));
        [Test] public void Drugi_catch_lapie_swoj_typ() => Assert.AreEqual("drugi", Uruchom(new PDwaCatche()));
        [Test] public void Wyjatek_z_metody_zewnetrznej_i_catch_innego_typu() => Assert.AreEqual("zewnetrzny", Uruchom(new PCatchZMetodyZewnetrznej()));
        [Test] public void Catch_typu_bazowego_lapie() => Assert.AreEqual("baza", Uruchom(new PCatchBazowy()));
        [Test] public void Po_wewnetrznym_try_zewnetrzny_catch_nadal_dziala() => Assert.AreEqual("zewnetrzny", Uruchom(new PZagniezdzonyTry()));
        [Test] public void Wyjatek_rzucony_w_catch_idzie_do_zewnetrznego() => Assert.AreEqual("wew:nowy", Uruchom(new PThrowWCatch()));
        [Test] public void Rethrow_po_obsludze_innego_wyjatku() => Assert.AreEqual("pierwszy", Uruchom(new PRethrow()));
        [Test] public void Kolejnosc_finally_przy_wyjsciu_normalnym() => Assert.AreEqual("wew,zew,po", Uruchom(new PDwaFinally()));
        [Test] public void Finally_przy_odwijaniu_do_metody_wywolujacej() => Assert.AreEqual("f1,f2,catch:x", Uruchom(new PFinallyOdwijanie()));
        [Test] public void Hibernacja_w_finally_przy_odwijaniu() => Assert.AreEqual("finally,catch:x", Uruchom(new PHibernacjaWFinally()));
        [Test] public void Try_w_petli() => Assert.AreEqual("0b1b2b", Uruchom(new PTryWPetli()));
        [Test] public void Catch_czysci_stos_ewaluacji() => Assert.AreEqual(5, Uruchom(new PWyjatekWWyrazeniu()));
        [Test] public void Catch_przez_metode_z_pierwsza_instrukcja_w_try() => Assert.AreEqual("zlapany", Uruchom(new PTryOdPierwszejInstrukcji()));
        [Test] public void Stale_jednobajtowe_to_int() => Assert.AreEqual("x|-3|100", Uruchom(new PStaleJednobajtowe()));
        [Test] public void Json_przez_hibernacje() => Assert.AreEqual("ok|1|a,b|7", Uruchom(new PJson()));

        [Test]
        public void Finally_wykonuje_sie_gdy_wyjatek_wychodzi_poza_maszyne()
        {
            var proces = new PWyjatekNaZewnatrz();
            var blad = Assert.Throws<ArgumentException>(() => new VirtualMachine().Start<object>("Start", proces));
            Assert.AreEqual("koniec", blad!.Message);
            Assert.AreEqual("f1,f2", proces.Slad);
        }

        [Test]
        public void Catch_when_daje_czytelny_blad()
        {
            var blad = Assert.Throws<NotSupportedException>(() => new VirtualMachine().Start<object>("Start", new PCatchWhen()));
            StringAssert.Contains("when", blad!.Message);
        }

        [Test]
        public void Odpowiedz_json_z_Resume_przezywa_kolejna_hibernacje()
        {
            var vm = new VirtualMachine();
            vm.Start<object>("Start", new PJsonZResume());
            vm = VirtualMachine.DeserializeVirtualMachine(VirtualMachine.SerializeVirtualMachine(vm));
            var wynik = vm.Resume<object>(new JsonObject { ["_przycisk"] = "zapisz", ["ilosc"] = 3 });
            Assert.AreEqual(VirtualMachineState.Hibernated, wynik.State);
            vm = VirtualMachine.DeserializeVirtualMachine(VirtualMachine.SerializeVirtualMachine(vm));
            Assert.AreEqual("zapisz/3", vm.Resume<object>().Result);
        }

        [Test]
        public void Uslugi_trafiaja_do_procesu_po_odczycie_przed_hookiem()
        {
            var vm = new VirtualMachine();
            vm.Start<object>("Start", new PZUsluga { Usluga = new UslugaTestowa("przed") });
            var stan = VirtualMachine.SerializeVirtualMachine(vm);
            StringAssert.DoesNotContain("UslugaTestowa", stan.Replace("PZUsluga", ""));

            vm = VirtualMachine.DeserializeVirtualMachine(stan, new DostawcaUslug(new UslugaTestowa("po")));
            Assert.AreEqual("po:hook", vm.Resume<object>().Result);
        }

        [Test]
        public void Brak_uslug_przy_odczycie_daje_czytelny_blad()
        {
            var vm = new VirtualMachine();
            vm.Start<object>("Start", new PZUsluga { Usluga = new UslugaTestowa("przed") });
            var stan = VirtualMachine.SerializeVirtualMachine(vm);
            var blad = Assert.Throws<StateSerializationException>(() => VirtualMachine.DeserializeVirtualMachine(stan));
            StringAssert.Contains("IServiceProvider", blad!.Message);
        }
    }

    public class WszystkieTypy
    {
        public byte Bajt;
        public char Znak;
        public long Dlugi;
        public ulong Bez;
        public double Zmienny;
        public decimal Kwota;
        public DateTime Data;
        public TimeSpan Czas;
        public Guid Id;
        public VirtualMachineState Stan;
        public int? Opcjonalny;
        public int? Pusty;
        public (int, string) Para;
        public int[]? Tablica;
        public byte[]? Bajty;
        public Type? Typ;
        public Uri? Adres;
    }

    public class Wezel
    {
        public string? Nazwa;
        public Wezel? Nastepny;
        public object? PudelkoA;
        public object? PudelkoB;
    }

    public class Licznik
    {
        public int Stan;
        public void Dodaj() => Stan++;
        public static int Podwoj(int x) => x * 2;
    }

    public class Delegaty
    {
        public Licznik? Licznik;
        public Action? Zlozony;
        public Func<int, int>? Statyczny;
    }

    public class PProsty
    {
        public int State { get; set; }
        public object Start()
        {
            State++;
            VirtualMachine.Hibernate("a");
            State++;
            VirtualMachine.Hibernate("b");
            State++;
            return State;
        }
    }

    public class PPrywatna
    {
        public int State { get; set; }
        public object Start()
        {
            Krok();
            Krok();
            return State;
        }

        [Interpret]
        private void Krok()
        {
            State++;
            VirtualMachine.Hibernate("a");
        }
    }

    public class PPrzeciazenia
    {
        public int State { get; set; }
        public object Start()
        {
            Krok();
            Krok(1);
            return State;
        }

        [Interpret]
        public void Krok()
        {
            State++;
            VirtualMachine.Hibernate("a");
        }

        [Interpret]
        public void Krok(int ile)
        {
            State += ile;
            VirtualMachine.Hibernate("b");
        }
    }

    public class PForeach
    {
        public object Start()
        {
            var lista = new List<string> { "a", "b", "c" };
            var wynik = "";
            foreach (var e in lista)
            {
                VirtualMachine.Hibernate(e);
                wynik += e;
            }
            return wynik;
        }
    }

    public class PLokalne
    {
        public object Start()
        {
            string s = "x";
            int i = 7;
            var lista = new List<int> { 1, 2 };
            decimal kwota = 12.5m;
            DateTime data = new DateTime(2030, 1, 1);
            VirtualMachine.Hibernate("a");
            return string.Join("|", new[] { s, i.ToString(), lista.Count.ToString(), kwota.ToString("0.00", new System.Globalization.CultureInfo("pl-PL")), data.Year.ToString() });
        }
    }

    public class PObiektZZerem
    {
        public int Start(object wartosc)
        {
            if (wartosc != null)
                return 1;
            return 0;
        }
    }

    public class PLaczenie
    {
        public object Start()
        {
            string a = "a", b = "b", c = "c", d = "d", e = "e";
            VirtualMachine.Hibernate("x");
            return a + b + c + d + e;
        }
    }

    public class PPomocnikPrzedHibernacja
    {
        public object Start()
        {
            var x = Pomocnik();
            VirtualMachine.Hibernate("a");
            return x;
        }

        [Interpret]
        public int Pomocnik()
        {
            return 1;
        }
    }

    public class PCatchInnegoTypu
    {
        public object Start()
        {
            try
            {
                try
                {
                    throw new ArgumentException("x");
                }
                catch (InvalidOperationException)
                {
                    return "wewnetrzny";
                }
            }
            catch (ArgumentException)
            {
                return "zewnetrzny";
            }
        }
    }

    public class PDwaCatche
    {
        public object Start()
        {
            try
            {
                throw new ArgumentException("x");
            }
            catch (InvalidOperationException)
            {
                return "pierwszy";
            }
            catch (ArgumentException)
            {
                return "drugi";
            }
        }
    }

    public class PCatchZMetodyZewnetrznej
    {
        public object Start()
        {
            try
            {
                try
                {
                    int.Parse("x");
                }
                catch (InvalidOperationException)
                {
                    return "wewnetrzny";
                }
            }
            catch (FormatException)
            {
                return "zewnetrzny";
            }
            return "nic";
        }
    }

    public class UslugaTestowa
    {
        public string Nazwa { get; }
        public UslugaTestowa(string nazwa) { Nazwa = nazwa; }
    }

    public class DostawcaUslug : IServiceProvider
    {
        private readonly object usluga;
        public DostawcaUslug(object usluga) { this.usluga = usluga; }
        public object? GetService(Type serviceType) => serviceType.IsInstanceOfType(usluga) ? usluga : null;
    }

    public class PZUsluga : IUsesServices, IDeserializedObject
    {
        [NonSerialized] private UslugaTestowa? _usluga;
        [NonSerialized] private string _hook = "";

        public UslugaTestowa? Usluga { set { _usluga = value; } }

        public void AttachServices(IServiceProvider services)
        {
            _usluga = (UslugaTestowa)services.GetService(typeof(UslugaTestowa))!;
        }

        public void AfterDeserialization()
        {
            //usługa musi już być
            _hook = _usluga!.Nazwa + ":hook";
        }

        public object Start()
        {
            VirtualMachine.Hibernate("a");
            return _hook;
        }
    }

    public class PJson
    {
        public JsonObject Pole { get; set; } = new JsonObject { ["stan"] = "ok" };

        public object Start()
        {
            var obiekt = new JsonObject { ["ilosc"] = 1 };
            var tablica = new JsonArray { "a", "b" };
            JsonNode wartosc = JsonValue.Create(7);
            VirtualMachine.Hibernate("x");
            return (string)Pole["stan"]! + "|" + (int)obiekt["ilosc"]! + "|" + (string)tablica[0]! + "," + (string)tablica[1]! + "|" + (int)wartosc;
        }
    }

    public class PStaleJednobajtowe
    {
        public object Start()
        {
            var tablica = new string[20];
            tablica[15] = "x";
            int ujemna = -3;
            object pudelko = 100;
            VirtualMachine.Hibernate("a");
            return tablica[15] + "|" + ujemna + "|" + (int)pudelko;
        }
    }

    public class PJsonZResume
    {
        public object Start()
        {
            var odp = (JsonObject)VirtualMachine.Hibernate("pytanie")!;
            VirtualMachine.Hibernate("drugie");
            return (string)odp["_przycisk"]! + "/" + (int)odp["ilosc"]!;
        }
    }

    public class PCatchBazowy
    {
        public object Start()
        {
            try
            {
                throw new ArgumentNullException("x");
            }
            catch (InvalidOperationException)
            {
                return "zly";
            }
            catch (Exception)
            {
                return "baza";
            }
        }
    }

    public class PZagniezdzonyTry
    {
        public object Start()
        {
            try
            {
                try
                {
                    int.Parse("1");
                }
                catch (FormatException)
                {
                    return "wewnetrzny";
                }
                VirtualMachine.Hibernate("a");
                throw new InvalidOperationException("po wewnetrznym");
            }
            catch (InvalidOperationException)
            {
                return "zewnetrzny";
            }
        }
    }

    public class PThrowWCatch
    {
        public object Start()
        {
            string wynik;
            try
            {
                try
                {
                    throw new ArgumentException("x");
                }
                catch (ArgumentException)
                {
                    VirtualMachine.Hibernate("w catch");
                    throw new InvalidOperationException("nowy");
                }
            }
            catch (InvalidOperationException e)
            {
                wynik = "wew:" + e.Message;
            }
            return wynik;
        }
    }

    public class PRethrow
    {
        public object Start()
        {
            string wynik;
            try
            {
                try
                {
                    throw new ArgumentException("pierwszy");
                }
                catch (ArgumentException)
                {
                    try
                    {
                        throw new FormatException("drugi");
                    }
                    catch (FormatException)
                    {
                    }
                    VirtualMachine.Hibernate("przed rethrow");
                    throw;
                }
            }
            catch (ArgumentException e)
            {
                wynik = e.Message;
            }
            return wynik;
        }
    }

    public class PDwaFinally
    {
        public string Slad { get; set; } = "";

        public object Start()
        {
            try
            {
                try
                {
                    VirtualMachine.Hibernate("a");
                }
                finally
                {
                    Slad += "wew,";
                }
            }
            finally
            {
                Slad += "zew,";
            }
            return Slad + "po";
        }
    }

    public class PFinallyOdwijanie
    {
        public string Slad { get; set; } = "";

        public object Start()
        {
            try
            {
                Rzuc();
            }
            catch (InvalidOperationException e)
            {
                Slad += "catch:" + e.Message;
            }
            return Slad;
        }

        [Interpret]
        public void Rzuc()
        {
            try
            {
                try
                {
                    throw new InvalidOperationException("x");
                }
                finally
                {
                    Slad += "f1,";
                }
            }
            catch (ArgumentException)
            {
                Slad += "zly,";
            }
            finally
            {
                Slad += "f2,";
            }
        }
    }

    public class PHibernacjaWFinally
    {
        public string Slad { get; set; } = "";

        public object Start()
        {
            try
            {
                Rzuc();
            }
            catch (InvalidOperationException e)
            {
                Slad += "catch:" + e.Message;
            }
            return Slad;
        }

        [Interpret]
        public void Rzuc()
        {
            try
            {
                throw new InvalidOperationException("x");
            }
            finally
            {
                VirtualMachine.Hibernate("w finally");
                Slad += "finally,";
            }
        }
    }

    public class PTryWPetli
    {
        public object Start()
        {
            var wynik = "";
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    wynik += i;
                    throw new InvalidOperationException("x");
                }
                catch (InvalidOperationException)
                {
                    VirtualMachine.Hibernate("a");
                    wynik += "b";
                }
            }
            return wynik;
        }
    }

    public class PWyjatekWWyrazeniu
    {
        public object Start()
        {
            int wynik;
            try
            {
                wynik = Math.Max(1, Rzuc());
            }
            catch (InvalidOperationException)
            {
                wynik = 5;
            }
            return wynik;
        }

        [Interpret]
        public int Rzuc()
        {
            throw new InvalidOperationException("x");
        }
    }

    public class PTryOdPierwszejInstrukcji
    {
        public object Start()
        {
            try
            {
                Rzuc();
                return "bez wyjatku";
            }
            catch (InvalidOperationException)
            {
                return "zlapany";
            }
        }

        public static void Rzuc()
        {
            throw new InvalidOperationException("x");
        }
    }

    public class PWyjatekNaZewnatrz
    {
        public string Slad { get; set; } = "";

        public object Start()
        {
            try
            {
                try
                {
                    throw new ArgumentException("koniec");
                }
                finally
                {
                    Slad += "f1";
                }
            }
            finally
            {
                Slad += ",f2";
            }
        }
    }

    public class PCatchWhen
    {
        public object Start()
        {
            try
            {
                throw new InvalidOperationException("x");
            }
            catch (InvalidOperationException e) when (e.Message == "x")
            {
                return "zlapany";
            }
        }
    }

    public class PParamsSpan
    {
        public object Start()
        {
            string a = "a";
            return string.Join("|", a, "b", "c", "d", "e");
        }
    }

    public class PTryCatch
    {
        public object Start()
        {
            try
            {
                VirtualMachine.Hibernate("a");
                throw new InvalidOperationException("po wznowieniu");
            }
            catch (InvalidOperationException)
            {
                return "zlapany";
            }
        }
    }

    public class PHibernacjaWCatch
    {
        public object Start()
        {
            string tekst;
            try
            {
                throw new InvalidOperationException("blad");
            }
            catch (InvalidOperationException e)
            {
                VirtualMachine.Hibernate("w catch");
                tekst = e.Message;
            }
            return "po catch: " + tekst;
        }
    }

    public class PFinally
    {
        public string Slad { get; set; } = "";
        public object Start()
        {
            try
            {
                VirtualMachine.Hibernate("a");
                Slad += "try";
            }
            finally
            {
                Slad += "+finally";
            }
            return Slad;
        }
    }

    public class PPole
    {
        private int _licznik;
        public object Start()
        {
            _licznik = 5;
            VirtualMachine.Hibernate("a");
            return _licznik;
        }
    }

    public class PInterpolacja
    {
        public object Start()
        {
            int i = 3;
            decimal kwota = 1.5m;
            string x = "x";
            VirtualMachine.Hibernate("a");
            return $"wartosc {i} {kwota.ToString("0.00", new System.Globalization.CultureInfo("pl-PL"))} [{x,3}]";
        }
    }

    public class PLambda
    {
        public object Start()
        {
            int mnoznik = 2;
            Func<int, int> f = x => x * mnoznik;
            VirtualMachine.Hibernate("a");
            return f(3);
        }
    }

    public class PLinq
    {
        public object Start()
        {
            var lista = new List<int> { 1, 2, 3, 4 };
            var parzyste = lista.Where(x => x % 2 == 0);
            VirtualMachine.Hibernate("a");
            return parzyste.Count();
        }
    }

    public class Klient
    {
        public string? Imie { get; set; }
        public List<string> Zgody { get; set; } = new List<string>();
    }

    public class PObiekt
    {
        public Klient? Klient { get; set; }
        public object Start()
        {
            var k = new Klient { Imie = "Jan" };
            Klient = k;
            k.Zgody.Add("a");
            VirtualMachine.Hibernate("a");
            k.Zgody.Add("b");
            return Klient.Imie + "/" + Klient.Zgody.Count + "/" + ReferenceEquals(k, Klient);
        }
    }

    public class PSlownik
    {
        public object Start()
        {
            var slownik = new Dictionary<string, int> { ["a"] = 1 };
            var odwrotny = new Dictionary<int, string> { [1] = "a" };
            var bezWielkosci = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase) { ["b"] = 2 };
            var zbior = new HashSet<string> { "x" };
            VirtualMachine.Hibernate("a");
            zbior.Add("y");
            zbior.Add("x");
            return $"{slownik["a"]}:{odwrotny[1]}|B:{bezWielkosci["B"]}|{zbior.Contains("x")}|{zbior.Count}";
        }
    }

    public class PForeachSlownik
    {
        public object Start()
        {
            var slownik = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2 };
            var wynik = "";
            foreach (var para in slownik)
            {
                VirtualMachine.Hibernate(para.Key);
                wynik += para.Key + para.Value;
            }
            return wynik;
        }
    }

    public class PForeachPoUsunieciu
    {
        public object Start()
        {
            var slownik = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
            slownik.Remove("a");
            foreach (var para in slownik)
            {
                VirtualMachine.Hibernate(para.Key);
            }
            return "koniec";
        }
    }

    public class PWyjatekWZmiennej
    {
        public object Start()
        {
            var wyjatek = new InvalidOperationException("boom", new ArgumentException("wew"));
            VirtualMachine.Hibernate("a");
            return wyjatek.Message + "/" + wyjatek.InnerException!.Message;
        }
    }

    public class PDaneZZewnatrz
    {
        public object Start()
        {
            var dane = VirtualMachine.Hibernate("czekam");
            return dane!;
        }
    }

    public class PStrumien
    {
        private MemoryStream _strumien = new MemoryStream();
        public object Start()
        {
            VirtualMachine.Hibernate("a");
            return _strumien.Length;
        }
    }

    public class PNonSerialized : IDeserializedObject
    {
        [NonSerialized] private string? _usluga = "przed hibernacja";

        public void AfterDeserialization()
        {
            _usluga = "odtworzony";
        }

        public object Start()
        {
            VirtualMachine.Hibernate("a");
            return _usluga!;
        }
    }

    public class PAsync
    {
        public object Start()
        {
            return Pobierz().Result;
        }

        [Interpret]
        public async Task<string> Pobierz()
        {
            await Task.Delay(10);
            return "z async";
        }
    }

    public class PWersjaA
    {
        public int State { get; set; }
        public object Start()
        {
            State += 1;
            VirtualMachine.Hibernate("a");
            State += 10;
            return State;
        }
    }

    public class PWersjaB
    {
        public int State { get; set; }
        public object Start()
        {
            State += 100;
            State += 200;
            State += 300;
            State += 1;
            VirtualMachine.Hibernate("a");
            State += 10;
            return State;
        }
    }
}
