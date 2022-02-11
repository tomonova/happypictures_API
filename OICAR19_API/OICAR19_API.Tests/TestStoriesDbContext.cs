using OICAR19_API.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OICAR19_API.Tests
{
    class TestStoriesDbContext : HappyPicturesDbContext_UnitTest

    {
        public DbSet<IMAGE> Images { get; set; }

        public DbSet<STORy> Stories { get; set; }

        public void Dispose() { }

        public void MarkAsModified(STORy item) { }

        public void MarkAsModified(IMAGE item)
        {
            throw new NotImplementedException();
        }

        public int SaveChanges() => 0;
    }
}
