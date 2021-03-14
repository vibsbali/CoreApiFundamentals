using System;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore.Internal;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiVersion("2.0")]
    [ApiController] //this gives validation features that otherwise require ModelState.IsValid
    public class Camps2Controller : ControllerBase
    {
        private readonly ICampRepository _repository;
        private IMapper _mapper;
        private LinkGenerator _linkGenerator;

        public Camps2Controller(ICampRepository repository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _repository = repository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }


        [HttpGet]
        public async Task<ActionResult> GetCamps(bool includeTalks = false)
        {
            try
            {
                var results = await _repository.GetAllCampsAsync(includeTalks);
                var result = new
                {
                    Count = results.Length,
                    Results = _mapper.Map<CampModel[]>(results)
                };
                
                return Ok(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [MapToApiVersion("1.0")]
        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> Get(string moniker)
        {
            try
            {
                var result = await _repository.GetCampAsync(moniker);
                if (result == null || result.Length == 0)
                {
                    return NotFound();
                }

                var model = _mapper.Map<CampModel>(result);
                return Ok(model);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [ApiVersion("1.1")]
        [HttpGet("{moniker}")]
        public async Task<ActionResult<CampModel>> Get11(string moniker)
        {
            try
            {
                //this will return all the talk data as well
                var result = await _repository.GetCampAsync(moniker, true);
                if (result == null || result.Length == 0)
                {
                    return NotFound();
                }

                var model = _mapper.Map<CampModel>(result);
                return Ok(model);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime theDate, bool includeTalks = false)
        {
            try
            {
                var results = await _repository.GetAllCampsByEventDate(theDate, includeTalks);
                if (!results.Any())
                {
                    return NotFound();
                }

                return _mapper.Map<CampModel[]>(results);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CampModel>> Post(CampModel model)
        {
            try
            {
                var campWithThisMoniker = await _repository.GetCampAsync(model.Moniker);
                if (campWithThisMoniker != null)
                {
                    return BadRequest("Talk already exists");
                }

                var location = _linkGenerator.GetPathByAction("Get", "Camps", new {moniker = model.Moniker});
                if (string.IsNullOrWhiteSpace(location))
                {
                    return BadRequest("Could not use this moniker");
                }
                //create a new camp
                var camp = _mapper.Map<Camp>(model);
                _repository.Add(camp);
                if (await _repository.SaveChangesAsync())
                {
                    return Created($"{location}", _mapper.Map<CampModel>(camp));
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> Put(string moniker, CampModel model)
        {
            try
            {
                //check that moniker and model.Moniker are same

                var existingModel = await _repository.GetCampAsync(model.Moniker);
                if (existingModel == null)
                {
                    return BadRequest("Moniker doesn't exists");
                }

                _mapper.Map(model, existingModel);

                if (await _repository.SaveChangesAsync())
                {
                    return _mapper.Map<CampModel>(existingModel);
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string moniker)
        {
            try
            {
                var existingModel = await _repository.GetCampAsync(moniker);
                if (existingModel == null)
                {
                    return BadRequest("Moniker doesn't exists");
                }

                _repository.Delete(existingModel);
                if (await _repository.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Failure");
            }

            return BadRequest();
        }

    }
}
