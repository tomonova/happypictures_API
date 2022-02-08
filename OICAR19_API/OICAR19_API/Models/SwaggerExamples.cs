using Swashbuckle.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OICAR19_API.Models
{
    public class SwaggerExamples : IExamplesProvider
    {
        public virtual object GetExamples()
        {
            List<SCHEDULE_IMAGE> lista = new List<SCHEDULE_IMAGE>();
            lista.Add(new SCHEDULE_IMAGE() { IMAGEID = 2, POSITION = 1 });
            var value = new SCHEDULE()
            {
                PROFILEID = 2,
                SCHEDULE_DATE = new DateTime(2022, 10, 22)
            };
            return value;
        }
    }
}