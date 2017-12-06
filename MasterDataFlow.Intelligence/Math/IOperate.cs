using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MasterDataFlow.Intelligence.Math
{
    public interface IOperate<T>
    {
        T Add(T op1, T op2);
        T Subtract(T op1, T op2);
        T Multiply(T op1, T op2);
        T Divide(T op1, T op2);
        T Modulo(T op1, T op2);
        T Power(T op1, T op2);

        T Increment(T op1);
        T Decrement(T op1);

        int GetHashCode(T op1);
        int CompareTo(T op1, T op2);
        bool Equals(T op1, T op2);

        bool Equal(T op1, T op2);
        bool NotEqual(T op1, T op2);
        bool LessThan(T op1, T op2);
        bool GreaterThan(T op1, T op2);
        bool LessOrEqual(T op1, T op2);
        bool GreaterOrEqual(T op1, T op2);
    }
}
