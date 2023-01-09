using HNTopAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace HNTopAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {

        private readonly ILogger<ItemsController> _logger;

        public ItemsController(ILogger<ItemsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync(string sort)
        {
            List<Item> items = sort == "best" ? Globals.Globals.GlobalItemsSortedByScore : Globals.Globals.GlobalItems;
            return new JsonResult(items);
        }
        [HttpGet]
        [Route("{page_id:int}")]
        public async Task<IActionResult> GetItemById(int page_id)
        {
            var item = Globals.Globals.GlobalFullStories[page_id];                       
            return new JsonResult(item);
        }
    }
}