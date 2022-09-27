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
    if(message.Text == "/start")
    {
        botClient.SendTextMessageAsync(message.Chat.Id, text: "Этот бот позволит тебе хранить любые файлы, " +
            "которые ты ему отправишь и в дальнейшем сможешь скачать их.\nЕсли ты еще не пользовался этим ботом, " +
            "то может отправить любой файл, который он сохранит в недрах своего сервера, для этого выбери команду /UploadFile." +
            "\nЕсли ты уже отправлял какие-либо файлы, и хочешь скачать их, то выбери команду /DownloadFile");
        return;
    }

    if (message.Text == "/UploadFile")
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Отправь любой файл, который должен сохранить бот:");
        return;
    }

    //if (message.Text == "Upload Photo")
    //{
    //    await botClient.SendTextMessageAsync(message.Chat.Id, "Send a photo");
    //    return;
    //}
    //else if (message.Text == "Upload Document")
    //{
    //    await botClient.SendTextMessageAsync(message.Chat.Id, "Send a documnet");
    //    return;
    //}
    //else if (message.Text == "Upload Video")
    //{
    //    await botClient.SendTextMessageAsync(message.Chat.Id, "Send a video");
    //    return;
    //}
    //else if (message.Text == "Upload Audio")
    //{
    //    await botClient.SendTextMessageAsync(message.Chat.Id, "Send a audio");
    //    return;
    //}

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
    string[] DirectoryDocument = System.IO.Directory.GetFiles(@"Files\Document\");
    string[] DirectoryVideo = System.IO.Directory.GetFiles(@"Files\Video\");
    string[] DirectoryAudio = System.IO.Directory.GetFiles(@"Files\Audio\");
    string namePhoto = "";
    string nameDocument = "";
    string nameVideo = "";
    string nameAudio = "";
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
    foreach (var item in DirectoryDocument)
    {
        FileInfo DocumentInfo = new FileInfo(item);
        nameDocument = DocumentInfo.Name;
        if(message.Text == nameDocument)
        {
            await using Stream stream = System.IO.File.OpenRead($@"Files\Document\{nameDocument}");
            await botClient.SendDocumentAsync(message.Chat.Id, stream);
            await botClient.SendTextMessageAsync(message.Chat.Id, "Take your document");
            return;
        }
    }
    foreach (var item in DirectoryVideo)
    {
        FileInfo VideoInfo = new FileInfo(item);
        nameVideo = VideoInfo.Name;
        if (message.Text == nameVideo)
        {
            await using Stream stream = System.IO.File.OpenRead($@"Files\Video\{nameVideo}");
            await botClient.SendVideoAsync(message.Chat.Id, stream);
            await botClient.SendTextMessageAsync(message.Chat.Id, "Take your video");
            return;
        }
    }
    foreach (var item in DirectoryAudio)
    {
        FileInfo AudioInfo = new FileInfo(item);
        nameAudio = AudioInfo.Name;
        if (message.Text == nameAudio)
        {
            await using Stream stream = System.IO.File.OpenRead($@"Files\Audio\{nameAudio}");
            await botClient.SendAudioAsync(message.Chat.Id, stream);
            await botClient.SendTextMessageAsync(message.Chat.Id, "Take your audio");
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

        string destinationFilePath = $@"Files\Photo\{FileId}.jpg";
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
        string destinationFilePath = $@"Files\Document\{message.Document.FileName}";
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
        string destinationFilePath = $@"Files\Audio\{message.Audio.FileName}";
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
        string destinationFilePath = $@"Files\Video\{message.Video.FileName}";
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
        foreach (var item in ReposFilePhoto)
        {
            FileInfo newItem = new FileInfo(item);
            getFilePhoto += $"/\n{newItem.Name}\n";
        }
        await botClient.SendTextMessageAsync
            (
            callbackQuery.Message.Chat.Id,
            $"Всего фотографий: {ReposFilePhoto.Length}\nСписок Фото:\n {getFilePhoto}\n" +
            $"Cкопируй название файла который ты хочешь скачать и отправь мне!");
        return;
    }
    if (callbackQuery.Data.StartsWith("Download_Document"))
    {
        string[] ReposFileDocument = System.IO.Directory.GetFiles($@"Files\Document\");
        string getFileDocument = ""; 
        foreach (var item in ReposFileDocument)
        {
            FileInfo newItem = new FileInfo(item);
            getFileDocument += $"/\n{newItem.Name}\n";
        }
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id,
            $"Всего документов: {ReposFileDocument.Length}\nСписок документов:\n{getFileDocument}\n" +
            $"Скопируй название файла который ты хочешь скачать и отправь мне!");
        return;
    }
    if (callbackQuery.Data.StartsWith("Download_Video"))
    {
        string[] ReposFileVideo= System.IO.Directory.GetFiles($@"Files\Video\");
        string getFileVideo = "";
        foreach (var item in ReposFileVideo)
        {
            FileInfo newItem = new FileInfo(item);
            getFileVideo += $"/\n{newItem.Name}\n";
        }
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id,
            $"Всего видеороликов: {ReposFileVideo.Length}\nСписок видеороликов:\n{getFileVideo}\n" +
            $"Скопируй название файла который ты хочешь скачать и отправь мне!");
        return;
    }
    if (callbackQuery.Data.StartsWith("Download_Audio"))
    {
        string[] ReposFileAudio = System.IO.Directory.GetFiles($@"Files\Audio\");
        string getFileAudio = "";
        foreach (var item in ReposFileAudio)
        {
            FileInfo newItem = new FileInfo(item);
            getFileAudio += $"/\n{newItem.Name}\n";
        }
        await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id,
            $"Всего аудиофайлов: {ReposFileAudio.Length}\nСписок аудиофайлов:\n{getFileAudio}\n" +
            $"Скопируй название файла который ты хочешь скачать и отправь мне!");
        return;
    }
}


async Task Eror(ITelegramBotClient arg1, Exception arg2, CancellationToken arg3)
{

}
