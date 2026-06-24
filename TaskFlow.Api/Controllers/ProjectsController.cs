using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TaskFlow.Application.Services.Projects;
using TaskFlow.Application.DTOs.Projects;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/projects")]
[Authorize]
public sealed class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IProjectService projectService, ILogger<ProjectsController> logger)
    {
        _projectService = projectService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<ProjectDto>> Create([FromBody] CreateProjectRequest request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Create called. IsAuthenticated={IsAuthenticated}", User?.Identity?.IsAuthenticated);
        _logger.LogDebug("NameIdentifier claim={NameId}", User?.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        CreateProjectRequest finalRequest = request;

        if (request.OwnerUserId == Guid.Empty)
        {
            var sub = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(sub, out var ownerId))
                return Unauthorized(); // retourner 401 quand pas authentifié / claim manquant

            finalRequest = new CreateProjectRequest
            {
                Name = request.Name,
                OwnerUserId = ownerId
            };
        }

        var project = await _projectService.CreateAsync(finalRequest, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var project = await _projectService.GetByIdAsync(id, cancellationToken);
        return project is null ? NotFound() : Ok(project);
    }

    [HttpGet("owner/{ownerUserId:guid}")]
    public async Task<ActionResult<IReadOnlyList<ProjectDto>>> GetByOwnerUserId(Guid ownerUserId, CancellationToken cancellationToken)
    {
        var projects = await _projectService.GetByOwnerUserIdAsync(ownerUserId, cancellationToken);
        return Ok(projects);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProjectDto>> Update(Guid id, [FromBody] UpdateProjectRequest request, CancellationToken cancellationToken)
    {
        var project = await _projectService.UpdateAsync(id, request, cancellationToken);
        return project is null ? NotFound() : Ok(project);
    }
}