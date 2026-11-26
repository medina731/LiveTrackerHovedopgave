using LiveTrackerHovedopgave.Models;
using LiveTrackerHovedopgave.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LiveTrackerHovedopgave.Controllers;

[Route("api/bundles")]
public class ApiController : ControllerBase
{
    private IBundleRepository repository;

    public ApiController(IBundleRepository repository)
    {
        this.repository = repository;
    }

    // Modtager fremskridt fra modden og gemmer det for den pågældende save
    [HttpPost("{saveFolderName}")]
    public IActionResult POST(string saveFolderName, [FromBody] List<GoldenScroll> rooms)
    {
        if (rooms == null)
        {
            return BadRequest();
        }

        bool gemt = repository.SaveProgress(saveFolderName, rooms);

        if (gemt == false)
        {
            return NotFound();
        }

        return Ok();
    }

    // Returnerer rum, bundles og slots for en save som JSON
    [HttpGet("{saveFolderName}")]
    public IActionResult GET(string saveFolderName)
    {
        List<GoldenScroll> rooms = repository.GetAll(saveFolderName);
        return Ok(rooms);
    }
}