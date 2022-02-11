using OICAR19_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OICAR19_API.Tests
{
    class TestImagesDbSet:TestDBSet<IMAGE>
    {
        public override IMAGE Find(params object[] keyValues)
        {
            return this.SingleOrDefault(img => img.IDIMAGE == (int)keyValues.Single());
        }
    }
}
