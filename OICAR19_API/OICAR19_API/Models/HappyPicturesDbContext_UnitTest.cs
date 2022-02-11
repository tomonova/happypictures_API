using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OICAR19_API.Models
{
    public interface HappyPicturesDbContext_UnitTest :IDisposable
    {
        DbSet<IMAGE> Images { get; }
        DbSet<STORy> Stories { get; }
        int SaveChanges();
        void MarkAsModified(IMAGE item);
    }
}
