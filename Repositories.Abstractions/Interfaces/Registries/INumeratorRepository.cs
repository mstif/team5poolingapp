using ApplicationCore.Enums;
using ApplicationCore.Registries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces.Registries
{
    public interface INumeratorRepository
    {
        public string GetNextStringNumber(ObjectTypes objectType, long CustomerID, DateTime dateIssue);

        public long GetNextLongNumber(ObjectTypes objectType, long CustomerID, DateTime dateIssue);

        IEnumerable<Numerator> GetList();

    }
}
