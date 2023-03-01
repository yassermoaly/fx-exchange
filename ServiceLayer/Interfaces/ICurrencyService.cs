using DataAccessLayer.Interfaces;
using Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface ICurrencyService
    {
        Currency GetByISO(string ISO);
        Task Load(ICurrencyRepository CurrencyRepository);
    }
}
