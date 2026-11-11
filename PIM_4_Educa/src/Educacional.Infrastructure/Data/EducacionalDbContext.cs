using Educacional.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Educacional.Infrastructure.Data;

/// <summary>
/// Mapeia as tabelas criadas pelo script SQL (database-first).
/// O banco é definido pelo script em /banco; aqui NÃO usamos migrations.
/// </summary>
public class EducacionalDbContext : DbContext
{
    public EducacionalDbContext(DbContextOptions<EducacionalDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Disciplina> Disciplinas => Set<Disciplina>();
    public DbSet<Turma> Turmas => Set<Turma>();
    public DbSet<Matricula> Matriculas => Set<Matricula>();
    public DbSet<Atividade> Atividades => Set<Atividade>();
    public DbSet<Entrega> Entregas => Set<Entrega>();
    public DbSet<Aviso> Avisos => Set<Aviso>();
    public DbSet<Material> Materiais => Set<Material>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Usuario>(e =>
        {
            e.ToTable("Usuario");
            e.Property(x => x.CriadoEm).HasDefaultValueSql("SYSDATETIME()");
        });

        mb.Entity<Disciplina>().ToTable("Disciplina");
        mb.Entity<Turma>().ToTable("Turma");

        mb.Entity<Matricula>(e =>
        {
            e.ToTable("Matricula", t => t.HasTrigger("trg_Matricula_ValidaAluno"));
            e.Property(x => x.DataMatricula).HasDefaultValueSql("SYSDATETIME()");
        });

        mb.Entity<Atividade>().ToTable("Atividade");

        mb.Entity<Entrega>(e =>
        {
            e.ToTable("Entrega", t =>
            {
                t.HasTrigger("trg_Entrega_ValidaPrazo");
                t.HasTrigger("trg_Entrega_AuditaNota");
            });
            e.Property(x => x.Nota).HasPrecision(4, 2);
            e.Property(x => x.DataEnvio).HasDefaultValueSql("SYSDATETIME()");
        });

        mb.Entity<Aviso>(e =>
        {
            e.ToTable("Aviso");
            e.Property(x => x.DataPublicacao).HasDefaultValueSql("SYSDATETIME()");
        });

        mb.Entity<Material>().ToTable("Material");
    }
}
