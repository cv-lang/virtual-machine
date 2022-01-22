using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.Core.Attributes
{
    /// <summary>
    /// Atrybut oznacza że dana metoda powinna być interpretowana przez wirutalną maszynę
    /// Metody bez tego znacznika będą wykonane
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class InterpretAttribute : System.Attribute
    {
    }
}
