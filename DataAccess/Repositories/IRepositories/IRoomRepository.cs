using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.IRepositories
{
    public interface IRoomRepository : IRepository<Room>
    {
        bool CheckAvailability(int roomId, DateTime checkInDate, DateTime checkOutDate);
        bool CheckAvailability(int hotelId);

    }
}
