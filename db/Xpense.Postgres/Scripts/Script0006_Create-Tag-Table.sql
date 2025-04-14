-- Create Tag Table
CREATE TABLE IF NOT EXISTS Xpense.Tag (
    id bigserial Primary Key,
    is_deleted boolean,
    created_on timestamp with time zone,
    last_modified timestamp with time zone,
    tag varchar(100) UNIQUE,
    bg_color_hex char(6),
    fg_color_hex char(6)
);