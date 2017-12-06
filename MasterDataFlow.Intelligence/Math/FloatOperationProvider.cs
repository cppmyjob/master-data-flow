using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Math
{
    public class FloatOperationProvider : OperationProvider<float>
    {
        public static readonly FloatOperationProvider Instance = new FloatOperationProvider();

        private FloatOperationProvider()
        {
        }

        public override float Add(float op1, float op2)
        {
            return op1 + op2;
        }

        public override float Subtract(float op1, float op2)
        {
            return op1 - op2;
        }

        public override float Multiply(float op1, float op2)
        {
            return op1 * op2;
        }

        public override float Divide(float op1, float op2)
        {
            return op1 / op2;
        }

        public override float Modulo(float op1, float op2)
        {
            return op1 % op2;
        }

        public override float Power(float op1, float op2)
        {
            return (int)System.Math.Pow(op1, op2);
        }

        public override float Increment(float op1)
        {
            return op1 + 1;
        }

        public override float Decrement(float op1)
        {
            return op1 - 1;
        }

        public override int GetHashCode(float op1)
        {
            return op1.GetHashCode();
        }
    }
}
