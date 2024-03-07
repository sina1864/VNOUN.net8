using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Entities;
using Vnoun.Core.Entities;
using Vnoun.Core.Repositories;

namespace Vnoun.API.Controllers;

[Route("api/Home")]
[ApiController]
public class HomeController : BaseController<User>
{
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly ILocationRepository _locationRepository;
    private readonly IWebHostEnvironment _hostingeEnvironment;
    public HomeController(IMapper mapper, IWebHostEnvironment hostingEnvironment, IConfiguration config,
        IUserRepository userRepository, ILocationRepository locationRepository) : base(hostingEnvironment)
    {
        _mapper = mapper;
        _config = config;
        _userRepository = userRepository;
        _locationRepository = locationRepository;
        _hostingeEnvironment = hostingEnvironment;
    }

    [HttpGet("/")]
    public async Task<IActionResult> Index([FromQuery] string gt, [FromQuery] string lt)
    {
        var filter = Builders<Product>.Filter.ElemMatch("colors", Builders<Core.Entities.MetaEntities.Color>.Filter.Gt("price", int.Parse(gt)));

        filter &= Builders<Product>.Filter.ElemMatch("colors", Builders<Core.Entities.MetaEntities.Color>.Filter.Lt("price", int.Parse(lt)));

        filter &= Builders<Product>.Filter.ElemMatch("colors", Builders<Core.Entities.MetaEntities.Color>.Filter.In("colorName", new List<string>() { "red" }));

        var sorted = Builders<Product>.Sort.Ascending("colors.price");

        var products = DB.Collection<Product>()
            .Find(FilterDefinition<Product>.Empty)
            .Sort(sorted);

        return Ok(new
        {
            status = "success",
            data = products
        });
    }
}