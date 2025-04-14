-- Create Category Table
CREATE TABLE IF NOT EXISTS Xpense.Category (
    id bigserial Primary Key,
    is_deleted boolean,
    created_on timestamp with time zone,
    last_modified timestamp with time zone,
    category varchar(100) UNIQUE,
    priority_id bigint References Xpense.Priority (id) -- fk for Priority table
);