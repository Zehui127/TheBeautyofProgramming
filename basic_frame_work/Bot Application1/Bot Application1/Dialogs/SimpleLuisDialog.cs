using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Net;
using Newtonsoft.Json;

namespace Bot_Application1.Dialogs
{
    [LuisModel("f6b501c3-9ad2-4ce6-a086-3d9f2c0c329b","5ae56471cf00425f8f949ead61af47fd" )]
    [Serializable]
    public class SimpleLuisDialog : LuisDialog<object>
    {
        public const string Department = "院系";
        public const string Position = "职位";
        public const string Name = "名字";
        public const string Degree = "学位";
        public const string Action = "干什么";

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceived);
        }
        [LuisIntent("None")]
        private async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"对不起，我不太理解你的请求：";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
        [LuisIntent("查询时间")]
        private async Task findWhen(IDialogContext context, LuisResult result)
        {
            EntityRecommendation department,position,name,degree,action;
            
            
            
           
            if (!result.TryFindEntity(Department, out department) && !result.TryFindEntity(Position, out position) && !result.TryFindEntity(Name, out name) && !result.TryFindEntity(Degree, out degree) && !result.TryFindEntity(Action, out action))
            {
                await context.PostAsync("无意义问题");
            }
            else
            {
                var question = "";
                if (result.TryFindEntity(Department, out department)) {
                    question += department.Entity;
                }
                if (result.TryFindEntity(Position, out position)) {
                    question += position.Entity;
                }
                if (result.TryFindEntity(Name, out name)) {
                    question += name.Entity;
                }
                if (result.TryFindEntity(Degree, out degree)) {
                    question += degree.Entity;
                }
                if (result.TryFindEntity(Action, out action)) {
                    question += action.Entity;
                }


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




            
            }
            context.Wait(MessageReceived);
        }
        [LuisIntent("查询人名")]
        private async Task findName(IDialogContext context, LuisResult result)
        {
            EntityRecommendation department;
            if (!result.TryFindEntity(Department, out department))
            {
                await context.PostAsync("请明确查询人的院系");
            }
            else
            {
                await context.PostAsync($"输入人的输入的院系是{department.Entity}");
            }
            context.Wait(MessageReceived);
        }


    }
}