#Running integration tests
---

Integration tests are run within a docker-compose environment. This configuration
is stored in the ```docker-compose-integration.yml``` file. The test suite may be run
by running
```docker-compose -f docker-compose-integration.yml up --build --abort-on-container-exit```
from the root directory.

When new migrations are added via EF Core, new SQL scripts must be created through

```dotnet ef migrations script```

The Postgres docker volumes must then be removed to force it to re-initialize the database.
When time allows, we can change this behavior to be less tedious.