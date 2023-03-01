namespace Models.Data
{
    public record Currency
    {
        public Currency()
        {
            Accounts = new HashSet<Account>();
        }
        public short Id { get; set; }
        public string Name { get; set; } = default!;
        public string ISOCode { get; set; } = default!;


        public virtual ICollection<Account> Accounts { get; set; }
    }
}
