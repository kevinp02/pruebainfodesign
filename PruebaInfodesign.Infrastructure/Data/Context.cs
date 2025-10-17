using Microsoft.EntityFrameworkCore;
using PruebaInfodesign.Domain.Entities;

namespace PruebaInfodesign.Infrastructure.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options)
        {
        }

        public DbSet<Linea> Lineas { get; set; } = null!;
        public DbSet<TipoCliente> TiposCliente { get; set; } = null!;
        public DbSet<RegistroConsumo> RegistrosConsumo { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Linea>(entity =>
            {
                entity.ToTable("Lineas");
                entity.HasKey(e => e.LineaId);

                entity.Property(e => e.CodigoLinea)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(200);

                entity.HasIndex(e => e.CodigoLinea)
                    .IsUnique()
                    .HasDatabaseName("IX_Lineas_CodigoLinea");

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("GETDATE()");
            });

            modelBuilder.Entity<TipoCliente>(entity =>
            {
                entity.ToTable("TiposCliente");
                entity.HasKey(e => e.TipoClienteId);

                entity.Property(e => e.NombreTipo)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(200);

                entity.HasIndex(e => e.NombreTipo)
                    .IsUnique()
                    .HasDatabaseName("IX_TiposCliente_Nombre");

                entity.HasData(
                    new TipoCliente
                    {
                        TipoClienteId = 1,
                        NombreTipo = "Residencial",
                        Descripcion = "Clientes residenciales o domésticos"
                    },
                    new TipoCliente
                    {
                        TipoClienteId = 2,
                        NombreTipo = "Comercial",
                        Descripcion = "Clientes comerciales y empresariales"
                    },
                    new TipoCliente
                    {
                        TipoClienteId = 3,
                        NombreTipo = "Industrial",
                        Descripcion = "Clientes industriales de alto consumo"
                    }
                );
            });

            modelBuilder.Entity<RegistroConsumo>(entity =>
            {
                entity.ToTable("RegistrosConsumo");
                entity.HasKey(e => e.RegistroId);

                entity.Property(e => e.Fecha)
                    .IsRequired()
                    .HasColumnType("date");

                entity.Property(e => e.ConsumoWh)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)")
                    .HasDefaultValue(0);

                entity.Property(e => e.CostoUnitario)
                    .IsRequired()
                    .HasColumnType("decimal(18,6)")
                    .HasDefaultValue(0);

                entity.Property(e => e.PorcentajePerdida)
                    .IsRequired()
                    .HasColumnType("decimal(5,2)")
                    .HasDefaultValue(0);

                entity.Property(e => e.FechaCreacion)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Linea)
                    .WithMany(l => l.Registros)
                    .HasForeignKey(e => e.LineaId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_RegistroConsumo_Linea");

                entity.HasOne(e => e.TipoCliente)
                    .WithMany(tc => tc.Registros)
                    .HasForeignKey(e => e.TipoClienteId)
                    .OnDelete(DeleteBehavior.Restrict)
                    .HasConstraintName("FK_RegistroConsumo_TipoCliente");

                entity.HasIndex(e => e.Fecha)
                    .HasDatabaseName("IX_RegistroConsumo_Fecha");

                entity.HasIndex(e => new { e.LineaId, e.Fecha })
                    .HasDatabaseName("IX_RegistroConsumo_LineaFecha");

                entity.HasIndex(e => new { e.TipoClienteId, e.Fecha })
                    .HasDatabaseName("IX_RegistroConsumo_TipoClienteFecha");

                entity.HasIndex(e => new { e.Fecha, e.LineaId, e.TipoClienteId })
                    .HasDatabaseName("IX_RegistroConsumo_FechaCompleto");

                entity.HasCheckConstraint("CK_ConsumoPositivo", "[ConsumoWh] >= 0");
                entity.HasCheckConstraint("CK_CostoPositivo", "[CostoUnitario] >= 0");
                entity.HasCheckConstraint("CK_PerdidaValida", "[PorcentajePerdida] >= 0 AND [PorcentajePerdida] <= 100");
            });
        }
    }
}
