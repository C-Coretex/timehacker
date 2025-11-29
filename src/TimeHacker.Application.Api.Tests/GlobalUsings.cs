global using System.Linq.Expressions;

global using AwesomeAssertions;
global using Moq;
global using MockQueryable;

global using TimeHacker.Helpers.Tests.Mocks;
global using TimeHacker.Helpers.Tests.Mocks.Extensions;
global using TimeHacker.Helpers.Domain.Abstractions.Delegates;
global using TimeHacker.Helpers.Domain.Abstractions.Interfaces;

global using TimeHacker.Domain.Services.Processors;
global using TimeHacker.Domain.Services.Services;
global using TimeHacker.Domain.Entities.Categories;
global using TimeHacker.Domain.Entities.ScheduleSnapshots;
global using TimeHacker.Domain.Entities.Tasks;
global using TimeHacker.Domain.Entities.Tags;
global using TimeHacker.Domain.Entities.Users;
global using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;
global using TimeHacker.Domain.Models.InputModels.ScheduleSnapshots;
global using TimeHacker.Domain.Models.EntityModels.Enums;
global using TimeHacker.Domain.Models.ReturnModels;
global using TimeHacker.Domain.IRepositories.Users;
global using TimeHacker.Domain.IRepositories.Categories;
global using TimeHacker.Domain.IRepositories.ScheduleSnapshots;
global using TimeHacker.Domain.IRepositories.Tags;
global using TimeHacker.Domain.IRepositories.Tasks;
global using TimeHacker.Domain.IRepositories;
global using TimeHacker.Domain.DTOs.RepeatingEntity;
global using TimeHacker.Domain.Helpers.ScheduleSnapshots;

global using TimeHacker.Application.Api.Contracts.DTOs.Categories;
global using TimeHacker.Application.Api.Contracts.IAppServices.Categories;
global using TimeHacker.Application.Api.Contracts.IAppServices.ScheduleSnapshots;
global using TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;
global using TimeHacker.Application.Api.Contracts.DTOs.Tags;
global using TimeHacker.Application.Api.Contracts.IAppServices.Tags;
global using TimeHacker.Application.Api.Contracts.DTOs.Tasks;
global using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;
global using TimeHacker.Application.Api.Contracts.IAppServices.Users;
global using TimeHacker.Application.Api.AppServices.Users;
global using TimeHacker.Application.Api.AppServices.Tasks;
global using TimeHacker.Application.Api.AppServices.Tags;
global using TimeHacker.Application.Api.AppServices.ScheduleSnapshots;
global using TimeHacker.Application.Api.AppServices.Categories;