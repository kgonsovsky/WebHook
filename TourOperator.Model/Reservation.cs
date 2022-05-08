using System;

namespace TourOperator.Model;

public class Reservation : BaseEntity
{
    public string Guest { get; set; }
        
    public DateTime? CheckIn { get; set; }

    public DateTime? CheckOut { get; set; }

    public string Agency { get; set; }

    public bool Handled { get; set; }

}