namespace MasterDataFlow.Intelligence.Math
{
    public sealed class Int32OperationProvider : OperationProvider<int>
    {
        public static readonly Int32OperationProvider Instance = new Int32OperationProvider();

        private Int32OperationProvider()
        {
        }

        public override int Add(int op1, int op2)
        {
            return op1 + op2;
        }

        public override int Subtract(int op1, int op2)
        {
            return op1 - op2;
        }

        public override int Multiply(int op1, int op2)
        {
            return op1 * op2;
        }

        public override int Divide(int op1, int op2)
        {
            return op1 / op2;
        }

        public override int Modulo(int op1, int op2)
        {
            return op1 % op2;
        }

        public override int Power(int op1, int op2)
        {
            return (int) System.Math.Pow(op1, op2);
        }

        public override int Increment(int op1)
        {
            return op1 + 1;
        }

        public override int Decrement(int op1)
        {
            return op1 - 1;
        }


        public override int GetHashCode(int op1)
        {
            return op1.GetHashCode();
        }
    }
}