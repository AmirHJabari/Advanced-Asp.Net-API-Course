﻿using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SiteSettings
    {
        public string ElamahPath { get; set; }

        public JwtSettings Jwt { get; set; }
    }
}
