using System;
using TourOperator.Common;

namespace TourOperator.Entities
{
    public class Reservation : BaseEntity
    {
        public string Guest { get; set; }
        
        public DateTime? CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        public string Agency { get; set; }

        public string MealPlan { get; set; }

    }
}
