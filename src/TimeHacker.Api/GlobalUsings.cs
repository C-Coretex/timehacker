global using Microsoft.AspNetCore.Authorization;
global using Microsoft.AspNetCore.Http.HttpResults;
global using Microsoft.AspNetCore.Mvc;
global using Microsoft.EntityFrameworkCore;

global using System.Globalization;
global using System.Drawing;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.ComponentModel.DataAnnotations;

global using TimeHacker.Domain.IModels;
global using TimeHacker.Domain.Observability;
global using TimeHacker.Domain.DTOs.RepeatingEntity;
global using TimeHacker.Domain.Models.ReturnModels;
global using TimeHacker.Domain.Models.EntityModels.Enums;
global using TimeHacker.Domain.Models.EntityModels.RepeatingEntityTypes;
global using TimeHacker.Domain.Models.InputModels.ScheduleSnapshots;
global using TimeHacker.Domain.BusinessLogicExceptions;

global using TimeHacker.Application.Api.Contracts.DTOs.Tasks;
global using TimeHacker.Application.Api.Contracts.DTOs.ScheduleSnapshots;
global using TimeHacker.Application.Api.Contracts.IAppServices.Tasks;

global using TimeHacker.Api.Helpers;
global using TimeHacker.Api.Models.Input.Tasks;
global using TimeHacker.Api.Models.Return.ScheduleSnapshots;
