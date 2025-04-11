CREATE TABLE IF NOT EXISTS Xpense.Tbl_Accounts (
    id bigserial,
    is_deleted boolean,
    created_on timestamp with time zone,
    last_modified timestamp with time zone,
    name character varying(250),
    account_number character (10),
    balance bigint
)

