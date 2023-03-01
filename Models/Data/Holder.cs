using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Data
{
    public record Holder
    {
        public Holder()
        {
            Accounts=new HashSet<Account>();
        }
        public long Id { get; set; }
        public string FirstName { get; set; } =  default!;
        public string LastName { get; set; } =  default!;
        public string PassportId { get; set; } = default!;
        public string Address { get; set; } = default!;

        public virtual ICollection<Account> Accounts { get; set; }
    }
}
