using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using IcreamShopApi.Data;

namespace IcreamShopApi.Controllers.ODATA
{
    [Route("odata/IceCreams")] 
    public class IceCreamController : ODataController
    {
        private readonly CreamDbContext _context;

        public IceCreamController(CreamDbContext context)
        {
            _context = context;
        }

        
        [EnableQuery]   
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_context.IceCreams.Include(i => i.Category));
        }

       
        [EnableQuery]
        [HttpGet("{key}")] 
        public IActionResult Get(int key)
        {
            var iceCream = _context.IceCreams.Include(i => i.Category).FirstOrDefault(i => i.IceCreamId == key);
            if (iceCream == null)
            {
                return NotFound();
            }
            return Ok(iceCream);
        }
    }
}
