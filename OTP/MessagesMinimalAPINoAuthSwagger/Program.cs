using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.SwaggerUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

string jsonFilePath = "messages.json";
string jsonContent = File.ReadAllText(jsonFilePath);
var messages = JsonConvert.DeserializeObject<List<Message>>(jsonContent);

// Add the messages list to the DI container as a singleton
builder.Services.AddSingleton(messages);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(swaggerUIOptions =>
{
    swaggerUIOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "Messages API V1");
    swaggerUIOptions.DocExpansion(DocExpansion.List);
});

app.MapGet("/api/messages", (List<Message> messages, string keyword) =>
{
    try
    {
        if (string.IsNullOrEmpty(keyword))
            return Results.BadRequest("The keyword cannot be empty.");

        var keywordLowercase = keyword.ToLower();
        var matchingMessages = messages.Where(msg => msg.content?.ToLower().Contains(keywordLowercase) ?? false);
        return Results.Ok(matchingMessages);
    }
    catch (Exception ex)
    {
        // Log the exception if necessary
        Console.WriteLine(ex.Message);
        return Results.Problem("An error occurred while processing your request.", null, 500);
    }
});

app.MapPost("/api/messages", (List<Message> messages, Message message) =>
{
    if (message == null)
        return Results.BadRequest("Invalid message.");

    messages.Add(message);

    // The messages.json file could be updated here.

    return Results.Created($"/api/messages?id={message.id}", message);
});

app.Run();

public class Message
{
    public string? id { get; set; }
    public string? parentId { get; set; }
    public string? userId { get; set; }
    public string? ts { get; set; }
    public string? content { get; set; }
}
