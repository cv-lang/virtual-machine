using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Cvl.VirtualMachine.UnitTest.Basic
{
    public class NullThisTest
    {
        [Test]
        public void Test1()
        {
            Nullable<Int32> i1 = 33;
            Nullable<Int32> i2 = 33;
            i1 = null;

            var ret = i1.Equals(i2);
            Assert.IsFalse(ret);

            try
            {

                object instance = i1;
                var equalsMethod = typeof(object)
                     .GetMethod("Equals",
                     System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                var call = Expression.Call(
                        Expression.Constant(instance, typeof(object)),
                        equalsMethod,
                        Expression.Convert(Expression.Constant(i2), typeof(object)));
                var lambda = Expression.Lambda<Func<bool>>(call).Compile();
                var ret3 = lambda(); // prints false 

                ////i1 = 3;
                //var equalsMethod = typeof(object)
                //    .GetMethod("Equals",
                //    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

                //var ret2= equalsMethod.Invoke((Nullable<int>)i1, 
                //    System.Reflection.BindingFlags.Static
                //    , null, new object[] { i2 }, null);
                ////var privateMEthods = typeof(RuntimeMethodHandle).GetMethods(
                ////    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

                ////dynamic em = equalsMethod;
                ////dynamic sig = em.UnsafeInvokeInternal(i1, null, null);

                ////var im = privateMEthods[30];

                ////var ret3 =im.Invoke(null, new object[] { i1, new object[] { i2 }, null, false});
            } catch(Exception ex)
            {

            }            
        }
    }
}
