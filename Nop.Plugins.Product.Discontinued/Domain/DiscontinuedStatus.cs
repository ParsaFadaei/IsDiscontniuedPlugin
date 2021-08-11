using System;
using System.Collections.Generic;
using System.Text;
using Nop.Core;

namespace Nop.Plugin.Product.Discontinued.Domain
{
    public class DiscontinuedStatus:BaseEntity
    {
        public bool DiscontinuedState { get; set; }
        public virtual int ProductId { get; set; }
    }
}
