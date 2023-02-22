﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using proxygen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace proxygen.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public IndexModel(ILogger<IndexModel> logger, IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public void OnGet()
        {

        }

        [BindProperty]
        public string Input { get; set; }

        //TODO: error messaging
        public async Task<RedirectToPageResult> OnPostAsync()
        {
            List<SanitizedCardsToPrint> cards = ParseInput();

            // pass cards to the method that will grab text
            // method will check cache for text, then fall back on the Scryfall API
            // maybe I'll pass a list of Not Found cards to the end user as well, to give them feedback
            // and to give me test cases to run

            // then redirect to the output page, passing along the list of cards

			TempData["cards"] = JsonSerializer.Serialize(await BuildModel(cards));

            return RedirectToPage("Proxies");
        }

        //todo: move me somewhere else
        private List<SanitizedCardsToPrint> ParseInput()
        {
            // probably going to need to account for multiple newline characters - we'll see how that goes
            List<string> rawCards = new List<string>(Input.Split(Environment.NewLine));

            List<SanitizedCardsToPrint> cards = new List<SanitizedCardsToPrint>();

            foreach (var pair in rawCards)
            {
                // fails on 1997 world champion and some random korean card
                // maybe I'll fix, maybe I won't
                if (int.TryParse(pair.Substring(0, Math.Max(pair.IndexOf(' '), 0)), out int num))
                {
                    cards.Add(new SanitizedCardsToPrint
                    {
                        CardsToPrint = num,
                        CardName = pair.Substring(pair.IndexOf(' ')).Trim()
                    });
                }
                else
                {
                    cards.Add(new SanitizedCardsToPrint
                    {
                        CardsToPrint = 1,
                        CardName = pair.Trim()
                    });
                }
            }

            return cards;
        }

        private async Task<List<ProxyPageModel>> BuildModel(List<SanitizedCardsToPrint> cardsToRun)
        {
            //TODO: check cache

            var scryfallClient = _clientFactory.CreateClient();

            var resultsObject = new List<ProxyPageModel>();

            foreach (var cardPair in cardsToRun)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, @$"https://api.scryfall.com/cards/search?q={"\"" + cardPair.CardName + "\"" }");

                var result = await scryfallClient.SendAsync(request);

                if (!result.IsSuccessStatusCode) continue;
                
                var resultString = await result.Content.ReadAsStringAsync();
                var resultsList = Newtonsoft.Json.JsonConvert.DeserializeObject<ScryfallResultsObject>(resultString);

                var card = resultsList.data
                    .SingleOrDefault(x => x.name.ToLower() == cardPair.CardName.ToLower());

                //todo: handle DFCs better
                if (card == null) continue;

                for(var i = 0; i < cardPair.CardsToPrint; i++)
                {
                    resultsObject.Add(new ProxyPageModel(card));
                }
            }

            return resultsObject;
        }
    }
}
