//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace OICAR19_API.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class SCHEDULE_IMAGE
    {
        [JsonIgnore]
        public int IDSCHEDULE_IMAGE { get; set; }
        [JsonProperty(Order = -5)]
        public int POSITION { get; set; }
        public int SCHEDULEID { get; set; }
        public int IMAGEID { get; set; }


        public virtual IMAGE IMAGE { get; set; }
        [JsonIgnore]
        public virtual SCHEDULE SCHEDULE { get; set; }
    }
}
