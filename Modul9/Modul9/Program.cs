using Telegram.Bot;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

string token = "5714853777:AAE9em_gc-pwHhilk57EXzM89ZmHpehU3So";

var client = new TelegramBotClient(token);

client.StartReceiving(Update, Eror);

var me = await client.GetMeAsync();
Console.WriteLine($"Начал прослушку @{me.Username}");
Console.ReadKey();

async Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
{
    if(update.Type == UpdateType.Message && update?.Message?.Text != null)
    {
        await HandleMessage(botClient, update.Message);
        return;
    }

    if(update?.Message?.Photo != null || update?.Message?.Audio != null || update?.Message?.Document != null || update?.Message?.Video != null)
    {
        await HandleOtherMessage(botClient, update.Message);
        return;
    }

    if(update.Type == UpdateType.CallbackQuery)
    {
        await HandleCallbackQuery(botClient, update.CallbackQuery);
        return;
    }
}
async Task HandleMessage(ITelegramBotClient botClient, Message message) //метод обработки сообщения
{
    Console.WriteLine(message);
    if(message.Text == "/start")
    {
        botClient.SendTextMessageAsync(message.Chat.Id, text: "Choose commands: /DownloadFile || /UploadFile");
        return;
    }

    if (message.Text == "/UploadFile")
    {
        ReplyKeyboardMarkup keyboard = new(new[]
        {
            new KeyboardButton[] { "Upload Photo", "Upload Document" },
            new KeyboardButton[] { "Upload Video", "Upload Audio" }
        })
        {
            ResizeKeyboard = true
        };

        await botClient.SendTextMessageAsync(message.Chat.Id, "Choose:", replyMarkup: keyboard);
        return;
    }

    if (message.Text == "Upload Photo")
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Send a photo");
        return;
    }
    else if (message.Text == "Upload Document")
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Send a documnet");
        return;
    }
    else if (message.Text == "Upload Video")
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Send a video");
        return;
    }
    else if (message.Text == "Upload Audio")
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Send a audio");
        return;
    }

    if (message.Text == "/DownloadFile")
    {
        InlineKeyboardMarkup keyboard = new(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Download Photo", "Download_Photo"),
                InlineKeyboardButton.WithCallbackData("Download Documnet", "Download_Document"),
            },
                        new[]
            {
                InlineKeyboardButton.WithCallbackData("Download Video", "Download_Video"),
                InlineKeyboardButton.WithCallbackData("Download Audio", "Download_Audio"),
            },
        });
        await botClient.SendTextMessageAsync(message.Chat.Id, "Choose inline:", replyMarkup: keyboard);
        return;
    }
    string[] DirectoryPhoto = System.IO.Directory.GetFiles(@"Files\Photo\");
    string namePhoto = "";
    foreach (var item in DirectoryPhoto)
    {
        FileInfo PhotoInfo = new FileInfo(item);
        namePhoto = PhotoInfo.Name;
        if (message.Text == namePhoto)
        {
            await using Stream stream = System.IO.File.OpenRead($@"Files\Photo\{namePhoto}");
            await botClient.SendPhotoAsync(message.Chat.Id, stream);
            await botClient.SendTextMessageAsync(message.Chat.Id, "Take your photo");
            return;
        }
    }
    await botClient.SendTextMessageAsync(message.Chat.Id, $"You said: \n{message.Text}");
    return;
}

async Task HandleOtherMessage(ITelegramBotClient botClient, Message message)
{
    if (message.Photo != null)
    {
        var FileId = message.Photo.Last().FileId;
        var FileInfo = await botClient.GetFileAsync(FileId);
        var FilePath = FileInfo.FilePath;

        string destinationFilePath = $@"Files\Photo\_{FileId}.jpg";
        await using FileStream fs = System.IO.File.OpenWrite(destinationFilePath);
        await botClient.DownloadFileAsync(FilePath, destination: fs);
        await botClient.SendTextMessageAsync(message.Chat.Id, "Photo downloaded");
        return;
    }
    if (message.Document != null)
    {
        var FileId = message.Document.FileId;
        var FileInfo = await botClient.GetFileAsync(FileId);
        var FilePath = FileInfo.FilePath;
        string destinationFilePath = $@"Files\Document\_{message.Document.FileName}";
        await using FileStream fs = System.IO.File.OpenWrite(destinationFilePath);
        await botClient.DownloadFileAsync(FilePath, destination: fs);
        await botClient.SendTextMessageAsync(message.Chat.Id, "Document downloaded");
        return;
    }
    if (message.Audio != null)
    {
        var FileId = message.Audio.FileId;
        var FileInfo = await botClient.GetFileAsync(FileId);
        var FilePath = FileInfo.FilePath;
        string destinationFilePath = $@"Files\Audio\_{message.Audio.FileName}";
        await using FileStream fs = System.IO.File.OpenWrite(destinationFilePath);
        await botClient.DownloadFileAsync(FilePath, destination: fs);
        await botClient.SendTextMessageAsync(message.Chat.Id, "Audio downloaded");
        return;
    }
    if (message.Video != null)
    {
        var FileId = message.Video.FileId;
        var FileInfo = await botClient.GetFileAsync(FileId);
        var FilePath = FileInfo.FilePath;
        string destinationFilePath = $@"Files\Video\_{message.Video.FileName}";
        await using FileStream fs = System.IO.File.OpenWrite(destinationFilePath);
        await botClient.DownloadFileAsync(FilePath, destination: fs);
        await botClient.SendTextMessageAsync(message.Chat.Id, "Video downloaded");
        return;
    }
    return;
}

async Task HandleCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
{
    if (callbackQuery.Data.StartsWith("Download_Photo"))
    {
        string[] ReposFilePhoto = System.IO.Directory.GetFiles($@"Files\Photo\");
        string getFilePhoto = "";
        int i = 1;
        foreach (var item in ReposFilePhoto)
        {
            FileInfo newItem = new FileInfo(item);
            //getFilePhoto += $"/{item.Replace(item, "Photo")}{i++} Время создания файла: {newItem.CreationTime}\n";
            getFilePhoto += $"/\n{newItem.Name}\n";
        }
        await botClient.SendTextMessageAsync
            (
            callbackQuery.Message.Chat.Id,
            $"Всего фотографий: {ReposFilePhoto.Length}\nСписок Фото:\n {getFilePhoto}\n" +
            $"Cкопируй название файла который ты хочешь скачать и отправь мне!"
            );
        return;
    }
    if (callbackQuery.Data.StartsWith("Download_Documnet"))
    {
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Список документов");
        return;
    }
    if (callbackQuery.Data.StartsWith("Download_Video"))
    {
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Список видео");
        return;
    }
    if (callbackQuery.Data.StartsWith("Download_Music"))
    {
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Список музыки");
        return;
    }
    await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $"You choose with data: {callbackQuery.Data}");
    return;
}


async Task Eror(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
{

}
