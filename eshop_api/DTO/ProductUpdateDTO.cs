﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eshop_api.DTO
{
    public class ProductUpdateDTO : ProductCreateDTO
    {
        [Required]
        public int Id { get; set; }
    }
}
