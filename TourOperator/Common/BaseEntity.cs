using System;

namespace TourOperator.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime Created { get; set; }
    }
}
