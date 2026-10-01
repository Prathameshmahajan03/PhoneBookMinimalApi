using PhonebookApplication.Models;
using PhonebookApplication.Services;

namespace PhonebookApplication.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
            LoginRequest request,
            IAuthService authService) =>
        {
            if (!ValidationHelper.TryValidateModel(request, out var errors))
            {
                return Results.ValidationProblem(errors);
            }

            LoginResponse? response = await authService.LoginAsync(request);

            if (response == null)
            {
                return Results.Unauthorized();
            }

            return Results.Ok(response);
        })
        .AllowAnonymous();
    }
}
