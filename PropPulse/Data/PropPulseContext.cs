﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PropPulse.Models;

namespace PropPulse.Data
{
    public class PropPulseContext : DbContext
    {
        public PropPulseContext (DbContextOptions<PropPulseContext> options)
            : base(options)
        {
        }

        public DbSet<PropPulse.Models.User> User { get; set; } = default!;
    }
}
