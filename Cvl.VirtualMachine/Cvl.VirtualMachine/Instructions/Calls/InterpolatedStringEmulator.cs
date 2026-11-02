using System.Reflection;
using System.Text;

namespace Cvl.VirtualMachine.Instructions.Calls
{
    /// <summary>
    /// Zastępuje DefaultInterpolatedStringHandler, którego C# używa dla $"..." -
    /// to ref struct, więc nie da się go trzymać w zmiennej lokalnej maszyny ani zapisać w stanie.
    /// </summary>
    public class InterpolatedStringEmulator
    {
        private readonly StringBuilder tekst = new StringBuilder();
        private readonly IFormatProvider? provider;

        public InterpolatedStringEmulator(IFormatProvider? provider)
        {
            this.provider = provider;
        }

        /// <summary>
        /// Wywołuje odpowiednik metody handlera i zwraca jej wynik (tylko ToStringAndClear coś zwraca)
        /// </summary>
        public string? Wykonaj(MethodBase metoda, List<object> parametry)
        {
            switch (metoda.Name)
            {
                case "AppendLiteral":
                    tekst.Append((string)parametry[0]);
                    return null;
                case "AppendFormatted":
                    DopiszWartosc(metoda, parametry);
                    return null;
                case "ToStringAndClear":
                    var wynik = tekst.ToString();
                    tekst.Clear();
                    return wynik;
                default:
                    throw new NotSupportedException($"Maszyna nie obsługuje metody DefaultInterpolatedStringHandler.{metoda.Name}");
            }
        }

        private void DopiszWartosc(MethodBase metoda, List<object> parametry)
        {
            var typWartosci = metoda.IsGenericMethod ? metoda.GetGenericArguments()[0] : metoda.GetParameters()[0].ParameterType;
            var wartosc = InstructionBase.DopasujTyp(parametry[0], typWartosci);

            int wyrownanie = 0;
            string? format = null;
            var opisParametrow = metoda.GetParameters();
            for (int i = 1; i < parametry.Count; i++)
            {
                if (opisParametrow[i].Name == "alignment")
                {
                    wyrownanie = (int)parametry[i];
                }
                else
                {
                    format = (string?)parametry[i];
                }
            }

            var tekstWartosci = wartosc is IFormattable formattable
                ? formattable.ToString(format, provider)
                : wartosc?.ToString();
            tekstWartosci ??= "";

            tekst.Append(wyrownanie >= 0 ? tekstWartosci.PadLeft(wyrownanie) : tekstWartosci.PadRight(-wyrownanie));
        }
    }
}
