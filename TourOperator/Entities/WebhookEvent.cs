﻿using TourOperator.Common;

namespace TourOperator.Entities
{
    public class WebhookEvent : BaseEntity
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public WebhookEvent()
        {

        }       
    }
}