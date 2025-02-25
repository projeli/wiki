using WikiService.Domain.Exceptions;

namespace WikiService.Api.Exceptions;

public class MissingEnvironmentVariableException(string missingEnvironmentVariable)
    : WikiServiceException("Missing environment variable: " + missingEnvironmentVariable);