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
    using System;
    using System.Collections.Generic;
    
    public partial class STORY_CARD
    {
        public int STORYID { get; set; }
        public int CARDID { get; set; }
        public int CARD_ORDER { get; set; }
    
        public virtual CARD CARD { get; set; }
        public virtual STORy STORy { get; set; }
    }
}
