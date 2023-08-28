using TrybeHotel.Models;
using TrybeHotel.Dto;

namespace TrybeHotel.Repository
{
    public class BookingRepository : IBookingRepository
    {
        protected readonly ITrybeHotelContext _context;
        public BookingRepository(ITrybeHotelContext context)
        {
            _context = context;
        }

        // 9. Refatore o endpoint POST /booking
        public BookingResponse Add(BookingDtoInsert booking, string email)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.RoomId == booking.RoomId);
            var hotel = _context.Hotels.FirstOrDefault(h => h.HotelId == room.HotelId);
            _context.Cities.FirstOrDefault(c => c.CityId == hotel.CityId);
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            if (room == null || booking.GuestQuant > room.Capacity)
            {
                return null;
            }

            var newBooking = new Booking
            {
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestQuant = booking.GuestQuant,
                UserId = user.UserId,
                RoomId = room.RoomId
            };

            _context.Bookings.Add(newBooking);
            _context.SaveChanges();

            return new BookingResponse
            {
                bookingId = newBooking.BookingId,
                checkIn = newBooking.CheckIn,
                checkOut = newBooking.CheckOut,
                guestQuant = newBooking.GuestQuant,
                room = new RoomDto
                {
                    roomId = room.RoomId,
                    name = room.Name,
                    capacity = room.Capacity,
                    image = room.Image,
                    hotel = new HotelDto
                    {
                        hotelId = room.Hotel.HotelId,
                        name = room.Hotel.Name,
                        address = room.Hotel.Address,
                        cityId = room.Hotel.CityId,
                        cityName = room.Hotel.City.Name,
                        state = room.Hotel.City.State
                    }

                }
            };


        }

        // 10. Refatore o endpoint GET /booking
        public BookingResponse GetBooking(int bookingId, string email)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            
            var bookingResponse = _context.Bookings
                .Where(b => b.UserId == user.UserId && b.BookingId == bookingId)
                .Select(b => new BookingResponse
                {
                    bookingId = b.BookingId,
                    checkIn = b.CheckIn,
                    checkOut = b.CheckOut,
                    guestQuant = b.GuestQuant,
                    room = new RoomDto
                    {
                        roomId = b.Room.RoomId,
                        name = b.Room.Name,
                        capacity = b.Room.Capacity,
                        image = b.Room.Image,
                        hotel = new HotelDto
                        {
                            hotelId = b.Room.Hotel.HotelId,
                            name = b.Room.Hotel.Name,
                            address = b.Room.Hotel.Address,
                            cityId = b.Room.Hotel.CityId,
                            cityName = b.Room.Hotel.City.Name,
                            state = b.Room.Hotel.City.State
                        }
                    }
                }).FirstOrDefault();

            return bookingResponse;    
        }

        public Room GetRoomById(int RoomId)
        {
            return _context.Rooms.FirstOrDefault(r => r.RoomId == RoomId);
        }

    }

}