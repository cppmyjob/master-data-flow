using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Math
{
    public abstract class OperationProvider<T> : IOperate<T>
        where T : System.IComparable<T>, System.IEquatable<T>
    {
        public abstract T Add(T op1, T op2);
        public abstract T Subtract(T op1, T op2);
        public abstract T Multiply(T op1, T op2);
        public abstract T Divide(T op1, T op2);
        public abstract T Modulo(T op1, T op2);
        public abstract T Power(T op1, T op2);

        public abstract T Increment(T op1);
        public abstract T Decrement(T op1);

        public abstract int GetHashCode(T op1);

        public int CompareTo(T op1, T op2) { return (op1.CompareTo(op2)); }
        public bool Equals(T op1, T op2) { return (op1.Equals(op2)); }

        public bool Equal(T op1, T op2) { return (op1.CompareTo(op2) == 0); }
        public bool NotEqual(T op1, T op2) { return (op1.CompareTo(op2) != 0); }
        public bool LessThan(T op1, T op2) { return (op1.CompareTo(op2) < 0); }
        public bool GreaterThan(T op1, T op2) { return (op1.CompareTo(op2) > 0); }
        public bool LessOrEqual(T op1, T op2) { return (op1.CompareTo(op2) <= 0); }
        public bool GreaterOrEqual(T op1, T op2) { return (op1.CompareTo(op2) >= 0); }
    }
}
