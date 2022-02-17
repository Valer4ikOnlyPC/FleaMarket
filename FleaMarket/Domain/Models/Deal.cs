﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Deal
    {
        public Guid DealId { get; set; }
        public Guid UserMaster { get; set; }
        public Guid ProductMaster { get; set; }
        public Guid UserRecipient { get; set; }
        public Guid ProductRecipient { get; set; }
        public enumIsActive IsActive { get; set; }

        public enum enumIsActive : int
        {
            Сonsideration,
            Accepted,
            Сompleted,
            Terminated
        }
    }
}
