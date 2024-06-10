// Copyright (c) Uwe Gradenegger <info@gradenegger.eu>

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Reflection;
using Microsoft.AspNetCore.Authentication.Negotiate;

var builder = WebApplication.CreateBuilder();
var appName = Assembly.GetExecutingAssembly().GetName().Name;

builder.Logging.AddEventLog(settings =>
{
    settings.SourceName = appName;
});

// CoreWCF doesn't pass exceptions to logging by default as SOAP failures are also thrown as exceptions
builder.Services.AddSingleton<IServiceBehavior, UnhandledExceptionLoggingBehavior>();

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme).AddNegotiate();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

builder.Services.AddSingleton<Policy>();
builder.Services.AddSingleton<SecurityTokenService>();

var app = builder.Build();

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<Policy>();
    serviceBuilder.AddServiceEndpoint<Policy, IPolicy>(new WSHttpBinding(SecurityMode.Transport), "/Service.svc/CEP");
    serviceBuilder.AddService<SecurityTokenService>();
    serviceBuilder.AddServiceEndpoint<SecurityTokenService, ISecurityTokenService>(new WSHttpBinding(SecurityMode.Transport), "/Service.svc/CES");
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpsGetEnabled = true;
});

app.UseHttpsRedirection();
app.UseHsts();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
