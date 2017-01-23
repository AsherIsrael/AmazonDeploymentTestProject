using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuotingDojoRedux.Factories;
using QuotingDojoRedux.Models;

namespace QuotingDojoRedux.Controllers
{
    public class QuotesController : Controller
    {
        private readonly QuoteFactory _quoteFactory;

        public QuotesController(QuoteFactory quoteFactory)
        {
            _quoteFactory = quoteFactory;
        }
        
        [HttpGet]
        [Route("quotes")]
        public IActionResult Index()
        {
            if(!CheckLogin())
            {
                return RedirectToAction("RegisterPage", "Users");
            }
            IEnumerable<Quote> AllQuotes = _quoteFactory.FindAll();
            ViewBag.Quotes = AllQuotes;
            ViewBag.CurrUserId = (int)HttpContext.Session.GetInt32("CurrUserId");
            return View();
        }

        [HttpGet]
        [Route("newquote")]
        public IActionResult NewQuote()
        {
            if(!CheckLogin())
            {
                return RedirectToAction("RegisterPage", "Users");
            }
            ViewBag.Errors = new List<string>();
            return View();
        }

        [HttpPost]
        [Route("quotes")]
        public IActionResult Process(Quote model)
        {
            if(ModelState.IsValid)
            {
                _quoteFactory.Add(model, (int)HttpContext.Session.GetInt32("CurrUserId"));
                return RedirectToAction("Index");
            }
            ViewBag.Errors = ModelState.Values;
            return View("NewQuote");
        }

        [HttpGet]
        [Route("deletequote/{QuoteId}")]
        public IActionResult Delete(int QuoteId)
        {
            if(!CheckLogin())
            {
                return RedirectToAction("RegisterPage", "Users");
            }
            Quote DeleteQuote = _quoteFactory.GetQuoteById(QuoteId);
            if(DeleteQuote.UserId == (int)HttpContext.Session.GetInt32("CurrUserId"))
            {
                 _quoteFactory.Delete(QuoteId);
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("editquote/{QuoteId}")]
        public IActionResult EditPage(int QuoteId)
        {
            if(!CheckLogin())
            {
                return RedirectToAction("RegisterPage", "Users");
            }
            Quote EditQuote = _quoteFactory.GetQuoteById(QuoteId);
            if(EditQuote.UserId == (int)HttpContext.Session.GetInt32("CurrUserId"))
            {
                ViewBag.QuoteId = EditQuote.QuoteId;
                ViewBag.OldContent = EditQuote.QuoteContent;
                 return View();
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("edit/{QuoteId}")]
        public IActionResult Edit(int QuoteId, Quote Revision)
        {
            if(!CheckLogin())
            {
                return RedirectToAction("RegisterPage", "Users");
            }
            Quote EditQuote = _quoteFactory.GetQuoteById(QuoteId);
            if(EditQuote.UserId == (int)HttpContext.Session.GetInt32("CurrUserId"))
            {
                EditQuote.QuoteContent = Revision.QuoteContent;
                _quoteFactory.Update(EditQuote);
            }
            return RedirectToAction("Index");
        }

        private bool CheckLogin()
        {
            return (HttpContext.Session.GetInt32("CurrUserId") != null);
        }
    }
}