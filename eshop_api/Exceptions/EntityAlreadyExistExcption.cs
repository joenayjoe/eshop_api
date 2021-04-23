using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshop_api.Exceptions
{
    public class EntityAlreadyExistExcption : Exception
    {
        public EntityAlreadyExistExcption(string message) : base(message)
        {
        }
    }
}
