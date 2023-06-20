using System;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace TradeReportETL.Common.Http
{
    public class HttpStatusExceptionMiddleware
    {
        private readonly ILogger<HttpStatusExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;

        public HttpStatusExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = loggerFactory?.CreateLogger<HttpStatusExceptionMiddleware>() ??
                      throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (AccessDeniedException ex)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, HttpStatusExceptionMiddleware ignored");

                    throw;
                }

                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                context.Response.ContentType = "application/json";
                var json = JsonConvert.SerializeObject(new
                {
                    status = 0,
                    message = ex.Message
                });

                await context.Response.WriteAsync(json);
            }
            catch (NotFoundException ex)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, HttpStatusExceptionMiddleware ignored");

                    throw;
                }

                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                context.Response.ContentType = "application/json";
                var json = JsonConvert.SerializeObject(new
                {
                    status = 0,
                    message = ex.Message
                });

                await context.Response.WriteAsync(json);
            }
            catch (ConflictingOperationException ex)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, HttpStatusExceptionMiddleware ignored");

                    throw;
                }

                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                context.Response.ContentType = "application/json";
                var json = JsonConvert.SerializeObject(new
                {
                    status = 0,
                    message = ex.Message
                });

                await context.Response.WriteAsync(json);
            }
            catch (ArgumentException ex)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, HttpStatusExceptionMiddleware ignored");

                    throw;
                }

                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
                context.Response.ContentType = "application/json";
                var json = JsonConvert.SerializeObject(new
                {
                    status = 0,
                    message = ex.Message
                });

                await context.Response.WriteAsync(json);
            }
            catch (Exception ex) when (ex is SqlException || ex.InnerException is SqlException)
            {
                if (context.Response.HasStarted)
                {
                    _logger.LogWarning("The response has already started, HttpStatusExceptionMiddleware ignored");

                    throw;
                }

                var sqlException = (SqlException)(ex is SqlException ? ex : ex.InnerException);
                context.Response.Clear();
                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                context.Response.ContentType = "application/json";
                var json = JsonConvert.SerializeObject(new
                {
                    status = 0,
                    message = sqlException.ToString()
                });

                await context.Response.WriteAsync(json);
            }
        }
    }

    public class AccessDeniedException : Exception
    {
        public AccessDeniedException()
        {
        }

        public AccessDeniedException(string message) : base(message)
        {
        }
    }


    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }

    public class ConflictingOperationException : Exception
    {
        public ConflictingOperationException(string message) : base(message)
        {
        }

        public ConflictingOperationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}