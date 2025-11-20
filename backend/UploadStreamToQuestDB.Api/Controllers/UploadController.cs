using Common.RabbitMQ;
using Common.RabbitMQ.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MSSql.Infrastructure.Entities;
using MSSql.Infrastructure.Repositories.Abstractions;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using UploadStream;
using UploadStreamToQuestDB.API.CustomAttributes;
using UploadStreamToQuestDB.API.Exceptions;
using UploadStreamToQuestDB.API.SwaggerFilters;
using UploadStreamToQuestDB.Application;
using UploadStreamToQuestDB.Domain;

namespace UploadStreamToQuestDB.API.Controllers {
    /// <summary>
    /// Controller for handling file upload requests.
    /// </summary>
    [Route("api")]
    [Produces("application/json")]
    public class UploadController : Controller {
        private readonly ILogger<UploadController> _logger;
        private readonly IUploadPipeline _pipeline;
        private readonly IQueueService queueService;
        //private readonly IServiceScopeFactory _scopeFactory;
        //private readonly IEventRepository eventRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UploadController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="pipeline">The upload pipeline.</param>
        public UploadController(
            ILogger<UploadController> logger,
            IUploadPipeline pipeline,
            IQueueService queueService) {
            _logger = logger;
            _pipeline = pipeline;
            this.queueService = queueService;
            //this.eventRepository = eventRepository;
        }

        /// <summary>
        /// Endpoint for uploading files to the server.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> containing the upload result.</returns>
        [HttpPost("stream")]
        [SwaggerOperation(Summary = "Endpoint for uploading files to the server.")]
        [MultipartFormData]
        [DisableFormModelBinding]
        [FileUploadOperation.FileContentType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ControllerStream() {
            _logger.LogTrace("Controller upload stream starts.");
            if (!Request.Headers.ContainsKey("X-SessionId") || string.IsNullOrEmpty(Request.Headers["X-SessionId"]))
                throw new XSessionIdException();

            FileModelsInput files = new FileModelsInput() {
                SessionId = Request.Headers["X-SessionId"],
                FilePath = Path.Join(
                    Path.GetTempPath(),
                    Guid.NewGuid().ToString())
            };
            _logger.LogTrace($"X-SessionId is equal to {files.SessionId}");
            _logger.LogTrace($"FilePath is equal to {files.FilePath}");

            _logger.LogTrace($"Pipeline is initializing.");
            _pipeline.Initialize(this);
            _logger.LogTrace($"Pipeline is initialized.");

            _logger.LogTrace($"Pipeline starts.");
            await _pipeline.Run(files);
            _logger.LogTrace($"Pipeline is executed.");

            if (!ModelState.IsValid)
                return BadRequest();
            _logger.LogTrace($"Model is valid.");

            _logger.LogTrace("Controller upload stream is ended.");
            if (files.HasErrors) {
                _logger.LogError("There are errors in the uploaded files.");
                var problem = new ProblemDetails {
                    Title = files.SessionId,
                    Detail = files.FilePath,
                    Status = 500
                };

                problem.Extensions.Add("Errors", files.Select(x => {
                    var state = x.GetState();

                    return new {
                        state,
                        x.File.Name,
                        x.File.FileName,
                        x.File.ContentDisposition,
                        x.File.ContentType,
                        x.File.Length
                    };
                }));

                return BadRequest(problem);
            } else {
                // zapisz do bazy guida
                //using var scope = _scopeFactory.CreateScope();
                //var repo = scope.ServiceProvider.GetRequiredService<IEventRepository>();

                //var result = await repo.AddAsync(new EventEntry() {
                //    Id = files.SessionId,
                //});

                var algorithmA = new AlgorithmDetailsA() {
                    Id = Guid.NewGuid().ToString(),
                    SessionId = files.SessionId
                };
                string msgA = JsonSerializer.Serialize(algorithmA);
                var algorithmB = new AlgorithmDetailsB() {
                    Id = Guid.NewGuid().ToString(),
                    SessionId = files.SessionId
                };
                string msgB = JsonSerializer.Serialize(algorithmB);
                var algorithmC = new AlgorithmDetailsC() {
                    Id = Guid.NewGuid().ToString(),
                    SessionId = files.SessionId
                };
                string msgC = JsonSerializer.Serialize(algorithmC);

                await queueService.Publish(msgA);
                await queueService.Publish(msgB);
                await queueService.Publish(msgC);

                return Ok(new {
                    files.SessionId,
                    files.FilePath,
                    Files = files.Select(x => {
                        var state = x.GetState();

                        return new {
                            state,
                            x.File.Name,
                            x.File.FileName,
                            x.File.ContentDisposition,
                            x.File.ContentType,
                            x.File.Length
                        };
                    })
                });
            }
        }
    }
}
