-- Create Merchant Table
CREATE TABLE IF NOT EXISTS Xpense.Merchant (
    id bigserial Primary Key,
    is_deleted boolean,
    created_on timestamp with time zone,
    last_modified timestamp with time zone,
    merchant varchar(100) UNIQUE
);