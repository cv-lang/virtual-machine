using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Mono.Reflection;

namespace Cvl.VirtualMachine.Core.Serializers
{
    /// <summary>
    /// Kod procesu zmienił się od zapisania stanu, więc zapisany numer instrukcji wskazywałby w inne miejsce
    /// </summary>
    public class ProcessCodeChangedException : StateSerializationException
    {
        public string Method { get; }
        public string SavedFingerprint { get; }
        public string CurrentFingerprint { get; }

        public ProcessCodeChangedException(string method, string savedFingerprint, string currentFingerprint)
            : base($"Kod metody {method} zmienił się od zapisania stanu procesu - wznowienie od zapisanej instrukcji dałoby błędny wynik. "
                   + "Zostaw starą wersję klasy procesu, dopóki zapisane procesy się nie zakończą, a nowe uruchamiaj na nowej klasie.")
        {
            Method = method;
            SavedFingerprint = savedFingerprint;
            CurrentFingerprint = currentFingerprint;
        }
    }

    /// <summary>
    /// Odcisk kodu metody: instrukcje IL, zmienne lokalne i bloki try.
    /// Liczony z rozwiązanych operandów, a nie z surowego IL - tokeny metadanych zmieniają się
    /// przy każdej zmianie w assembly, także w zupełnie innej klasie, a to nie jest zmiana tej metody.
    /// </summary>
    public static class CodeFingerprint
    {
        private static readonly ConcurrentDictionary<MethodBase, string> cache = new();

        public static string Get(MethodBase method)
        {
            return cache.GetOrAdd(method, Compute);
        }

        private static string Compute(MethodBase method)
        {
            var body = method.GetMethodBody();
            if (body == null)
            {
                return "";
            }

            var description = new StringBuilder();
            foreach (var local in body.LocalVariables)
            {
                description.Append("local ").Append(TypeNames.Get(local.LocalType)).Append('\0');
            }

            foreach (var clause in body.ExceptionHandlingClauses)
            {
                description.Append("try ").Append(clause.Flags).Append(' ').Append(clause.TryOffset).Append(' ').Append(clause.TryLength)
                    .Append(' ').Append(clause.HandlerOffset).Append(' ').Append(clause.HandlerLength).Append(' ');
                if (clause.Flags == ExceptionHandlingClauseOptions.Clause)
                {
                    description.Append(TypeNames.Get(clause.CatchType!));
                }
                else if (clause.Flags == ExceptionHandlingClauseOptions.Filter)
                {
                    description.Append(clause.FilterOffset);
                }
                description.Append('\0');
            }

            foreach (var instruction in method.GetInstructions())
            {
                description.Append(instruction.Offset).Append(' ').Append(instruction.OpCode.Name).Append(' ')
                    .Append(DescribeOperand(instruction.Operand)).Append('\0');
            }

            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(description.ToString()));
            return Convert.ToHexString(hash, 0, 16);
        }

        private static string DescribeOperand(object? operand)
        {
            switch (operand)
            {
                case null:
                    return "";
                case Instruction target:
                    return "@" + target.Offset;
                case Instruction[] targets:
                    return string.Join(",", targets.Select(t => "@" + t.Offset));
                case MethodBase method:
                    return MemberIdentity.Get(method);
                case FieldInfo field:
                    return MemberIdentity.Get(field);
                case Type type:
                    return TypeNames.Get(type);
                case LocalVariableInfo local:
                    return "L" + local.LocalIndex;
                case ParameterInfo parameter:
                    return "A" + parameter.Position;
                case byte[] signature:
                    return Convert.ToHexString(signature);
                case IFormattable formattable:
                    return formattable.ToString(null, CultureInfo.InvariantCulture);
                default:
                    return operand.ToString() ?? "";
            }
        }
    }
}
