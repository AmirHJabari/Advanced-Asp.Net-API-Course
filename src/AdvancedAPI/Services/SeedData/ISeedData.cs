using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.DI;

namespace Services.SeedData
{
    public interface ISeedData : IScopedDependency
    {
        void InitializeData();
    }
}
