using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Cvl.VirtualMachine.UnitTest.Proces.Model
{
    public class DataFormFactory<T>
    {
        public DataFormFactory<T> DataForm(Action<DataFormFactory<T>> panel)
        {
            var factory = new DataFormFactory<T>();
            panel(factory);
            return this;
        }

        public DataFormFactory<T> AddField<TD>(Expression<Func<T, TD>> nazwaPola, string tooltip = null)
        {

            return this;
        }
    }
}
