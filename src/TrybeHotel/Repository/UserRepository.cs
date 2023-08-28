using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class UserRepository : IUserRepository
    {
        protected readonly ITrybeHotelContext _context;
        public UserRepository(ITrybeHotelContext context)
        {
            _context = context;
        }
        public UserDto GetUserById(int userId)
        {
            throw new NotImplementedException();
        }

        public UserDto Login(LoginDto login)
        {
           var userContext = _context.Users.FirstOrDefault(u => u.Email == login.Email && u.Password == login.Password);
            
            if (userContext == null)
            {
                return null;
            }

            return new UserDto
            {
                userId = userContext.UserId,
                Name = userContext.Name,
                Email = userContext.Email,
                userType = userContext.UserType
            }; 


           
        }
        public UserDto Add(UserDtoInsert user)
        {
            var newUser = new User 
            {
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                UserType = "client"
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            return new UserDto
            {
                userId = newUser.UserId,
                Name = newUser.Name,
                Email = newUser.Email,
                userType = newUser.UserType
            };

        }

        public UserDto GetUserByEmail(string userEmail)
        {
            var userContext = _context.Users.FirstOrDefault(u => u.Email == userEmail);

            if (userContext == null)
            {
                return null;
            }

            return new UserDto
            {
                userId = userContext.UserId,
                Name = userContext.Name,
                Email = userContext.Email,
                userType = userContext.UserType
            }; 

        }

        public IEnumerable<UserDto> GetUsers()
        {
           var allUsersDto = _context.Users
               .Select(user => new UserDto
                {
                    userId = user.UserId,
                    Name = user.Name,                
                    Email = user.Email,
                    userType = user.UserType
                });
            return allUsersDto;     
        }
    }
}