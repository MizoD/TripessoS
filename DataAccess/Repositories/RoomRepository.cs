using DataAccess.Repositories.IRepositories;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class RoomRepository : Repository<Room> , IRoomRepository
    {
        public RoomRepository(ApplicationDbContext context) : base(context)
        {
        }
        public bool CheckAvailability(int roomId, DateTime checkInDate, DateTime checkOutDate)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.Id == roomId);
            if (room == null || room.AvailableRooms <= 0)
                return false;

            bool isBooked = _context.Bookings.Any(b =>
                b.RoomId == roomId &&
                checkInDate < b.CheckOutDate &&
                checkOutDate > b.CheckInDate
            );

            return !isBooked;
        }
        public bool CheckAvailability(int hotelId)
        {
            return _context.Rooms.Any(r => r.HotelId == hotelId && r.IsAvailable);
        }


    }
}
