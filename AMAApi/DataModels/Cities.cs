﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmsApi.DataModels
{
    public class Cities
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
       

    }
}