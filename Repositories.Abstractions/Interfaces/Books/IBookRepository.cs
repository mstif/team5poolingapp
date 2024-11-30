
using ApplicationCore.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces.Books
{
    public interface IBookRepository<T> : IRepository<T> where T : EntityBook
    {
    }
}
