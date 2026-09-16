namespace GolBet.Entities.Common
{
    // Base común para todas las entidades del dominio: identidad + auditoría + baja lógica.
    public abstract class AuditableEntity
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
