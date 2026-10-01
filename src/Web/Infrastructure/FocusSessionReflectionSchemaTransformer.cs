using FocusPocuss.Domain.Enums;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace FocusPocuss.Web.Infrastructure;

internal sealed class FocusSessionReflectionSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        var schemaType = Nullable.GetUnderlyingType(context.JsonTypeInfo.Type)
            ?? context.JsonTypeInfo.Type;

        if ((schemaType == typeof(FocusSessionReflection) || schemaType == typeof(RecoveryChoice)) && schema.Enum is not null)
        {
            for (var index = schema.Enum.Count - 1; index >= 0; index--)
            {
                if (schema.Enum[index] is null)
                {
                    schema.Enum.RemoveAt(index);
                }
            }
        }

        return Task.CompletedTask;
    }
}
