﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ffxivList.Models
{
    public class UserLevemete
    {
        public int UserLevemeteID { get; set; }
        public int LevemeteID { get; set; }
        public string UserID { get; set; }
        public bool IsComplete { get; set; }
    }
}
