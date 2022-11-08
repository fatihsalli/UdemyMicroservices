namespace FreeCourse.Services.Catalog.Settings
{
    public interface IDatabaseSettings
    {
        //appsettings alanındaki "DatabaseSettings" ayarlarına karşılık gelen propertyler oluşturarak options patterni kullanıyoruz.

        public string CourseCollectionName { get; set; }
        public string CategoryCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }


    }
}
