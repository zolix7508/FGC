﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GameCenterCore.Implementation.Persistance
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    internal partial class MainPersistanceEntities : DbContext
    {
        public MainPersistanceEntities()
            : base("name=MainPersistanceEntities")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        internal DbSet<UserDb> UserDbs { get; set; }
        internal DbSet<webpages_Membership> webpages_Membership { get; set; }
        internal DbSet<webpages_OAuthMembership> webpages_OAuthMembership { get; set; }
        internal DbSet<webpages_Roles> webpages_Roles { get; set; }
        public DbSet<GameDb> GameDbs { get; set; }
        public DbSet<PartyDb> PartyDbs { get; set; }
        public DbSet<PlayerDb> PlayerDbs { get; set; }
    }
}
