﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Models
{
    public class ComplexGenericModel<T>
    {
        public T GenericProperty { get; set; }
    }
}
