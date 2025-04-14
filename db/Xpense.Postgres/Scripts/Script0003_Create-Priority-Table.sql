-- Create Priority Table
CREATE TABLE IF NOT EXISTS Xpense.Priority (
    id bigserial Primary Key,
    is_deleted boolean,
    created_on timestamp with time zone,
    last_modified timestamp with time zone,
    priority varchar(100) unique,
    weight float4
);