using BroadVoicePOC.DataAccess.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BroadVoicePOC.DataAccess.Core
{
    public interface IDataAccessFactory
    {
        BroadVoicePOCContext GetBroadVoicePOCContext();

        BroadVoicePOCContext GetBroadVoicePOCContext(string connectionString);
    }
}
