-- Create Xpense Service User
CREATE  USER SRV_Xpense with PASSWORD 'password';

-- Create Xpense Database
Create Database If Not Exists XpenseDb WITH 
    OWNER = SRV_Xpense
    ENCODING = 'UTF8'
    LC_COLLATE = 'en_US.UTF-8'
    LC_CTYPE = 'en_US.UTF-8'
    TABLESPACE = pg_default
    STRATEGY = WAL_LOG;

-- GRANT Xpense Service USER FULL Permission
GRANT ALL PRIVILEGES ON DATABASE XpenseDb TO SRV_Xpense;

-- Create Default Schema
Create SCHEMA DEVXPENSE AUTHORIZATION SRV_Xpense;