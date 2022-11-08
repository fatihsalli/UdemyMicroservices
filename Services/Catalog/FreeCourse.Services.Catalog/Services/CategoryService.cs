using AutoMapper;
using FreeCourse.Services.Catalog.Dtos;
using FreeCourse.Services.Catalog.Models;
using FreeCourse.Services.Catalog.Settings;
using FreeCourse.Shared.Dtos;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FreeCourse.Services.Catalog.Services
{
    //CategoryRepository oluşturmadık bunun nedeni service classında hem database ile hem de mapleme işlemlerini yapmak için. Ayrıca repository ve bunu implente edildiği bir service class'ı da oluşturabilirdik.
    public class CategoryService:ICategoryService
    {
        private readonly IMongoCollection<Category> _categoryCollection;

        private readonly IMapper _mapper;

        public CategoryService(IMapper mapper,IDatabaseSettings databaseSettings)
        {
            //Client'a bağlanmak için "databaseSettings" üzerinden ConnectionString değerini veriyoruz.
            var client=new MongoClient(databaseSettings.ConnectionString);

            //Clientımızı oluşturduk client üzerinden veritabanını aldım. "databaseSettings.DatabaseName" hangi database'e bağlanmak istediğimizi belirttik.
            var database = client.GetDatabase(databaseSettings.DatabaseName);

            //Yukarıda aldığımız database üzerinden collection alıyoruz. Kategori adını parametre olarak "databaseSettings.CategoryCollectionName" olarak veriyoruz. Artık elimizde dolu bir categoryCollection mevcut.
            _categoryCollection = database.GetCollection<Category>(databaseSettings.CategoryCollectionName);
            _mapper = mapper;
        }

        public async Task<Response<List<CategoryDto>>> GetAllAsync()
        {
            //Bir parametre istediği için (category => true) olarak tanımlama yaptık tamamını vermesi için
            var categories = await _categoryCollection.Find(category => true).ToListAsync();
            return Response<List<CategoryDto>>.Success(_mapper.Map<List<CategoryDto>>(categories), 200);          
        }

        //Kursta CreateAsync(Category category) olarak yazıldı hatalı mı???
        public async Task<Response<CategoryDto>> CreateAsync(CategoryDto categoryDto)
        {
            var category=_mapper.Map<Category>(categoryDto);
            await _categoryCollection.InsertOneAsync(category);

            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), 200);

        }

        public async Task<Response<CategoryDto>> GetByIdAsync(string id)
        {
            //Id ile ilgili category'i bulduk.
            var category=await _categoryCollection.Find(x=> x.Id == id).FirstOrDefaultAsync();

            if (category==null)
            {
                return Response<CategoryDto>.Fail($"Category ({category.Id}) not found!", 404);
            }

            return Response<CategoryDto>.Success(_mapper.Map<CategoryDto>(category), 200);
        }




    }
}
