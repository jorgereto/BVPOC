using BroadVoicePOC.Model.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BroadVoicePOC.Business.Interfaces
{
    public interface ISalespersonService : IDisposable
    {
        SalespersonDTO GetSalesperson(int id);
    }
}
