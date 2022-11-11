using AutoMapper;
using Dapper;
using FreeCourse.Services.Discount.Dtos;
using FreeCourse.Shared.Dtos;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Services.Discount.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IDbConnection _dbConnection;

        public DiscountService(IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
            //Database bağlantı
            _dbConnection = new NpgsqlConnection(_configuration.GetConnectionString("PostreSql"));
        }

        public async Task<Response<NoContent>> Delete(int id)
        {
            //@ işareti ile gösterdiğimiz yeri new {id} yazarak dolduruyoruz. 
            var status=await _dbConnection.ExecuteAsync("delete from discount where id=@Id",new {Id=id});

            return status > 0 ? Response<NoContent>.Success(204) : Response<NoContent>.Fail("Discount not found!", 404);
        }

        public async Task<Response<List<DiscountDto>>> GetAll()
        {
            //Dapper ile direkt olarak Sql'de nasıl sorgu yapıyorsak o şekilde yazdık.
            var discount = await _dbConnection.QueryAsync<Models.Discount>("Select * from discount");
            var discountDto=_mapper.Map<List<DiscountDto>>(discount.ToList());

            return Response<List<DiscountDto>>.Success(discountDto, 200);
        }

        public async Task<Response<DiscountDto>> GetByCodeAndUserId(string code, string userId)
        {
            var discount = (await _dbConnection.QueryAsync<Models.Discount>("select * from discount where userid=@UserId and discountcode=@DiscountCode", new {UserId=userId,DiscountCode=code})).SingleOrDefault();

            if (discount == null)
            {
                return Response<DiscountDto>.Fail("Discount not found", 404);
            }
            var discountDto = _mapper.Map<DiscountDto>(discount);

            return Response<DiscountDto>.Success(discountDto, 200);
        }

        public async Task<Response<DiscountDto>> GetById(int id)
        {
            //@ işareti ile gösterdiğimiz yeri new {id} yazarak dolduruyoruz. 
            var discount=(await _dbConnection.QueryAsync<Models.Discount>("select * from discount where id=@Id", new { Id=id })).SingleOrDefault();
            if (discount==null)
            {
                return Response<DiscountDto>.Fail("Discount not found!", 404);
            }
            var discountDto = _mapper.Map<DiscountDto>(discount);

            return Response<DiscountDto>.Success(discountDto, 200);
        }

        public async Task<Response<NoContent>> Save(DiscountDto discountDto)
        {
            var discount=_mapper.Map<Models.Discount>(discountDto);
            //Gelen model üzerinden okuyup ekleyecektir.
            var saveStatus = await _dbConnection.ExecuteAsync("INSERT INTO discount (userid,rate,discountcode) VALUES(@UserId,@Rate,@DiscountCode)", discount);

            if (saveStatus>0)
            {
                return Response<NoContent>.Success(204);
            }

            return Response<NoContent>.Fail("an error accured while adding!", 500);
        }

        public async Task<Response<NoContent>> Update(DiscountDto discountDto)
        {
            var discount = _mapper.Map<Models.Discount>(discountDto);
            //İsimsiz class yardımıyla bu şekilde de yazabiliriz.
            var updateStatus=await _dbConnection.ExecuteAsync("update discount set userid=@UserId,discountcode=@DiscountCode,rate=@Rate where id=@Id", new { UserId=discount.UserId, DiscountCode = discount.DiscountCode,Rate=discount.Rate,Id=discount.Id});

            if (updateStatus > 0)
            {
                return Response<NoContent>.Success(204);
            }

            return Response<NoContent>.Fail("Discount not found!", 404);
        }
    }
}
