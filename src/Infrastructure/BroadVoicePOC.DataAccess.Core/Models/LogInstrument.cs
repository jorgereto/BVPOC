﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace BroadVoicePOC.DataAccess.Models
{
    public partial class LogInstrument
    {
        public int LogInstrumentId { get; set; }
        public string Username { get; set; }
        public DateTime Date { get; set; }
        public string Resource { get; set; }
        public string CallType { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public int EllapsedMs { get; set; }
    }
}