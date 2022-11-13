using System;
using System.Collections.Generic;
using System.Linq;

namespace FreeCourse.Services.Order.Domain.Core
{
    //Microsofttan hazır bir kütüphane kullandık.
    public abstract class ValueObject
    {
        //soldaki ile sağdaki itemi karşılaştırmak için kullanıyoruz.
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            if (ReferenceEquals(left, null) ^ ReferenceEquals(right, null))
            {
                return false;
            }
            return ReferenceEquals(left, null) || left.Equals(right);
        }

        //soldaki ile sağdaki itemi karşılaştırmak için kullanıyoruz.
        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {
            return !(EqualOperator(left, right));
        }

        //Miras aldığımız classta bu metotu ezmemiz gerekiyor. Equals kısmında tipleri eşit olan classların içeriğini kontrol ediyoruz.
        protected abstract IEnumerable<object> GetEqualityComponents();

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (ValueObject)obj;

            return this.GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
             .Select(x => x != null ? x.GetHashCode() : 0)
             .Aggregate((x, y) => x ^ y);
        }

        public ValueObject GetCopy()
        {
            return this.MemberwiseClone() as ValueObject;
        }

    }
}
