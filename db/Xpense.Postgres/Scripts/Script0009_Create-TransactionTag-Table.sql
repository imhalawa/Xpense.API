-- Create TransactionTags Table
CREATE TABLE IF NOT EXISTS Xpense.TransactionTag (
    id bigserial Primary Key,
    is_deleted boolean,
    created_on timestamp with time zone,
    last_modified timestamp with time zone,
    transaction_id bigint References Xpense.Transaction (id),
    tag_id bigint References Xpense.Tag (id)
);