using ScheduleUpdater;
using ScheduleUpdater.DAL.Context;
using ScheduleUpdater.Services;
using Microsoft.EntityFrameworkCore;
using ScheduleUpdater.DAL.Repository;
using ScheduleUpdater.Abstract;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddSingleton<IFileReaderService, FileReaderService>();
builder.Services.AddSingleton<ITaskManagerService, TaskManagerService>();
builder.Services.AddSingleton<IUpdaterRepository, UpdaterRepository>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<UpdaterDbContext>(options =>
    options.UseSqlServer(connectionString));

var host = builder.Build();
host.Run();
