using MagicVilla_villaAPI.Data;
using MagicVilla_villaAPI.Models;
using MagicVilla_villaAPI.Models.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_villaAPI.Controllers
{
    [Route("api/VillaAPI")]
    [ApiController]
    public class VillaAPIController : ControllerBase // Inheriting from ControllerBase, which provides basic functionalities for an API controller
    {
        // HTTP GET method to retrieve all villas
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)] // Specifies that this method can return a 200 OK response
        public ActionResult<IEnumerable<VillaDTO>> GetVillas()
        {
            // Returns the list of villas from the VillaStore
            return Ok(VillaStore.villaList);
        }

        // HTTP GET method to retrieve a specific villa by ID
        [HttpGet("{id:int}", Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)] // Specifies that this method can return a 200 OK response
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Specifies that this method can return a 404 Not Found response
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Specifies that this method can return a 400 Bad Request response
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            // Check if the ID is 0, which is invalid
            if (id == 0)
            {
                return BadRequest(); // Return 400 Bad Request
            }

            // Find the villa with the specified ID in the villa list
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if (villa == null) // Check if the villa is not found
            {
                return NotFound(); // Return 404 Not Found
            }
            else
            {
                return Ok(villa); // Return 200 OK with the villa
            }
        }

        // HTTP POST method to create a new villa
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] // Specifies that this method can return a 201 Created response
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Specifies that this method can return a 404 Not Found response
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Specifies that this method can return a 400 Bad Request response
        public ActionResult<VillaDTO> CreateVilla([FromBody] VillaDTO villaDTO)
        {
            // Check if a villa with the same name already exists
            if (VillaStore.villaList.FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                ModelState.AddModelError("", "Villa already exists!"); // Add an error to the model state
                return BadRequest(ModelState); // Return 400 Bad Request with the model state
            }

            // Check if the villaDTO is null
            if (villaDTO == null)
            {
                return BadRequest(villaDTO); // Return 400 Bad Request
            }

            // Check if the ID is greater than 0, which is invalid for a new villa
            if (villaDTO.Id > 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError); // Return 500 Internal Server Error
            }

            // Generate a new ID for the villa
            villaDTO.Id = VillaStore.villaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            // Add the new villa to the list
            VillaStore.villaList.Add(villaDTO);

            // Return 201 Created with the new villa and its location
            return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);
        }

        // HTTP DELETE method to delete a villa by ID
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Specifies that this method can return a 204 No Content response
        [ProducesResponseType(StatusCodes.Status404NotFound)] // Specifies that this method can return a 404 Not Found response
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Specifies that this method can return a 400 Bad Request response
        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        public IActionResult DeleteVilla(int id)
        {
            // Check if the ID is 0, which is invalid
            if (id == 0)
            {
                return BadRequest(); // Return 400 Bad Request
            }

            // Find the villa with the specified ID in the villa list
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if (villa == null) // Check if the villa is not found
            {
                return NotFound(); // Return 404 Not Found
            }

            // Remove the villa from the list
            VillaStore.villaList.Remove(villa);
            return NoContent(); // Return 204 No Content
        }

        // HTTP PUT method to update a villa by ID
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Specifies that this method can return a 204 No Content response
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Specifies that this method can return a 400 Bad Request response
        public IActionResult UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
        {
            // Check if the villaDTO is null or the ID does not match the villaDTO ID
            if (villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest(); // Return 400 Bad Request
            }

            // Find the villa with the specified ID in the villa list
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            // Update the villa properties
            villa.Name = villaDTO.Name;
            villa.Sqft = villaDTO.Sqft;
            villa.Occupancy = villaDTO.Occupancy;

            return NoContent(); // Return 204 No Content
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // Specifies that this method can return a 204 No Content response
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // Specifies that this method can return a 400 Bad Request response
        public IActionResult UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest(); // Return 400 Bad Request
            }
            var villa = VillaStore.villaList.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return BadRequest();
            }
            patchDTO.ApplyTo(villa, ModelState);
            if (ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }


    }
}
