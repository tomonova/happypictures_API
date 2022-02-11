using OICAR19_API.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OICAR19_API.Tests
{
    class TestImageDbContext : HappyPicturesDbContext_UnitTest
    {
        public TestImageDbContext()
        {
            this.Images = new TestImagesDbSet();
        }
        public DbSet<IMAGE> Images { get; set; }

        public DbSet<STORy> Stories { get; set; }

        public void Dispose() { }

        public void MarkAsModified(IMAGE item) {  }

        public int SaveChanges()
        {
            return 0;
        }
    }
}
