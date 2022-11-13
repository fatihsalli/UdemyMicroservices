using FreeCourse.Services.Order.Domain.Core;
using System.Collections.Generic;

namespace FreeCourse.Services.Order.Domain.OrderAggreagate
{
    //Bir Id'si olmadığı için ValueObject olarak tanımladık. Özellikleri tutuyor database de kendi başına yer almıyor, Entity içerisinden ulaşıyoruz. 
    public class Address : ValueObject
    {
        //Property durumu dışarıdan set edilememesi için private olarak tanımladık. Private set ettikten sonra nesne oluşturmak için constructordan faydalandık.
        public string Province { get; private set; }
        public string District { get; private set; }
        public string Street { get; private set; }
        public string ZipCode { get; private set; }
        public string Line { get; private set; }

        //Set edilmesini private seçtiğimiz için constructor üzerinden nesne türetiyoruz.
        public Address(string province, string district, string street, string zipCode, string line)
        {
            Province = province;
            District = district;
            Street = street;
            ZipCode = zipCode;
            Line = line;
        }

        //İçerisine yukarıdaki propertyleri tanımlıyoruz. Bu sayede Equals metotu içerisinde bunu kullanarak içeriği aynı mı değil mi kontrol edecek.
        protected override IEnumerable<object> GetEqualityComponents()
        {
            //Yield return ifadesi ile iterator’a çağrı yapılan foreach döngüsüne bir eleman döndürülürken yield break ifadesi ile de artık bulunan iterator içerisindeki iterasyonun sona erdiği bilgisi iterator’ı çağıran foreach döngüsüne iletilmekte. Yield metotu ile metottan return olmadan devam ediyor.
            yield return Province;
            yield return District;
            yield return Street;
            yield return ZipCode;
            yield return Line;
        }
    }
}
