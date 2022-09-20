using Telegram.Bot;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

string token = "5714853777:AAE9em_gc-pwHhilk57EXzM89ZmHpehU3So";

var client = new TelegramBotClient(token);

client.StartReceiving(Update, Eror);
Console.ReadKey();

async Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
{
    var message = update.Message;
    if (message.Text.ToLower() == "команды")
    {
        botClient.SendTextMessageAsync(message.Chat.Id, "fd",
            //"//DownloadPhoto - скачать фото",
            disableNotification: true,
            replyMarkup: new ReplyKeyboardMarkup(
                new KeyboardButton[] {
                    "Ну ка", "" }));
    }
    if (message.Photo != null)
    {
        var FileId = message.Photo.Last().FileId;
        var FileInfo = await botClient.GetFileAsync(FileId);
        var FilePath = FileInfo.FilePath;

        string destinationFilePath = $@"E:\TelegramBot\Photo\_{FileId}.jpg";
        await using FileStream fs = System.IO.File.OpenWrite(destinationFilePath);
        await botClient.DownloadFileAsync(FilePath, destination: fs);
    }
}

async Task Eror(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
{

}

//good