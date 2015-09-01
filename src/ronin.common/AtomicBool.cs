using System.Threading;

namespace Ronin.Common
{
    public class AtomicBool
    {

        private const int ValueTrue = 1;
        private const int ValueFalse = 0;

        private int currentValue;

        public AtomicBool(bool initialValue = false)
        {
            this.currentValue = BoolToInt(initialValue);
        }

        private int BoolToInt(bool value)
        {
            return value ? ValueTrue : ValueFalse;
        }

        private bool IntToBool(int value)
        {
            return value == ValueTrue;
        }

        public bool Value
        {
            get
            {
                return IntToBool(Interlocked.Add(
                ref this.currentValue, 0));
            }
        }
        public bool SetValue(bool newValue)
        {
            return IntToBool(
            Interlocked.Exchange(ref this.currentValue,
            BoolToInt(newValue)));
        }

        public bool CompareAndSet(bool expectedValue,
            bool newValue)
        {
            int expectedVal = BoolToInt(expectedValue);
            int newVal = BoolToInt(newValue);
            return Interlocked.CompareExchange(
            ref this.currentValue, newVal, expectedVal) == expectedVal;
        }
    }
}
