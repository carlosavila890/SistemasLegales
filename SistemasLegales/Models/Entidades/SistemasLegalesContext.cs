﻿using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SistemasLegales.Models.Entidades
{
    public partial class SistemasLegalesContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<Actor> Actor { get; set; }
        public virtual DbSet<AdminRequisitoLegal> AdminRequisitoLegal { get; set; }
        public virtual DbSet<Ciudad> Ciudad { get; set; }
        public virtual DbSet<Documento> Documento { get; set; }
        public virtual DbSet<OrganismoControl> OrganismoControl { get; set; }
        public virtual DbSet<Proceso> Proceso { get; set; }
        public virtual DbSet<RequisitoLegal> RequisitoLegal { get; set; }
        public virtual DbSet<Status> Status { get; set; }

        public SistemasLegalesContext(DbContextOptions<SistemasLegalesContext> options) 
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Actor>(entity =>
            {
                entity.HasKey(e => e.IdActor)
                    .HasName("PK_Actor");

                entity.Property(e => e.Departamento)
                    .IsRequired()
                    .HasColumnType("varchar(200)");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnType("varchar(200)");

                entity.Property(e => e.Nombres)
                    .IsRequired()
                    .HasColumnType("varchar(200)");
            });

            modelBuilder.Entity<AdminRequisitoLegal>(entity =>
            {
                entity.HasKey(e => e.IdAdminRequisitoLegal)
                    .HasName("PK_AdminRequisitoLegal");

                entity.Property(e => e.EmailNotificacion1)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.EmailNotificacion2)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Observaciones).HasColumnType("varchar(500)");

                entity.HasOne(d => d.ActorCustodioDocumento)
                    .WithMany(p => p.AdminRequisitoLegalIdActorCustodioDocumento)
                    .HasForeignKey(d => d.IdActorCustodioDocumento)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_AdminRequisitoLegal_Actor2");

                entity.HasOne(d => d.ActorDuennoProceso)
                    .WithMany(p => p.AdminRequisitoLegalIdActorDuennoProceso)
                    .HasForeignKey(d => d.IdActorDuennoProceso)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_AdminRequisitoLegal_Actor");

                entity.HasOne(d => d.ActorResponsableGestSeg)
                    .WithMany(p => p.AdminRequisitoLegalIdActorResponsableGestSeg)
                    .HasForeignKey(d => d.IdActorResponsableGestSeg)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_AdminRequisitoLegal_Actor1");

                entity.HasOne(d => d.Ciudad)
                    .WithMany(p => p.AdminRequisitoLegal)
                    .HasForeignKey(d => d.IdCiudad)
                    .HasConstraintName("FK_AdminRequisitoLegal_Ciudad");

                entity.HasOne(d => d.Documento)
                    .WithMany(p => p.AdminRequisitoLegal)
                    .HasForeignKey(d => d.IdDocumento)
                    .HasConstraintName("FK_AdminRequisitoLegal_Documento");

                entity.HasOne(d => d.OrganismoControl)
                    .WithMany(p => p.AdminRequisitoLegal)
                    .HasForeignKey(d => d.IdOrganismoControl)
                    .HasConstraintName("FK_AdminRequisitoLegal_OrganismoControl");

                entity.HasOne(d => d.Proceso)
                    .WithMany(p => p.AdminRequisitoLegal)
                    .HasForeignKey(d => d.IdProceso)
                    .HasConstraintName("FK_AdminRequisitoLegal_Proceso");

                entity.HasOne(d => d.RequisitoLegal)
                    .WithMany(p => p.AdminRequisitoLegal)
                    .HasForeignKey(d => d.IdRequisitoLegal)
                    .HasConstraintName("FK_AdminRequisitoLegal_RequisitoLegal");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.AdminRequisitoLegal)
                    .HasForeignKey(d => d.IdStatus)
                    .HasConstraintName("FK_AdminRequisitoLegal_Status");
            });

            modelBuilder.Entity<Ciudad>(entity =>
            {
                entity.HasKey(e => e.IdCiudad)
                    .HasName("PK_Ciudad");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnType("varchar(200)");
            });

            modelBuilder.Entity<Documento>(entity =>
            {
                entity.HasKey(e => e.IdDocumento)
                    .HasName("PK_Documento");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnType("varchar(200)");
            });

            modelBuilder.Entity<OrganismoControl>(entity =>
            {
                entity.HasKey(e => e.IdOrganismoControl)
                    .HasName("PK_OrganismoControl");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnType("varchar(200)");
            });

            modelBuilder.Entity<Proceso>(entity =>
            {
                entity.HasKey(e => e.IdProceso)
                    .HasName("PK_Proceso");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnType("varchar(200)");
            });

            modelBuilder.Entity<RequisitoLegal>(entity =>
            {
                entity.HasKey(e => e.IdRequisitoLegal)
                    .HasName("PK_RequisitoLegal");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnType("varchar(200)");
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.HasKey(e => e.IdStatus)
                    .HasName("PK_Status");

                entity.Property(e => e.Color)
                    .IsRequired()
                    .HasColumnType("varchar(100)");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasColumnType("varchar(100)");
            });
        }
    }
}