# TimeHacker Backend - Developer Guide

> **Note:** For frontend documentation, see [TimeHacker.UI/README.md](TimeHacker.UI/README.md)

## Architecture

Clean Architecture with 4 layers:

```
Domain → Application → Infrastructure → API
```

**Projects:**
- `TimeHacker.Domain` - Core entities, interfaces, domain models
- `TimeHacker.Domain.Services` - Business logic & algorithms
- `TimeHacker.Application.Api(.Contracts)` - Service implementations & DTOs
- `TimeHacker.Infrastructure(.Identity)` - EF Core, repositories, auth
- `TimeHacker.Api` - REST controllers
- `TimeHacker.Migrations(.Identity)` - Database migrations
- `TimeHacker.Helpers.*` - Shared utilities

**Request Flow:**
```
HTTP Request → Controller → AppService → Repository/Service/Processor → Response
```

## Core Entities

**Tasks:**
- `FixedTask` - Time-specific tasks (start/end times)
- `DynamicTask` - Flexible tasks (min/max/optimal duration)

**Organization:**
- `Category` & `Tag` - Many-to-many with both task types

**User Scoping:**
- All entities inherit from `UserScopedEntityBase`
- Automatic filtering by `userId` at repository level (multi-tenant)

## Scheduled Entities System

Handles recurring tasks and categories.

**Key Entities:**

1. **ScheduleEntity** - Defines recurring schedule
   - Links to parent `FixedTask` or `Category`
   - Contains `RepeatingEntityDto` (daily/weekly/monthly/yearly patterns)
   - Tracks creation history and optional end date

2. **ScheduleSnapshot** - Parent entity for a specific date
   - Created when "RefreshTasksForDay" is called
   - Contains all scheduled entities for that day
   - Links to `ScheduledTasks` and `ScheduledCategories`

3. **ScheduledTask** / **ScheduledCategory** - Generated instances
   - Created from parent + `ScheduleEntity`
   - Stores snapshot of data for specific occurrence
   - Links back to parent and `ScheduleSnapshot`

**Relationships:**
```
ScheduleEntity (recurring definition)
    └─ Parent: FixedTask or Category

When day is generated:
    ↓
ScheduleSnapshot (for specific date)
    ├─ ScheduledTasks (generated task instances)
    └─ ScheduledCategories (generated category instances)
```

**Generation Flow:**
1. User requests tasks for a date
2. System checks if `ScheduleSnapshot` exists
3. If not, generates it:
   - Fetches regular tasks
   - Finds `ScheduleEntities` matching date
   - Generates `ScheduledTask`/`ScheduledCategory` instances
   - Runs `TaskTimelineProcessor` to optimize schedule
   - Saves everything under `ScheduleSnapshot`
4. Returns results

## Key Components

**Services:**
- `TaskService` - Orchestrates task retrieval & generation
- `ScheduleEntityService` - Manages recurring schedules
- `FixedTaskService` / `DynamicTaskService` - CRUD operations

**Processors:**
- `TaskTimelineProcessor` - Schedules fixed tasks in slots, fills gaps with dynamic tasks

**Database:**
- PostgreSQL with EF Core
- ASP.NET Identity for auth
- Automatic UTC conversion for DateTimes
