using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.UnitTest.Proces.Model
{
    /// <summary>
    /// Testowe repozytorium
    /// </summary>
    public class Repozytorium
    {
        public virtual List<ObiektBiznesowy> PobierzObiekty()
        {
            return null;
        }

        public virtual List<ObiektBiznesowy> PobierzObiekty(string filtry)
        {
            return null;
        }

        public virtual List<ObiektBiznesowy> PobierzObiekty(string filtry, DateTime od)
        {
            return null;
        }

        public virtual List<ObiektBiznesowy> PobierzObiekty(string filtry, DateTime? od)
        {
            return null;
        }

        public virtual List<ObiektBiznesowy> PobierzObiekty(string filtry, DateTime? od, DateTime? _do)
        {
            return null;
        }
    }


    /// <summary>
    /// Testowe repozytorium typowane
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RepozytoriumTypowane<T> : Repozytorium
        where T : ObiektBiznesowy, new()
    {
        public virtual List<T> PobierzObiektyTypowane()
        {
            var lista = new List<T>();
            lista.Add(new T());
            lista.Add(new T());
            return lista;
        }
    }
}
