using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class CityRepository : ICityRepository
    {
        protected readonly ITrybeHotelContext _context;
        public CityRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        // 4. Refatore o endpoint GET /city
        public IEnumerable<CityDto> GetCities()
        {
            var allCities = (
                from city in _context.Cities
                select new CityDto
                {
                    cityId = city.CityId,
                    name = city.Name,
                    state = city.State
                }
            );

            return allCities;
        }

        // 2. Refatore o endpoint POST /city
        public CityDto AddCity(City city)
        {
            var cityAdd = new City
            {
                Name = city.Name,
                State = city.State
            };

            _context.Cities.Add(cityAdd);
            _context.SaveChanges();
            
            var cityAddDto = new CityDto
            {
                cityId = cityAdd.CityId,
                name = cityAdd.Name,
                state = cityAdd.State
            };

            return cityAddDto;
        }

        // 3. Desenvolva o endpoint PUT /city
        public CityDto UpdateCity(City city)
        {
          var cityExists = _context.Cities.FirstOrDefault(c => c.CityId == city.CityId);

          cityExists.Name = city.Name;
          cityExists.State = city.State;

          _context.SaveChanges();

          return new CityDto
          {
            cityId = cityExists.CityId,
            name = cityExists.Name,
            state = cityExists.State
          };
        }

    }
}