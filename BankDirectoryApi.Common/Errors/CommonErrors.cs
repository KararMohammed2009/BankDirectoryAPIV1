using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Common.Errors
{
    public enum CommonErrors
    {
    
       

        // General Validation Errors (Applicable across layers)
        InvalidInput = 1000,
        MissingRequiredField = 1001,
        InvalidFormat = 1002,
        OutOfRange = 1003,
        NotUnique = 1004,



        // Resource Access Errors (Data Access Layer & potentially others)
        ResourceNotFound = 2000,
        ResourceAlreadyExists = 2001,
        DatabaseConnectionFailed = 2002,
        DatabaseQueryFailed = 2003,
        FileAccessDenied = 2004,
        FileNotFound = 2005,
        ExternalServiceUnavailable = 2006,
        ExternalServiceTimeout = 2007,
        ExternalServiceError = 2008,

        // Authorization & Authentication Errors (Security Layer)
        UnauthorizedAccess = 3000,
        AuthenticationFailed = 3001,
        InvalidToken = 3002,
        ExpiredToken = 3003,
        Forbidden = 3004,

        // Business Logic Errors (Business Logic Layer)
        BusinessRuleViolation = 4000,
        OperationNotAllowed = 4001,

        // System Errors (General/Infrastructure)
        InternalServerError = 5000,
        NotImplemented = 5001,
        ServiceUnavailable = 5002,
        Timeout = 5003,
        ConfigurationError = 5004,
        UnexpectedError = 5005,
        OperationFailed = 5006,


        //Concurrency Errors
        ConcurrencyFailure = 6000,
        LockAcquisitionFailure = 6001,

        //Communication Errors
        NetworkError = 7000,
        SerializationError = 7001,
        DeserializationError = 7002,

        // Integration errors
        IntegrationError = 8000,
        IntegrationServiceUnavailable = 8001,
        IntegrationTimeout = 8002,

        // Security related errors.
        SecurityViolation = 9000,
        DataTampering = 9001,

        //Caching related errors
        CacheFailure = 10000,
        CacheMiss = 10001
    }
}
