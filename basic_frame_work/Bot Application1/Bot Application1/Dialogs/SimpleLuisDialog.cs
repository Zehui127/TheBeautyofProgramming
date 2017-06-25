using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bot_Application1.Dialogs
{
    [LuisModel("f6b501c3-9ad2-4ce6-a086-3d9f2c0c329b","5ae56471cf00425f8f949ead61af47fd" )]
    [Serializable]
    public class SimpleLuisDialog : LuisDialog<object>
    {
        protected string preEntitySet;
        public List<string> entityList = new List<string>(new string[] { "可数的::人数","可数的::建筑设施","可数的::面积","可数的::学术机构","可数的::其他", "可数的::职位", "附属信息","院系","院系::专业","名字", "学位" , "干什么" });
     

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceived);
        }
        [LuisIntent("None")]
        private async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"对不起，小白不知道你想问什么";
            await context.PostAsync(message);
            context.Wait(MessageReceived);
        }
        [LuisIntent("查询时间")]
        private async Task findWhen(IDialogContext context, LuisResult result)
        {
            var question = "";
            EntityRecommendation tempEntity;
            bool flag = true;
            if (result.TryFindEntity("代词", out tempEntity))
            {
                flag = false;

            }
            else {
                preEntitySet = "";
            }
            foreach (string myEntity in entityList) {

                if (result.TryFindEntity(myEntity, out tempEntity)) {
                    question += " " + tempEntity.Entity;
                    if (myEntity !="干什么") {
                        preEntitySet += " " + tempEntity.Entity;
                    }

                }
            }
            if (!flag) {
                question += " " + preEntitySet;
            }

                string responseString = string.Empty;

                var query = question; //User Query
                var knowledgebaseId = "a80ffe18-c56f-487e-91d4-d6ee495223ab"; // Use knowledge base -- 问时间.
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
           
                    await context.PostAsync(preEntitySet + "----" + question + "Answer:"+response.Answer + response.Score + result.TopScoringIntent.Intent + result.Entities[0].StartIndex + result.Query[0]);
              
           
                
                // await context.PostAsync(question);
            }
            context.Wait(MessageReceived);
        }
        [LuisIntent("查询人名")]
        private async Task findName(IDialogContext context, LuisResult result)
        {
            var question = "";
            EntityRecommendation tempEntity;
            bool flag = true;
            if (result.TryFindEntity("代词", out tempEntity))
            {
                flag = false;

            }
            else
            {
                preEntitySet = "";
            }
            foreach (string myEntity in entityList)
            {

                if (result.TryFindEntity(myEntity, out tempEntity))
                {
                    question += " " + tempEntity.Entity;
                    if (myEntity != "干什么")
                    {
                        preEntitySet += " " + tempEntity.Entity;
                    }

                }
            }
            if (!flag)
            {
                question += " " + preEntitySet;
            }
            string responseString = string.Empty;

            var query = question; //User Query
            var knowledgebaseId = "ffb71279-23fa-4727-bc6a-d94646c6b9c9"; // Use knowledge base -- 问人民.
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
                if (response.Score != 0)
                {
                    await context.PostAsync(preEntitySet + "----" + question +"Answer: "+ response.Answer + response.Score + result.TopScoringIntent.Intent + result.Entities[0].StartIndex + result.Query[0]);
                }
                else
                {
                    await context.PostAsync("对不起，小白在学校官网上无法找到答案");
                }
            }
            context.Wait(MessageReceived);
        }


        [LuisIntent("查询附属信息")]
        private async Task findExtra(IDialogContext context, LuisResult result)
        {
            await context.PostAsync("进入查询附属信息");
            context.Wait(MessageReceived);
        }





        [LuisIntent("查询地点")]
        private async Task findPlace(IDialogContext context, LuisResult result)
        {
            var question = "";
            EntityRecommendation tempEntity;
            bool flag = true;
            if (result.TryFindEntity("代词", out tempEntity))
            {
                flag = false;

            }
            else
            {
                preEntitySet = "";
            }
            foreach (string myEntity in entityList)
            {

                if (result.TryFindEntity(myEntity, out tempEntity))
                {
                    question += " " + tempEntity.Entity;
                    if (myEntity != "干什么")
                    {
                        preEntitySet += " " + tempEntity.Entity;
                    }

                }
            }
            if (!flag)
            {
                question += " " + preEntitySet;
            }
            string responseString = string.Empty;

            var query = question; //User Query
            var knowledgebaseId = "62dc817e-2181-4a3e-b37a-676d7e222f66"; // Use knowledge base -- 问地点.
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
                if (response.Score != 0)
                {
                    await context.PostAsync(preEntitySet + "----" + question + "Answer: "+response.Answer + response.Score + result.TopScoringIntent.Intent + result.Entities[0].StartIndex + result.Query[0]);
                }
                else
                {
                    await context.PostAsync("对不起，小白在学校官网上无法找到答案");
                }
            }
            context.Wait(MessageReceived);
        }






        [LuisIntent("查询数值")]
        private async Task findNumValue(IDialogContext context, LuisResult result)
        {
            var question = "";
            EntityRecommendation tempEntity;
            bool flag = true;
            if (result.TryFindEntity("代词", out tempEntity))
            {
                flag = false;

            }
            else
            {
                preEntitySet = "";
            }
            foreach (string myEntity in entityList)
            {

                if (result.TryFindEntity(myEntity, out tempEntity))
                {
                    question += " " + tempEntity.Entity;
                    if (myEntity != "干什么")
                    {
                        preEntitySet += " " + tempEntity.Entity;
                    }

                }
            }
            if (!flag)
            {
                question += " " + preEntitySet;
            }
            string responseString = string.Empty;

            var query = question; //User Query
            var knowledgebaseId = "0b8f820a-1bef-4fc1-b7e9-eb5ee7692128"; // Use knowledge base -- 问数值.
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
                if (response.Score != 0)
                {
                    await context.PostAsync(preEntitySet + "----" + question +"Answer:"+ response.Answer + response.Score + result.TopScoringIntent.Intent + result.Entities[0].StartIndex + result.Query[0]);
                }
                else
                {
                    await context.PostAsync("对不起，小白在学校官网上无法找到答案");
                }

            }
            context.Wait(MessageReceived);
        }
    }
}