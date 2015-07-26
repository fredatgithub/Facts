namespace Theraot.Facts
{
    public abstract class BinaryFactCheck<T1, T2, TTarget> : IFactCheck<T1, T2, TTarget>
    {
        protected BinaryFactCheck(IFactCheck<T1, T2, TTarget> left, IFactCheck<T1, T2, TTarget> right)
        {
            Left = left;
            Right = right;
        }

        public IFactCheck<T1, T2, TTarget> Left { get; private set; }
        public IFactCheck<T1, T2, TTarget> Right { get; private set; }
    }

    public class EqualsFactCheck<T1, T2, TTarget> : BinaryFactCheck<T1, T2, TTarget>
    {
        public EqualsFactCheck(IFactCheck<T1, T2, TTarget> left, IFactCheck<T1, T2, TTarget> right)
            : base(left, right)
        {
            // Empty
        }
    }

    public class Fact<T1, T2>
    {
        public Fact()
        {
            Item1 = new FactData<T1, T2, T1>(this, 0);
            Item2 = new FactData<T1, T2, T2>(this, 1);
        }

        public FactData<T1, T2, T1> Item1 { get; private set; }

        public FactData<T1, T2, T2> Item2 { get; private set; }

        public void Add(T1 peter, T2 paul)
        {
            throw new System.NotImplementedException();
        }
    }

#pragma warning disable 660,661 // Not meant to represent data

    public class FactData<T1, T2, TTarget> : IFactCheck<T1, T2, TTarget>
    {
        internal FactData(Fact<T1, T2> owner, int itemIndex)
        {
            ItemIndex = itemIndex;
            Owner = owner;
        }

        internal int ItemIndex { get; private set; }

        internal Fact<T1, T2> Owner { get; private set; }

        public static IFactCheck<T1, T2, TTarget> operator !=(FactData<T1, T2, TTarget> left, TTarget right)
        {
            return new ValueNotEqualsFactCheck<T1, T2, TTarget>(left, right);
        }

        public static IFactCheck<T1, T2, TTarget> operator !=(TTarget left, FactData<T1, T2, TTarget> right)
        {
            return new ValueNotEqualsFactCheck<T1, T2, TTarget>(right, left);
        }

        public static IFactCheck<T1, T2, TTarget> operator ==(FactData<T1, T2, TTarget> left, TTarget right)
        {
            return new ValueEqualsFactCheck<T1, T2, TTarget>(left, right);
        }

        public static IFactCheck<T1, T2, TTarget> operator ==(TTarget left, FactData<T1, T2, TTarget> right)
        {
            return new ValueEqualsFactCheck<T1, T2, TTarget>(right, left);
        }
    }

#pragma warning restore 660, 661

    public class NotEqualsFactCheck<T1, T2, TTarget> : BinaryFactCheck<T1, T2, TTarget>
    {
        public NotEqualsFactCheck(IFactCheck<T1, T2, TTarget> left, IFactCheck<T1, T2, TTarget> right)
            : base(left, right)
        {
            // Empty
        }
    }

    public class ValueEqualsFactCheck<T1, T2, TTarget> : IFactCheck<T1, T2, TTarget>
    {
        public ValueEqualsFactCheck(IFactCheck<T1, T2, TTarget> item, TTarget value)
        {
            Item = item;
            Value = value;
        }

        public IFactCheck<T1, T2, TTarget> Item { get; private set; }
        public TTarget Value { get; private set; }
    }

    public class ValueNotEqualsFactCheck<T1, T2, TTarget> : IFactCheck<T1, T2, TTarget>
    {
        public ValueNotEqualsFactCheck(IFactCheck<T1, T2, TTarget> item, TTarget value)
        {
            Item = item;
            Value = value;
        }

        public IFactCheck<T1, T2, TTarget> Item { get; private set; }
        public TTarget Value { get; private set; }
    }
}