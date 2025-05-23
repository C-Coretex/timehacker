﻿using System.Drawing;

namespace TimeHacker.Application.Models.Return.Categories
{
    public class CategoryReturnModel
    {
        public Guid Id { get; init; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public Color Color { get; set; }
    }
}
