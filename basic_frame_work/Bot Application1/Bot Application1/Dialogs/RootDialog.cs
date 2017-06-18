using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Net;
using Newtonsoft.Json;

namespace Bot_Application1.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
           // int length = (activity.Text ?? string.Empty).Length;
            var question = activity.Text;
           // var response = $"问题\"{question}\"的答案是：答案";

            // return our reply to the user
           



            string responseString = string.Empty;

            var query = question; //User Query
            var knowledgebaseId = "a80ffe18-c56f-487e-91d4-d6ee495223ab"; // Use knowledge base id created.
            var qnamakerSubscriptionKey = "904d93ec99d440969c82d25ddaebf814"; //Use subscription key assigned to you.

            //Build the URI
            Uri qnamakerUriBase = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v1.0");
            var builder = new UriBuilder($"{qnamakerUriBase}/knowledgebases/{knowledgebaseId}/generateAnswer");

            //Add the question as part of the body
            var postBody = $"{{\"question\": \"{query}\"}}";

            //Send the POST request
            using (WebClient client = new WebClient())
            {
                //Set the encoding to UTF8
                client.Encoding = System.Text.Encoding.UTF8;

                //Add the subscription key header
                client.Headers.Add("Ocp-Apim-Subscription-Key", qnamakerSubscriptionKey);
                client.Headers.Add("Content-Type", "application/json");
                responseString = client.UploadString(builder.Uri, postBody);
                QnAMakerResult response;
                try
                {
                    response = JsonConvert.DeserializeObject<QnAMakerResult>(responseString);
                }
                catch
                {
                    throw new Exception("Unable to deserialize QnA Maker response string.");
                }
                await context.PostAsync(response.Answer);
            }






            context.Wait(MessageReceivedAsync);
        }
    }


    public class QnAMakerResult
    {
        /// <summary>
        /// The top answer found in the QnA Service.
        /// </summary>
        [JsonProperty(PropertyName = "answer")]
        public string Answer { get; set; }

        /// <summary>
        /// The score in range [0, 100] corresponding to the top answer found in the QnA    Service.
        /// </summary>
        [JsonProperty(PropertyName = "score")]
        public double Score { get; set; }
    }


}