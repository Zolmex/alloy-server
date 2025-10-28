#region

using Common.Resources.Xml;
using Discord;
using Discord.WebSocket;
using Image = System.Drawing.Image;
using ImageFormat = System.Drawing.Imaging.ImageFormat;

#endregion

namespace DiscordBot
{
    public static class DiscordBotOutput
    {
        private const ulong GENERAL_CHAT = 1231580708292661313;
        public static IMessageChannel GeneralChannel;

        public static async void Load(DiscordSocketClient client)
        {
            GeneralChannel = await client.GetChannelAsync(GENERAL_CHAT) as IMessageChannel;
        }

        public static async void ShowItem(string itemName)
        {
            var desc = XmlLibrary.Id2Item(itemName);
            if (desc == null) return;
            Image sprite = ImageHelper.GetSpriteFromSheet(desc.Texture.File, desc.Texture.Index);
            using (var imgStream = new MemoryStream())
            {
                sprite.Save(imgStream, ImageFormat.Png);
                var build = new EmbedBuilder();
                build.WithImageUrl("attachment://anyImageName.png"); //or build.WithImageUrl("")
                await GeneralChannel.SendFileAsync(imgStream, "anyImageName.png", "", false, build.Build());
            }
        }

        public static async void SendMessage(string message)
        {
            await GeneralChannel.SendMessageAsync(message);
        }
    }
}