﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace uniformesV51.Model
{
    [Index(nameof(OrgId), IsUnique = false)]
    public class Z190_Bitacora
    {
        [Key]
        public string BitacoraId { get; set; } = Guid.NewGuid().ToString();
        public DateTime Fecha { get; set; } = DateTime.Now;
        public string UserId { get; set; } = "";
        public string Desc { get; set; } = "";
        public bool Sistema { get; set; } = false;
        public string OrgId { get; set; } = "";

    }
}
