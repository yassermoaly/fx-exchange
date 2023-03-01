using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface ISecurityService
    {
        T? LoadSecureObj<T>(string Token, out bool IsExpired);
        string GenerateSecureObj<T>(T Obj, int lifetimeInMinutes);
    }
}
