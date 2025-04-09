# Migrating from EF Core to Dapper with PostgreSQL

## Analysis

There are few challenges that i can think of In order to migrate from Ef core to Dapper:

- **Handling Migrations with SQL**: This will require a handcrafted migration script to execute actual SQL in a transactional manner. (under research though).
- **SQL rather than LINQ**: Utilize pure SQL instead of LINQ Expressions utilized for EF core.
- 
