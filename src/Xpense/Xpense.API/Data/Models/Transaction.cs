﻿using System.Collections.Generic;
using Xpense.API.Enums;

namespace Xpense.API.Data.Models
{
    public class Transaction : BaseEntity
    {
        public decimal Amount { get; set; }
        public string? Reason { get; set; }
        public TransactionType TransactionType { get; set; }

        public int FromAccountId { get; set; }
        public Account FromAccount { get; set; }

        public int ToAccountId { get; set; }
        public Account ToAccount { get; set; }

        public int CurrencyExchangeRateAuditId { get; set; }
        public CurrencyExchangeRateAudit CurrencyExchangeRateAudit { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
    }
}
