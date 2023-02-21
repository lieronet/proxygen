using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using proxygen.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

        public async Task OnPostAsync()
        {
            List<CardsModel> cards = ParseInput();

            BuildModel(cards);
            // pass cards to the method that will grab text
            // method will check cache for text, then fall back on the Scryfall API
            // maybe I'll pass a list of Not Found cards to the end user as well, to give them feedback
            // and to give me test cases to run

            // then redirect to the output page, passing along the list of cards
        }

        private List<CardsModel> ParseInput()
        {
            // probably going to need to account for multiple newline characters - we'll see how that goes
            List<string> rawCards = new List<string>(Input.Split(Environment.NewLine));

            List<CardsModel> cards = new List<CardsModel>();

            foreach (var pair in rawCards)
            {
                // fails on 1997 world champion and some random korean card
                // maybe I'll fix, maybe I won't
                if (int.TryParse(pair.Substring(0, Math.Max(pair.IndexOf(' '), 0)), out int num))
                {
                    cards.Add(new CardsModel
                    {
                        CardsToPrint = num,
                        CardName = pair.Substring(pair.IndexOf(' ')).Trim()
                    });
                }
                else
                {
                    cards.Add(new CardsModel
                    {
                        CardsToPrint = 1,
                        CardName = pair.Trim()
                    });
                }
            }

            return cards;
        }

        private async void BuildModel(List<CardsModel> cardsToRun)
        {
            //TODO: check cache

            var scryfallClient = _clientFactory.CreateClient();

            var resultsObject = new ProxyViewModel();

            foreach (var cardPair in cardsToRun)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, @$"https://api.scryfall.com/cards/search?q={"\"" + cardPair.CardName + "\"" }");

                var result = await scryfallClient.SendAsync(request);

                if (result.IsSuccessStatusCode)
                {
                    var resultString = await result.Content.ReadAsStringAsync();
                    var resultsList = Newtonsoft.Json.JsonConvert.DeserializeObject<ScryfallResultsObject>(resultString);

                    var card = resultsList.data
                        .Where(x => x.name.ToLower() == cardPair.CardName.ToLower())
                        .SingleOrDefault();

                    resultsObject.Cards.Add(new CardProxies()
                    {
                        NumToPrint = cardPair.CardsToPrint.GetValueOrDefault(),
                        Card = card
                    });
                }
            }
        }
    }
}
