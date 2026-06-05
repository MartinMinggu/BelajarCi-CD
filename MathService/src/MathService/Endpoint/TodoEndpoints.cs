using MathService.Models;

namespace MathService.Endpoints;

public static class TodoEndpoints
{
    private static readonly List<TodoItem> _todos = new();
    private static int _nextId = 1;

    public static void MapTodoEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/todos");

        group.MapGet("/", () => Results.Ok(_todos));

        group.MapGet("/{id}", (int id) => {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            return todo is null ? Results.NotFound() : Results.Ok(todo);
        });

        group.MapPost("/", (TodoItem item) => {
            item.Id = _nextId++;
            item.CreatedAt = DateTime.UtcNow;
            _todos.Add(item);
            return Results.Created($"/todos/{item.Id}", item);
        });

        group.MapPut("/{id}", (int id, TodoItem updated) => {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo is null) return Results.NotFound();
            todo.Title = updated.Title;
            todo.IsCompleted = updated.IsCompleted;
            return Results.Ok(todo);
        });

        group.MapDelete("/{id}", (int id) => {
            var todo = _todos.FirstOrDefault(t => t.Id == id);
            if (todo is null) return Results.NotFound();
            _todos.Remove(todo);
            return Results.NoContent();
        });
    }
}