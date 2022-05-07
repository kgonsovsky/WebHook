using System.ComponentModel.DataAnnotations;

namespace TourOperator.Model.Common
{
    public abstract class BaseEntity
    {
        [Key]
        public Guid? Id { get; set; }
        public DateTime Created { get; set; }
    }
}
