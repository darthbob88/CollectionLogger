﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace LogCollections
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class mediaEntities : DbContext
    {
        public mediaEntities()
            : base("name=mediaEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<porn> porn { get; set; }
        public virtual DbSet<tv_shows> tv_shows { get; set; }
        public virtual DbSet<artist> artist { get; set; }
        public virtual DbSet<comic_series> comic_series { get; set; }
        public virtual DbSet<album> album { get; set; }
        public virtual DbSet<movies> movies { get; set; }
        public virtual DbSet<tv_episodes> tv_episodes { get; set; }
    }
}
