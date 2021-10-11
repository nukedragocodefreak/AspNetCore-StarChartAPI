using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StarChart.Data;
using StarChart.Models;

namespace StarChart.Controllers
{
    [Route("")]
    [ApiController]
    public class CelestialObjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public CelestialObjectController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public IActionResult GetById(int id)
        {
            var result = _context.CelestialObjects.Find(id);
            if (result == null)
            {
                return NotFound();
            }

            result.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == id).ToList();
            return Ok(result);
        }

        [HttpGet("{name}")]
        public IActionResult GetByName(string name)
        {
            var result = _context.CelestialObjects.Where(e => e.Name == name).ToList();
            if (!result.Any())
            {
                return NotFound();
            }

            foreach (var res in result)
            {
                res.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == res.Id).ToList();
            }

            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var result = _context.CelestialObjects.ToList();

            foreach (var res in result)
            {
                res.Satellites = _context.CelestialObjects.Where(e => e.OrbitedObjectId == res.Id).ToList();
            }
            return Ok(result);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, CelestialObject celestialObject)
        {
            var result = _context.CelestialObjects.Find(id);
            if (result == null)
            {
                return NotFound();
            }
            result.Name = celestialObject.Name;
            result.OrbitalPeriod = celestialObject.OrbitalPeriod;
            result.OrbitedObjectId = celestialObject.OrbitedObjectId;
            _context.CelestialObjects.Update(result);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpPost]
        public IActionResult Create([FromBody] CelestialObject celestial)
        {
            _context.CelestialObjects.Add(celestial);
            _context.SaveChanges();
            return CreatedAtRoute("GetById", new { id = celestial.Id }, celestial);
        }

        [HttpPatch("{id}/{name}")]
        public IActionResult RenameObject(int id, string name)
        {
            var result = _context.CelestialObjects.Find(id);

            if (result == null)
            {
                return NotFound();
            }

            result.Name = name;
            _context.CelestialObjects.Update(result);
            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var result = _context.CelestialObjects.Where(e => e.Id == id || e.OrbitedObjectId == id);
            if (!result.Any())
            {
                return NotFound();
            }
            _context.CelestialObjects.RemoveRange(result);
            _context.SaveChanges();
            return NoContent();

        }

    }

}
