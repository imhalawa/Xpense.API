-- Create Currency Lookup Table
CREATE TABLE IF NOT EXISTS Xpense.Currency (
    id bigserial Primary Key,
    is_deleted boolean,
    created_on timestamp with time zone,
    last_modified timestamp with time zone,
    currency_code char(3) UNIQUE,
    currency varchar(100) UNIQUE
);