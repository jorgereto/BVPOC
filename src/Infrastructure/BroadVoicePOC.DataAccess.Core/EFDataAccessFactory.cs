using BroadVoicePOC.DataAccess.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BroadVoicePOC.DataAccess.Core
{
    public class EFDataAccessFactory : IDataAccessFactory
    {
        public BroadVoicePOCContext GetBroadVoicePOCContext()
        {
            return new BroadVoicePOCContext();
        }

        public BroadVoicePOCContext GetBroadVoicePOCContext(string connectionString)
        {
            DbContextOptions<BroadVoicePOCContext> options =
                SqlServerDbContextOptionsExtensions.UseSqlServer(new DbContextOptionsBuilder<BroadVoicePOCContext>(), connectionString).Options;
            return new BroadVoicePOCContext(options);
        }
    }
}
