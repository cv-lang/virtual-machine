using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine
{
    public class WirtualnaMaszyna
    {
        public HardwareContext HardwareContext { get; set; } = new HardwareContext();

        internal static void HibernateVirtualMachine()
        {
            throw new NotImplementedException();
        }

        internal static void EndProcessVirtualMachine()
        {
            throw new NotImplementedException();
        }

        public void WykonajMetode()
        {

        }

        public void Start(string nazwaMetody, object process)
        {
            
        }
    }
}
