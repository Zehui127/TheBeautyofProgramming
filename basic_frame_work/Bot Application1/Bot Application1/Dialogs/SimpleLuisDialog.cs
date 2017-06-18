using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace Bot_Application1.Dialogs
{
    [LuisModel("f6b501c3-9ad2-4ce6-a086-3d9f2c0c329b","5ae56471cf00425f8f949ead61af47fd" )]
    [Serializable]
    public class SimpleLuisDialog : LuisDialog<object>
    {
        public const string Department = "院系";
        public const string Position = "职位";

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