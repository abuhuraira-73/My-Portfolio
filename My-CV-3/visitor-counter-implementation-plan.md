# Visitor Counter Implementation Plan

This document outlines the implementation plan for adding a visitor counter feature to the ASP.NET portfolio website with MongoDB integration.

## 1. Create a new Visitor model

Create a simple Visitor model class that will store information about each visit:

```csharp
// Models/Visitor.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace My_CV_3.Models
{
    public class Visitor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("PageUrl")]
        public string PageUrl { get; set; } = string.Empty;

        [BsonElement("IpAddress")]
        public string IpAddress { get; set; } = string.Empty;

        [BsonElement("UserAgent")]
        public string UserAgent { get; set; } = string.Empty;

        [BsonElement("VisitedAt")]
        public DateTime VisitedAt { get; set; } = DateTime.UtcNow;
    }
}
```

## 2. Update MongoDB settings in appsettings.json

Add a new collection name for visitors in the existing MongoDB settings:

```json
"MongoDbSettings": {
  "ConnectionString": "",
  "DatabaseName": "my-cv",
  "ContactCollectionName": "contact",
  "VisitorCollectionName": "visitors"
}
```

## 3. Create a service for handling visitor tracking operations

Create a service to handle visitor tracking operations:

```csharp
// Services/VisitorService.cs
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using My_CV_3.Models;
using System.Threading.Tasks;

namespace My_CV_3.Services
{
    public class VisitorService
    {
        private readonly IMongoCollection<Visitor> _visitorCollection;

        public VisitorService(IConfiguration configuration)
        {
            var mongoSettings = configuration.GetSection("MongoDbSettings");
            var client = new MongoClient(mongoSettings["ConnectionString"]);
            var database = client.GetDatabase(mongoSettings["DatabaseName"]);
            _visitorCollection = database.GetCollection<Visitor>(mongoSettings["VisitorCollectionName"]);
        }

        public async Task RecordVisitAsync(Visitor visitor)
        {
            await _visitorCollection.InsertOneAsync(visitor);
        }

        public async Task<long> GetVisitCountAsync()
        {
            return await _visitorCollection.CountDocumentsAsync(FilterDefinition<Visitor>.Empty);
        }
    }
}
```

## 4. Implement middleware to track visits on all page requests

Create middleware to track all page visits:

```csharp
// Middleware/VisitorTrackingMiddleware.cs
using Microsoft.AspNetCore.Http;
using My_CV_3.Models;
using My_CV_3.Services;
using System.Threading.Tasks;

namespace My_CV_3.Middleware
{
    public class VisitorTrackingMiddleware
    {
        private readonly RequestDelegate _next;

        public VisitorTrackingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, VisitorService visitorService)
        {
            // Skip tracking for static files, API calls, etc.
            if (!context.Request.Path.Value.StartsWith("/css") && 
                !context.Request.Path.Value.StartsWith("/js") && 
                !context.Request.Path.Value.StartsWith("/lib") && 
                !context.Request.Path.Value.StartsWith("/images") &&
                !context.Request.Path.Value.Contains("favicon"))
            {
                var visitor = new Visitor
                {
                    PageUrl = context.Request.Path.Value,
                    IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                    UserAgent = context.Request.Headers["User-Agent"].ToString()
                };

                // Record the visit asynchronously (don't wait for it to complete)
                _ = visitorService.RecordVisitAsync(visitor);
            }

            // Call the next middleware in the pipeline
            await _next(context);
        }
    }

    // Extension method for registering the middleware
    public static class VisitorTrackingMiddlewareExtensions
    {
        public static IApplicationBuilder UseVisitorTracking(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<VisitorTrackingMiddleware>();
        }
    }
}
```

## 5. Register the middleware and service in Program.cs

Update Program.cs to register our service and middleware:

```csharp
// Program.cs (additions)
using My_CV_3.Services;
using My_CV_3.Middleware;

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<VisitorService>();

// ... existing code ...

// Add middleware (before UseRouting)
app.UseVisitorTracking();
app.UseRouting();
```

## 6. Test the visitor counter functionality

To test the functionality:
1. Create a simple admin page to view the visitor count
2. Check the MongoDB database directly to verify visits are being recorded
3. Use logging to verify the middleware is executing correctly

## Implementation Notes

This implementation will:
- Track all page visits (excluding static files)
- Store visit information in MongoDB
- Be efficient by not blocking the request pipeline (using fire-and-forget pattern)
- Capture basic information about each visit (URL, IP, user agent, timestamp)