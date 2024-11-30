
using ApplicationCore.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces.Documents
{
    public interface IDocumentRepository<T> : IRepository<T> where T : EntityDocument
    {

        T? GetDocumentByNumber(string number);


    }
}
