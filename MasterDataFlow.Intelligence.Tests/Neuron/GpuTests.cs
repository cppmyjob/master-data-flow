using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Alea;
using Alea.Parallel;

namespace MasterDataFlow.Intelligence.Tests.Neuron
{
    [TestClass]
    public class GpuTests
    {

        private const int Length = 1000000;

        [TestMethod]
        public void Exp()
        {

            var arg1 = Enumerable.Range(0, Length).ToArray();
            var result = new int[Length];

            var sw = System.Diagnostics.Stopwatch.StartNew();
            sw.Start();
            Parallel.For(0, result.Length, i => result[i] = (int)Alea.DeviceFunction.Exp((float)-arg1[i]));
            sw.Stop();
            Console.WriteLine("Exp Gpu:{0}ms", sw.ElapsedMilliseconds);


            sw = System.Diagnostics.Stopwatch.StartNew();
            sw.Start();
            Parallel.For(0, result.Length, i => result[i] = (int)System.Math.Exp(-arg1[i]));
            sw.Stop();
            Console.WriteLine("Exp Cpu:{0}ms", sw.ElapsedMilliseconds);
        }

        [TestMethod]
        public void DelegateWithClosureCpu()
        {
            

            var arg1 = Enumerable.Range(0, Length).ToArray();
            var arg2 = Enumerable.Range(0, Length).ToArray();
            var result = new int[Length];

            Parallel.For(0, result.Length, i => result[i] = arg1[i] + arg2[i]);

            var expected = arg1.Zip(arg2, (x, y) => x + y);

            Assert.Equals(expected, result);
        }

        [GpuManaged, TestMethod]
        public void DelegateWithClosureGpu()
        {
            var arg1 = Enumerable.Range(0, Length).ToArray();
            var arg2 = Enumerable.Range(0, Length).ToArray();
            var result = new int[Length];

            Gpu.Default.For(0, result.Length, i => result[i] = arg1[i] + arg2[i]);

            var expected = arg1.Zip(arg2, (x, y) => x + y);

            Assert.AreEqual(expected, result);
        }

        //[GpuManaged, TestMethod]
        //public static void ActionWithClosure()
        //{
        //    var gpu = Gpu.Default;
        //    var arg1 = Enumerable.Range(0, Length).ToArray();
        //    var arg2 = Enumerable.Range(0, Length).ToArray();
        //    var result = new int[Length];

        //    Action<int> op = i => result[i] = arg1[i] + arg2[i];

        //    gpu.For(0, arg1.Length, op);

        //    var expected = arg1.Zip(arg2, (x, y) => x + y);

        //    Assert.AreEqual(expected, result);
        //}

        //[GpuManaged, TestMethod]
        //public static void ActionFactoryWithClosure()
        //{
        //    var gpu = Gpu.Default;
        //    var arg1 = Enumerable.Range(0, Length).ToArray();
        //    var arg2 = Enumerable.Range(0, Length).ToArray();
        //    var result = new int[Length];
        //    var expected = new int[Length];

        //    Func<int[], Action<int>> opFactory = res => i => res[i] = arg1[i] + arg2[i];

        //    gpu.For(0, arg1.Length, opFactory(result));

        //    Parallel.For(0, arg1.Length, opFactory(expected));

        //    Assert.AreEqual(expected, result);
        //}

    }
}
