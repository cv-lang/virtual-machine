using System;
using System.Collections.Generic;
using System.Text;

namespace Cvl.VirtualMachine.UnitTest.Basic.Arithmetic
{
    public class ArithmeticTestProcess
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        public int Sub(int a, int b)
        {
            return a - b;
        }

        public int Mul(int a, int b)
        {
            return a * b;
        }

        public int Div(int a, int b)
        {
            return a / b;
        }
    }
}
