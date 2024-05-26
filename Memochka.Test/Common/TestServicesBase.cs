using Memochka.Models.MemochkaDbContext;
using Microsoft.Build.Evaluation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Memochka.Test.Common
{
    public abstract class TestServicesBase : IDisposable
    {
        protected readonly MemochkaContext _context;

        public TestServicesBase()
        {
            _context = MemochkaContextFactory.Create();
        }

        public void Dispose() 
        {
            MemochkaContextFactory.Destroy(_context);
        }
    }
}
