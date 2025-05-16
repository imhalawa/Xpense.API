with ts as (select now() as current_ts)
insert into Xpense.priority (is_deleted, created_on, last_modified, priority, weight)
select v.is_deleted, ts.current_ts, ts.current_ts, v.priority, v.weight from ts, (values(false,'Extreme', 1)) as v(is_deleted, priority, weight);

with ts as (select now() as current_ts)
insert into Xpense.priority (is_deleted, created_on, last_modified, priority, weight)
select v.is_deleted, ts.current_ts, ts.current_ts, v.priority, v.weight from ts, (values(false,'High', 2)) as v(is_deleted, priority, weight);

with ts as (select now() as current_ts)
insert into Xpense.priority (is_deleted, created_on, last_modified, priority, weight)
select v.is_deleted, ts.current_ts, ts.current_ts, v.priority, v.weight from ts, (values(false,'Medium', 3)) as v(is_deleted, priority, weight);

with ts as (select now() as current_ts)
insert into Xpense.priority (is_deleted, created_on, last_modified, priority, weight)
select v.is_deleted, ts.current_ts, ts.current_ts, v.priority, v.weight from ts, (values(false,'Low', 4)) as v(is_deleted, priority, weight);

with ts as (select now() as current_ts)
insert into Xpense.priority (is_deleted, created_on, last_modified, priority, weight)
select v.is_deleted, ts.current_ts, ts.current_ts, v.priority, v.weight from ts, (values(false,'Trivial', 5)) as v(is_deleted, priority, weight);