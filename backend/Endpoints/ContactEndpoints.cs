using PhonebookApplication.Exceptions;
using PhonebookApplication.Models;
using PhonebookApplication.Repositories;
using PhonebookApplication.Services;

namespace PhonebookApplication.Endpoints;

public static class ContactEndpoints
{
    public static void MapContactEndpoints(this IEndpointRouteBuilder app)
    {
        // GET /api/contacts
        app.MapGet("/api/contacts", async (
            IContactRepository repository,
            int pageNumber = 1,
            int pageSize = 10,
            string? searchTerm = null) =>
        {
            if (pageNumber < 1)
            {
                return Results.BadRequest("Page number must be greater than or equal to 1.");
            }

            if (pageSize < 1)
            {
                return Results.BadRequest("Page size must be greater than or equal to 1.");
            }

            var result = await repository.GetContactsPagedAsync(
                pageNumber,
                pageSize,
                searchTerm
            );

            return Results.Ok(result);
        })
        .RequireAuthorization();

        // GET /api/contacts/export/csv
        app.MapGet("/api/contacts/export/csv", async (
            IContactRepository repository,
            IContactExportService exportService) =>
        {
            List<ContactExportRecord> contacts =
                await repository.GetContactsForExportAsync();

            byte[] file = exportService.CreateCsv(contacts);

            return Results.File(file, "text/csv; charset=utf-8", "contacts.csv");
        })
        .RequireAuthorization();

        // GET /api/contacts/export/json
        app.MapGet("/api/contacts/export/json", async (
            IContactRepository repository,
            IContactExportService exportService) =>
        {
            List<ContactExportRecord> contacts =
                await repository.GetContactsForExportAsync();

            byte[] file = exportService.CreateJson(contacts);

            return Results.File(file, "application/json; charset=utf-8", "contacts.json");
        })
        .RequireAuthorization();

        // POST /api/contacts
        app.MapPost("/api/contacts", async (
            Contact contact,
            IContactRepository repository) =>
        {
            if (!ValidationHelper.TryValidateModel(contact, out var errors))
            {
                return Results.ValidationProblem(errors);
            }

            try
            {
                int newId = await repository.InsertContactAsync(contact);

                var createdContact = await repository.GetContactByIdAsync(newId);

                return Results.Created($"/api/contacts/{newId}", createdContact);
            }
            catch (DuplicatePhoneException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        })
        .RequireAuthorization();

        // GET /api/contacts/{id}
        app.MapGet("/api/contacts/{id}", async (
            int id,
            IContactRepository repository) =>
        {
            var contact = await repository.GetContactByIdAsync(id);

            if (contact == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(contact);
        })
        .RequireAuthorization();

        // PUT /api/contacts/{id}
        app.MapPut("/api/contacts/{id}", async (
            int id,
            Contact contact,
            IContactRepository repository) =>
        {
            if (!ValidationHelper.TryValidateModel(contact, out var errors))
            {
                return Results.ValidationProblem(errors);
            }

            try
            {
                var updated = await repository.UpdateContactAsync(id, contact);

                if (!updated)
                {
                    return Results.NotFound();
                }

                return Results.Ok();
            }
            catch (DuplicatePhoneException ex)
            {
                return Results.BadRequest(ex.Message);
            }
        })
        .RequireAuthorization();

        // DELETE /api/contacts/{id}
        app.MapDelete("/api/contacts/{id}", async (
            int id,
            IContactRepository repository) =>
        {
            var deleted = await repository.DeleteContactAsync(id);

            if (!deleted)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        })
        .RequireAuthorization();
    }
}
