-- Create Account Table
CREATE TABLE IF NOT EXISTS Xpense.Account (
    id bigserial Primary Key,
    is_deleted boolean,
    created_on timestamp with time zone,
    last_modified timestamp with time zone,
    account varchar(100),
    account_number char(10) unique,
    balance bigint
);