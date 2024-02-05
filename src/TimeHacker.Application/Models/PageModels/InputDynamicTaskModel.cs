﻿using System.ComponentModel.DataAnnotations;

namespace TimeHacker.Application.Models.PageModels
{
    public class InputDynamicTaskModel
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public string Category { get; set; }
        public uint Priority { get; set; }
        public TimeSpan MinTimeToFinish { get; set; }
        public TimeSpan MaxTimeToFinish { get; set; }
        public TimeSpan? OptimalTimeToFinish { get; set; }
    }
}