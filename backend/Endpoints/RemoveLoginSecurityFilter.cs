using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace PhonebookApplication.Endpoints;

/// <summary>
/// Removes the global Bearer security requirement from the login endpoint
/// in the generated OpenAPI document. The document-level security requirement
/// remains; this filter sets an empty operation-level security list on the
/// login path, which overrides the document-level requirement for that
/// specific operation.
/// </summary>
public class RemoveLoginSecurityFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (var path in swaggerDoc.Paths)
        {
            if (path.Key.StartsWith("/api/auth/login", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var operation in path.Value.Operations)
                {
                    operation.Value.Security.Clear();
                }
            }
        }
    }
}
