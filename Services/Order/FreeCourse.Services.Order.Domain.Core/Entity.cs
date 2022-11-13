using System;

namespace FreeCourse.Services.Order.Domain.Core
{
    //Domain katmanındaki classlarımız için referans niteliğinde oluşturduk bu klasörü. Microsoft'un hazır bir kütüphanesini kullandık. Domain Driven Design da entity katmanında business kod olabilir.
    public abstract class Entity
    {
        private int? _requestedHashCode;
        private int _Id;

        //İstersek ezebilelim diye virtual olarak tanımladık.
        public virtual int Id
        {
            get => _Id;
            set => _Id = value;
        }

        //Geçici mi değil mi kontrol ediyor. Default bir değeri var ise veritabanında karşılığı yoktur gibi.
        public bool IsTransient()
        {
            return this.Id == default(Int32);
        }

        public override int GetHashCode()
        {
            if (!IsTransient())
            {
                if (!_requestedHashCode.HasValue)
                    _requestedHashCode = this.Id.GetHashCode() ^ 31; // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)

                return _requestedHashCode.Value;
            }
            else
                return base.GetHashCode();
        }

        //İki objenin birbirine eşit olup olmadığını kontrol eder.
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity))
                return false;

            if (Object.ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            Entity item = (Entity)obj;

            if (item.IsTransient() || this.IsTransient())
                return false;
            else
                return item.Id == this.Id;
        }

        //soldaki ile sağdaki itemi karşılaştırmak için kullanıyoruz.
        public static bool operator ==(Entity left, Entity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }

        //soldaki ile sağdaki itemi karşılaştırmak için kullanıyoruz.
        public static bool operator !=(Entity left, Entity right)
        {
            if (Object.Equals(left, null))
                return (Object.Equals(right, null)) ? true : false;
            else
                return left.Equals(right);
        }


    }
}
