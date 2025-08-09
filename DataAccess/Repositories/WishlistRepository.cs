using DataAccess.Repositories.IRepositories;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class WishlistRepository : Repository<Wishlist> , IWishlistRepository
    {
        private readonly ApplicationDbContext _context;

        public WishlistRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public IEnumerable<Wishlist> GetAll()
        {
            return _context.Wishlists.ToList();
        }

        public void Add(Wishlist wishlist)
        {
            _context.Wishlists.Add(wishlist);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
