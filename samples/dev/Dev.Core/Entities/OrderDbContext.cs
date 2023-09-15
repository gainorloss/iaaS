﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Diagnostics.CodeAnalysis;

namespace Dev.ConsoleApp.Entities
{
    public class OrderDbContext
        : DbContextBase<OrderDbContext>
    {
        public OrderDbContext([NotNull] DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        protected override void OnSavingChanges(EntityEntry entry)
        {

        }
    }
}
