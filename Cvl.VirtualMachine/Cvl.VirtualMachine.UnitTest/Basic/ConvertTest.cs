using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Cvl.VirtualMachine.UnitTest.Basic
{
    public class ConvertTest
    {
        [SetUp]
        public void Setup() { }

        [Test]
        public void Test1()
        {
            var vm = new VirtualMachine();
            var process = new ConvertTestProcess();

            Assert.AreEqual(process.Convert_i(10.5), vm.StartTestExecution<int>("conv.i", process, 10.5));

            Assert.AreEqual(process.Convert_i1(10.5), vm.StartTestExecution<SByte>("conv.i1", process, 10.5));
            Assert.AreEqual(process.Convert_i1(200), vm.StartTestExecution<SByte>("conv.i1", process, 200));

            Assert.AreEqual(process.Convert_i2(10.5), vm.StartTestExecution<short>("conv.i2", process, 10.5));
            Assert.AreEqual(process.Convert_i2(40000), vm.StartTestExecution<short>("conv.i2", process, 40000));

            Assert.AreEqual(process.Convert_i4(10.5), vm.StartTestExecution<int>("conv.i4", process, 10.5));

            Assert.AreEqual(process.Convert_i8(10.5), vm.StartTestExecution<int>("conv.i8", process, 10.5));

            Assert.AreEqual(process.Convert_r(10.5), vm.StartTestExecution<float>("conv.r", process, 10.5));

            Assert.AreEqual(process.Convert_r4(10.5), vm.StartTestExecution<float>("conv.r4", process, 10.5));

            Assert.AreEqual(process.Convert_r8(10.5), vm.StartTestExecution<double>("conv.r8", process, 10.5));

            Assert.AreEqual(process.Convert_u(10.5), vm.StartTestExecution<uint>("conv.u", process, 10.5));
            Assert.AreEqual(process.Convert_u(-10.5), vm.StartTestExecution<uint>("conv.u", process, -10.5));

            Assert.AreEqual(process.Convert_u1(10.5), vm.StartTestExecution<byte>("conv.u1", process, 10.5));
            Assert.AreEqual(process.Convert_u1(-10), vm.StartTestExecution<byte>("conv.u1", process, -10));
            Assert.AreEqual(process.Convert_u1(500), vm.StartTestExecution<byte>("conv.u1", process, 500));

            Assert.AreEqual(process.Convert_u2(10.5), vm.StartTestExecution<ushort>("conv.u2", process, 10.5));
            Assert.AreEqual(process.Convert_u2(70000), vm.StartTestExecution<ushort>("conv.u2", process, 70000));
            Assert.AreEqual(process.Convert_u2(-10.5), vm.StartTestExecution<ushort>("conv.u2", process, -10.5));

            Assert.AreEqual(process.Convert_u4(10.5), vm.StartTestExecution<UInt32>("conv.u4", process, 10.5));
            Assert.AreEqual(process.Convert_u4(-10.5), vm.StartTestExecution<UInt32>("conv.u4", process, 10.5));

            Assert.AreEqual(process.Convert_u8(10.5), vm.StartTestExecution<UInt64>("conv.u8", process, 10.5));
            Assert.AreEqual(process.Convert_u8(-10.5), vm.StartTestExecution<UInt64>("conv.u8", process, 10.5));

        }
    }

    public class ConvertTestProcess
    {
        #region conv.i
        public int Convert_i(object value)
        {
            return Convert.ToInt32(value);
        }
        public SByte Convert_i1(object value)
        {
            return Convert.ToSByte(value);
        }
        public short Convert_i2(object value)
        {
            return (short)value;
        }
        public int Convert_i4(object value)
        {
            return Convert.ToInt32(value);
        }
        public int Convert_i8(object value)
        {
            return Convert.ToInt32(value);
        }
        #endregion
        #region conv.r
        public float Convert_r(object value)
        {
            return (float)value;
        }
        public float Convert_r4(object value)
        {
            return (float)value;
        }
        public double Convert_r8(object value)
        {
            return (double)value;
        }
        #endregion
        #region conv.u
        public uint Convert_u(object value)
        {
            return Convert.ToUInt32(value);
        }
        public byte Convert_u1(object value)
        {
            return Convert.ToByte(value);
        }
        public ushort Convert_u2(object value)
        {
            return (ushort)Convert.ToInt32(value);
        }
        public UInt32 Convert_u4(object value)
        {
            return Convert.ToUInt32(value);
        }
        public UInt64 Convert_u8(object value)
        {
            return Convert.ToUInt64(value);
        }
        #endregion
    }
}
