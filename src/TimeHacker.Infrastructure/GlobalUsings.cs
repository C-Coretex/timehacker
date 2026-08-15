global using System.Text.Json;
global using System.Text.Json.Serialization.Metadata;

global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;

global using TimeHacker.Helpers.Db.Abstractions.BaseClasses;
global using TimeHacker.Helpers.Domain.Abstractions.Interfaces.DbEntity;

global using TimeHacker.Domain.IModels;
global using TimeHacker.Domain.Entities.EntityBase;
global using TimeHacker.Domain.Entities.Categories;
global using TimeHacker.Domain.Entities.ScheduleSnapshots;
global using TimeHacker.Domain.Entities.Tags;
global using TimeHacker.Domain.Entities.Tasks;
global using TimeHacker.Domain.Entities.Users;
global using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
global using TimeHacker.Domain.IRepositories.Tasks;
