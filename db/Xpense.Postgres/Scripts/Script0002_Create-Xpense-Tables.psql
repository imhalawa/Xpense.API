-- Create Account Table
CREATE TABLE IF NOT EXISTS Xpense.Account (
    id bigserial Primary Key,
    is_deleted boolean,
    created_on timestamp with time zone,
    last_modified timestamp with time zone,
    account character varying(100),
    account_number character (10) unique,
    balance bigint
);

-- Create Priority Table
CREATE TABLE IF NOT EXISTS Xpense.Priority (
    id bigserial Primary Key,
    is_deleted boolean,
    created_on timestamp with time zone,
    last_modified timestamp with time zone,
    priority character varying (100) UNIQUE,
    Weight float4,
);

-- Create Category Table
CREATE TABLE IF NOT EXISTS Xpense.Category (
    id bigserial Primary Key,
    is_deleted boolean,
    created_on timestamp with time zone,
    last_modified timestamp with time zone,
    category character varying (100) UNIQUE,
    priority_id bigint References Priority (id) -- fk for Priority table
);

-- Create Merchant Table
CREATE TABLE IF NOT EXISTS Xpense.Merchant (
    id bigserial Primary Key,
    is_deleted boolean,
    created_on timestamp with time zone,
    last_modified timestamp with time zone,
    merchant character varying (100) UNIQUE
);

-- Create Tag Table
CREATE TABLE IF NOT EXISTS Xpense.Tag (
    id bigserial Primary Key,
    is_deleted boolean,
    created_on timestamp with time zone,
    last_modified timestamp with time zone,
    tag character varying (100) UNIQUE,
    bg_color_hex character (6),
    fg_color_hex character (6)
);

-- Create Currency Lookup Table
CREATE TABLE IF NOT EXISTS Xpense.Currency (
    id bigserial Primary Key,
    is_deleted boolean,
    created_on timestamp with time zone,
    last_modified timestamp with time zone,
    currency_code character (3) UNIQUE,
    currency character varying (100) UNIQUE,
);

-- Create TransactionType Table
CREATE TABLE IF NOT EXISTS Xpense.TransactionType(
    id bigserial Primary Key,
    is_deleted boolean,
    created_on timestamp with time zone,
    last_modified timestamp with time zone,
    transaction_type character varying (50)
);

-- Create Transactions Table
CREATE TABLE IF NOT EXISTS Xpense.Transaction (
    id bigserial Primary Key,
    is_deleted boolean,
    created_on timestamp with time zone,
    last_modified timestamp with time zone,
    amount money UNIQUE,
    currency_id bigint References Currency (id),
    transaction_type_id bigint References TransactionType (id),
    account_id bigint References Account(id),
    merchant_id bigint References Merchant(id),
    category_id bigint References Category(id)
);

-- Create TransactionTags Table
CREATE TABLE IF NOT EXISTS Xpense.TransactionTag (
    id bigserial Primary Key,
    is_deleted boolean,
    created_on timestamp with time zone,
    last_modified timestamp with time zone,
    transaction_id bigint References Transaction (id),
    tag_id bigint References Tag(id)
);