namespace FreeCourse.Shared.Messages
{
    //Catalog.Api tarafında bir kurs adı değiştiğinde Order tarafında da değişmesi için event oluşturduk. Bu eventı basket ve yine kurs adının değişmesinden etkilenen diğer mikroservislerde dinleyerek kullanabilir.
    public class CourseNameChangedEvent
    {
        public string CourseId { get; set; }
        public string UpdatedName { get; set; }


    }
}
