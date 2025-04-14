-- Create transaction_type enum
CREATE TYPE TE_TransactionType as ENUM ('credit', 'debit', 'transfer');

-- Create Transactions Table
CREATE TABLE IF NOT EXISTS Xpense.Transaction (
    id bigserial Primary Key,
    is_deleted boolean,
    created_on timestamp with time zone,
    last_modified timestamp with time zone,
    amount money UNIQUE,
    transaction_type TE_TransactionType,
    currency_id bigint References Xpense.Currency (id),
    account_id bigint References Xpense.Account(id),
    merchant_id bigint References Xpense.Merchant(id),
    category_id bigint References Xpense.Category(id)
);