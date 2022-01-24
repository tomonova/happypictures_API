using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OICAR19_API.Models
{
    public class Status
    {
        public static readonly int SHARED = 1; 
        public static readonly int NOT_SHARED = 0;
        public static readonly int DELETED = 1;
        public static readonly int NOT_DELETED = 0; 
        public static readonly int LIKED = 1;
        public static readonly string ADMIN_ACCOUNT = "admin@happypictures.org";
    }
}