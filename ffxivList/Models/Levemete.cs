﻿using System.ComponentModel.DataAnnotations;

namespace ffxivList.Models
{
    public class Levemete
    {
        public int LevemeteId { get; set; }
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters.")]
        public string LevemeteName { get; set; }
        [Range(1,60, ErrorMessage = "Level must be within 1 and 60.")]
        public int LevemeteLevel { get; set; }
    }
}
