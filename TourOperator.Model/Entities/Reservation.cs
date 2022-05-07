using TourOperator.Model.Common;

namespace TourOperator.Model.Entities
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
