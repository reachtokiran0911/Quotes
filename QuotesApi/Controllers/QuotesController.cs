using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuotesApi.Data;
using QuotesApi.Models;

namespace QuotesApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Quotes")]
    [Authorize]
    public class QuotesController : Controller
    {

        private QuotesDBContext _dbContext;

        public QuotesController(QuotesDBContext context)
        {
            _dbContext = context;
        }
        // GET: api/Quotes
        [HttpGet]
        [ResponseCache(Duration =60)]
        [AllowAnonymous]
        public IActionResult Get(string sort)
        {
            IQueryable<Quote> _quotes;
            switch(sort)
            {
                case "desc":
                    _quotes = _dbContext.Quotes.OrderByDescending(q => q.CreatedAt);
                    break;
                case "asc":
                    _quotes = _dbContext.Quotes.OrderBy(q => q.CreatedAt);
                    break;
                default:
                    _quotes = _dbContext.Quotes;
                    break;
            }

            return  Ok(_quotes);
        }

        [HttpGet("[action]")]
        public IActionResult MyQuote()
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var result = _dbContext.Quotes.Where(q => q.UserId == userId);
            return Ok(result);
        }

        [HttpGet("[Action]")]
        public IActionResult PagingQuote(int? pageNumber, int? pageSize)
        {
            var quotes = _dbContext.Quotes.OrderBy(q => q.Id);
            int currentPageNumber = pageNumber ?? 1;
            int currentPageSize = pageSize ?? 5;
            return Ok(quotes.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize));
        }

        [HttpGet]
        [Route("[Action]")]
        public IActionResult SearchQuote(string type)
        {
            var result = _dbContext.Quotes.Where(q => q.Type.StartsWith(type));
            return Ok(result);
        }

        // GET: api/Quotes/5
        [HttpGet("{id}", Name = "Get")]
        public IActionResult Get(int id)
        {
            var quote = _dbContext.Quotes.Find(id);
            if(quote != null)
            {
                return Ok(quote);
            }
           else
            {
                return NotFound("Record doesnot exist against this id..");
            }
        }
        
        // POST: api/Quotes
        [HttpPost]
        public IActionResult Post([FromBody]Quote objQuotes)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            objQuotes.UserId = userId;
            _dbContext.Quotes.Add(objQuotes);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }
        
        // PUT: api/Quotes/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]Quote objQuotes)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var objResult = _dbContext.Quotes.Find(id);
            if(objResult == null)
            {
                return NotFound("no record found against this id");
            }
            if(userId != objResult.UserId)
            {
                return BadRequest("no record found for this user.");
            }
            else
            {
                objResult.Title = objQuotes.Title;
                objResult.Author = objQuotes.Author;
                objResult.Description = objQuotes.Description;
                objResult.Type = objQuotes.Type;
                objResult.CreatedAt = objQuotes.CreatedAt;
                _dbContext.SaveChanges();
                return Ok("Record updated successfully !!");
            }
          

        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            string userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            var objQuote = _dbContext.Quotes.Find(id);
            if(objQuote != null)
            {
                _dbContext.Quotes.Remove(objQuote);
                _dbContext.SaveChanges();
                return Ok("Record deleted successfully.. ");
            }

            if (userId != objQuote.UserId)
            {
                return BadRequest("no record found for this user.");
            }
            else
            {
                return NotFound("Record doesnot exist against this id.");
            }
          
        }
    }
}
