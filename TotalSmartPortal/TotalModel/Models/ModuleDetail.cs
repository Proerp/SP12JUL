//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TotalModel.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ModuleDetail
    {
        public int ModuleDetailID { get; set; }
        public int TaskID { get; set; }
        public int ModuleID { get; set; }
        public string SoftDescription { get; set; }
        public string Description { get; set; }
        public string DescriptionEN { get; set; }
        public string Actions { get; set; }
        public string Controller { get; set; }
        public double LastOpen { get; set; }
        public double SerialID { get; set; }
        public string ImageIndex { get; set; }
        public double InActive { get; set; }
        public bool Enabled { get; set; }
        public int ModuleLineID { get; set; }
        public string SoftName { get; set; }
    }
}