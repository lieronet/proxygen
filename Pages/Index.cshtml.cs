using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using proxygen.Models;
using JsonSerializer = System.Text.Json.JsonSerializer;

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

        public string Cards { get; set; }
        public void OnGet()
        {
            var randy = new Random();

            Cards = "CARDS";

            for (int i = 0; i < randy.Next(1, 10); i++)
            {
                Cards += "CARDS";
            }
        }

        [BindProperty]
        public string Input { get; set; }

        //TODO: error messaging
        public async Task<RedirectToPageResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Input)) return RedirectToPage("Index");

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
            List<string> rawCards = new(Input.Split(Environment.NewLine));

            List<SanitizedCardsToPrint> cards = new();

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

            var scryfallClient = _clientFactory.CreateClient("Scryfall");

            var resultsObject = new List<ProxyPageModel>();

            foreach (var cardPair in cardsToRun)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, @$"/cards/search?q=""{cardPair.CardName}""");

                var result = await scryfallClient.SendAsync(request);

                if (!result.IsSuccessStatusCode) continue;

                var resultString = await result.Content.ReadAsStringAsync();
                var resultsList = JsonConvert.DeserializeObject<ScryfallResultsObject>(resultString);

                if(resultsList.data.Any(x => x.name.Equals(cardPair.CardName, StringComparison.CurrentCultureIgnoreCase)))
                {
                    var card = resultsList.data
                        .SingleOrDefault(x => x.name.Equals(cardPair.CardName, StringComparison.CurrentCultureIgnoreCase));

                    //shouldn't be an issue anymore but you never know
                    if (card == null) continue;

                    for (var i = 0; i < cardPair.CardsToPrint; i++)
                    {
                        resultsObject.Add(new ProxyPageModel(card));
                    }
                }
                else if(resultsList.data.Any(x => x.card_faces != null))
                {
                    var card = resultsList.data
                        .Where(x=>x.card_faces != null)
                        .FirstOrDefault(x => x.card_faces.Any(y => 
                        y.name.Equals(cardPair.CardName, StringComparison.OrdinalIgnoreCase)));

                    if (card == null)
                    {
                        //nearest match
                        card = resultsList.data.FirstOrDefault();
                    }

                    resultsObject.AddRange(ParseDfcs(card, cardPair.CardsToPrint ?? 1));
                }
            }

            return resultsObject;
        }

        private IEnumerable<ProxyPageModel> ParseDfcs(ScryfallCardModel scryfallCardModel, int toPrint)
        {
            var results = new List<ProxyPageModel>();

            for(int i = 0; i < toPrint; i++)
            {
                foreach(var card in scryfallCardModel.card_faces)
                {
                    results.Add(new ProxyPageModel(card));
                }
            }

            return results;
        }
    }
}
